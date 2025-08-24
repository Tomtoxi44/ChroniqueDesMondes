using Microsoft.Extensions.Logging;

namespace Cdm.Common.Services;

public interface IEmailService
{
    Task<bool> SendInvitationEmailAsync(string toEmail, string campaignName, string inviterName, string invitationLink, string? message = null);
    Task<bool> SendInvitationAcceptedEmailAsync(string toEmail, string campaignName, string playerName);
    Task<bool> SendInvitationRejectedEmailAsync(string toEmail, string campaignName, string playerName);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> logger;

    public EmailService(ILogger<EmailService> logger)
    {
        this.logger = logger;
    }

    public async Task<bool> SendInvitationEmailAsync(string toEmail, string campaignName, string inviterName, string invitationLink, string? message = null)
    {
        // Pour l'instant, on simule l'envoi d'email avec des logs
        this.logger.LogInformation("📧 Email d'invitation simulé:");
        this.logger.LogInformation("  To: {Email}", toEmail);
        this.logger.LogInformation("  Campaign: {Campaign}", campaignName);
        this.logger.LogInformation("  Inviter: {Inviter}", inviterName);
        this.logger.LogInformation("  Link: {Link}", invitationLink);
        
        if (!string.IsNullOrEmpty(message))
        {
            this.logger.LogInformation("  Message: {Message}", message);
        }

        // Simulation d'un délai d'envoi
        await Task.Delay(100);

        this.logger.LogInformation("✅ Email d'invitation envoyé avec succès (simulé)");
        return true;
    }

    public async Task<bool> SendInvitationAcceptedEmailAsync(string toEmail, string campaignName, string playerName)
    {
        this.logger.LogInformation("📧 Email acceptation invitation simulé:");
        this.logger.LogInformation("  To: {Email} - {Player} a accepté votre invitation pour {Campaign}", toEmail, playerName, campaignName);
        
        await Task.Delay(100);
        return true;
    }

    public async Task<bool> SendInvitationRejectedEmailAsync(string toEmail, string campaignName, string playerName)
    {
        this.logger.LogInformation("📧 Email rejet invitation simulé:");
        this.logger.LogInformation("  To: {Email} - {Player} a refusé votre invitation pour {Campaign}", toEmail, playerName, campaignName);
        
        await Task.Delay(100);
        return true;
    }
}