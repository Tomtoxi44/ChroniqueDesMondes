using Cdm.Business.Common.Models.Campaign.Invitation;
using Cdm.Common;
using Cdm.Common.Services;
using Cdm.Data;
using Cdm.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace Cdm.Business.Common.Business.Campaigns;

public class InvitationService
{
    private readonly AppDbContext dbContext;
    private readonly IEmailService emailService;
    private readonly ILogger<InvitationService> logger;

    public InvitationService(AppDbContext dbContext, IEmailService emailService, ILogger<InvitationService> logger)
    {
        this.dbContext = dbContext;
        this.emailService = emailService;
        this.logger = logger;
    }

    /// <summary>
    /// Envoie une invitation à rejoindre une campagne
    /// </summary>
    public async Task<InvitationView> SendInvitationAsync(int campaignId, InvitationRequest request, int invitedById)
    {
        // Vérifier que la campagne existe et que l'utilisateur a le droit d'inviter
        var campaign = await this.dbContext.Campaigns
            .Include(c => c.CreatedBy)
            .FirstOrDefaultAsync(c => c.Id == campaignId && c.IsActive);

        if (campaign == null)
            throw new BusinessException("Campaign not found");

        if (campaign.CreatedById != invitedById)
            throw new BusinessException("You can only invite players to campaigns you created");

        // Vérifier si une invitation active existe déjà
        var existingInvitation = await this.dbContext.CampaignInvitations
            .FirstOrDefaultAsync(ci => ci.CampaignId == campaignId && 
                                      ci.Email == request.Email && 
                                      ci.Status == InvitationStatus.Pending);

        if (existingInvitation != null)
            throw new BusinessException("An active invitation already exists for this email");

        // Vérifier si l'utilisateur fait déjà partie de la campagne
        var existingUser = await this.dbContext.Users
            .FirstOrDefaultAsync(u => u.UserEmail == request.Email);

        if (existingUser != null)
        {
            var existingParticipant = await this.dbContext.CampaignParticipants
                .FirstOrDefaultAsync(cp => cp.CampaignId == campaignId && 
                                          cp.UserId == existingUser.Id && 
                                          cp.IsActive);

            if (existingParticipant != null)
                throw new BusinessException("This user is already a participant in this campaign");
        }

        // Créer l'invitation
        var invitation = new CampaignInvitation
        {
            CampaignId = campaignId,
            Email = request.Email,
            UserId = existingUser?.Id,
            Status = InvitationStatus.Pending,
            Token = this.GenerateInvitationToken(),
            InvitedById = invitedById,
            Message = request.Message,
            ExpiresAt = DateTime.UtcNow.AddDays(request.ExpirationDays)
        };

        this.dbContext.CampaignInvitations.Add(invitation);
        await this.dbContext.SaveChangesAsync();

        // Envoyer l'email d'invitation
        var inviterName = campaign.CreatedBy.UserName;
        var invitationLink = $"https://localhost:7153/invitation/{invitation.Token}"; // TODO: URL configurable
        
        try
        {
            await this.emailService.SendInvitationEmailAsync(
                request.Email, 
                campaign.Name, 
                inviterName, 
                invitationLink, 
                request.Message);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to send invitation email to {Email}", request.Email);
            // On continue même si l'email échoue
        }

        return await this.GetInvitationViewAsync(invitation.Id);
    }

    /// <summary>
    /// Répond à une invitation (accepter/refuser)
    /// </summary>
    public async Task<bool> RespondToInvitationAsync(string token, InvitationResponse response, int? userId = null)
    {
        var invitation = await this.dbContext.CampaignInvitations
            .Include(ci => ci.Campaign)
            .ThenInclude(c => c.CreatedBy)
            .FirstOrDefaultAsync(ci => ci.Token == token);

        if (invitation == null)
            throw new BusinessException("Invitation not found");

        if (invitation.Status != InvitationStatus.Pending)
            throw new BusinessException("This invitation has already been responded to");

        if (DateTime.UtcNow > invitation.ExpiresAt)
        {
            invitation.Status = InvitationStatus.Expired;
            await this.dbContext.SaveChangesAsync();
            throw new BusinessException("This invitation has expired");
        }

        // Mettre à jour l'invitation
        invitation.Status = response.Accept ? InvitationStatus.Accepted : InvitationStatus.Rejected;
        invitation.RespondedAt = DateTime.UtcNow;

        if (response.Accept)
        {
            // Si acceptée, créer le participant
            var user = await this.GetOrCreateUserForInvitationAsync(invitation, userId);

            var participant = new CampaignParticipant
            {
                CampaignId = invitation.CampaignId,
                UserId = user.Id,
                Role = ParticipantRole.Player,
                Permissions = ParticipantPermissions.ReadOnly | ParticipantPermissions.CreateCharacters | ParticipantPermissions.EditOwnCharacters,
                InvitationId = invitation.Id
            };

            this.dbContext.CampaignParticipants.Add(participant);

            // Notifier le MJ
            try
            {
                await this.emailService.SendInvitationAcceptedEmailAsync(
                    invitation.Campaign.CreatedBy.UserEmail,
                    invitation.Campaign.Name,
                    user.UserName);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to send acceptance notification email");
            }
        }
        else
        {
            // Notifier le MJ du refus
            try
            {
                await this.emailService.SendInvitationRejectedEmailAsync(
                    invitation.Campaign.CreatedBy.UserEmail,
                    invitation.Campaign.Name,
                    invitation.Email);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to send rejection notification email");
            }
        }

        await this.dbContext.SaveChangesAsync();
        return response.Accept;
    }

    /// <summary>
    /// Récupère les membres d'une campagne (participants + invitations en attente)
    /// </summary>
    public async Task<CampaignMembersView> GetCampaignMembersAsync(int campaignId, int requestingUserId)
    {
        var campaign = await this.dbContext.Campaigns
            .Include(c => c.CreatedBy)
            .FirstOrDefaultAsync(c => c.Id == campaignId && c.IsActive);

        if (campaign == null)
            throw new BusinessException("Campaign not found");

        // Vérifier les permissions
        if (campaign.CreatedById != requestingUserId)
        {
            var participant = await this.dbContext.CampaignParticipants
                .FirstOrDefaultAsync(cp => cp.CampaignId == campaignId && 
                                          cp.UserId == requestingUserId && 
                                          cp.IsActive);

            if (participant == null || !participant.Permissions.HasFlag(ParticipantPermissions.ManageParticipants))
                throw new BusinessException("You don't have permission to view campaign members");
        }

        // Récupérer les participants
        var participants = await this.dbContext.CampaignParticipants
            .Include(cp => cp.User)
            .Where(cp => cp.CampaignId == campaignId && cp.IsActive)
            .Select(cp => new ParticipantView
            {
                Id = cp.Id,
                CampaignId = cp.CampaignId,
                UserId = cp.UserId,
                UserName = cp.User.UserName,
                UserEmail = cp.User.UserEmail,
                Role = cp.Role,
                Permissions = cp.Permissions,
                JoinedDate = cp.JoinedDate,
                IsActive = cp.IsActive,
                InvitationId = cp.InvitationId
            })
            .ToListAsync();

        // Récupérer les invitations en attente
        var pendingInvitations = await this.dbContext.CampaignInvitations
            .Include(ci => ci.InvitedBy)
            .Where(ci => ci.CampaignId == campaignId && ci.Status == InvitationStatus.Pending)
            .Select(ci => new InvitationView
            {
                Id = ci.Id,
                CampaignId = ci.CampaignId,
                CampaignName = campaign.Name,
                Email = ci.Email,
                UserId = ci.UserId,
                Status = ci.Status,
                Token = ci.Token,
                InvitedByName = ci.InvitedBy.UserName,
                Message = ci.Message,
                CreatedDate = ci.CreatedDate,
                ExpiresAt = ci.ExpiresAt,
                RespondedAt = ci.RespondedAt
            })
            .ToListAsync();

        return new CampaignMembersView
        {
            CampaignId = campaignId,
            CampaignName = campaign.Name,
            GameMasterName = campaign.CreatedBy.UserName,
            Participants = participants,
            PendingInvitations = pendingInvitations
        };
    }

    /// <summary>
    /// Supprime un participant d'une campagne
    /// </summary>
    public async Task RemoveParticipantAsync(int campaignId, int participantId, int requestingUserId)
    {
        var campaign = await this.dbContext.Campaigns
            .FirstOrDefaultAsync(c => c.Id == campaignId && c.IsActive);

        if (campaign == null)
            throw new BusinessException("Campaign not found");

        if (campaign.CreatedById != requestingUserId)
            throw new BusinessException("Only the game master can remove participants");

        var participant = await this.dbContext.CampaignParticipants
            .FirstOrDefaultAsync(cp => cp.Id == participantId && cp.CampaignId == campaignId);

        if (participant == null)
            throw new BusinessException("Participant not found");

        participant.IsActive = false;
        await this.dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Annule une invitation en attente
    /// </summary>
    public async Task CancelInvitationAsync(int invitationId, int requestingUserId)
    {
        var invitation = await this.dbContext.CampaignInvitations
            .Include(ci => ci.Campaign)
            .FirstOrDefaultAsync(ci => ci.Id == invitationId);

        if (invitation == null)
            throw new BusinessException("Invitation not found");

        if (invitation.Campaign.CreatedById != requestingUserId)
            throw new BusinessException("You can only cancel invitations you sent");

        if (invitation.Status != InvitationStatus.Pending)
            throw new BusinessException("This invitation cannot be cancelled");

        invitation.Status = InvitationStatus.Cancelled;
        await this.dbContext.SaveChangesAsync();
    }

    private async Task<InvitationView> GetInvitationViewAsync(int invitationId)
    {
        return await this.dbContext.CampaignInvitations
            .Include(ci => ci.Campaign)
            .Include(ci => ci.InvitedBy)
            .Where(ci => ci.Id == invitationId)
            .Select(ci => new InvitationView
            {
                Id = ci.Id,
                CampaignId = ci.CampaignId,
                CampaignName = ci.Campaign.Name,
                Email = ci.Email,
                UserId = ci.UserId,
                Status = ci.Status,
                Token = ci.Token,
                InvitedByName = ci.InvitedBy.UserName,
                Message = ci.Message,
                CreatedDate = ci.CreatedDate,
                ExpiresAt = ci.ExpiresAt,
                RespondedAt = ci.RespondedAt
            })
            .FirstAsync();
    }

    private async Task<User> GetOrCreateUserForInvitationAsync(CampaignInvitation invitation, int? userId)
    {
        if (userId.HasValue)
        {
            var user = await this.dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId.Value);

            if (user == null || user.UserEmail != invitation.Email)
                throw new BusinessException("User not found or email mismatch");

            return user;
        }

        if (invitation.UserId.HasValue)
        {
            return await this.dbContext.Users
                .FirstAsync(u => u.Id == invitation.UserId.Value);
        }

        throw new BusinessException("User must be logged in to accept invitation");
    }

    private string GenerateInvitationToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }
}