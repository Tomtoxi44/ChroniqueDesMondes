using Cdm.Business.Common.Models.Campaign.Npc;
using Cdm.Common;
using Cdm.Common.Enums;
using Cdm.Data.Dnd;
using Cdm.Data.Dnd.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Cdm.Business.Common.Business.Campaigns;

public class NpcBusiness
{
    private readonly DndDbContext _context;

    public NpcBusiness(DndDbContext context)
    {
        _context = context;
    }

    public async Task<NpcView> CreateNpcAsync(NpcRequest request, int userId)
    {
        // Vérifier que l'utilisateur a accès au chapitre
        var chapter = await _context.Chapters
            .Include(c => c.Campaign)
            .FirstOrDefaultAsync(c => c.Id == request.ChapterId);

        if (chapter == null)
            throw new BusinessException("Chapter not found");

        if (chapter.Campaign.CreatedById != userId)
            throw new BusinessException("You don't have permission to add NPCs to this chapter");

        // Valider la compatibilité GameType
        if (chapter.Campaign.GameType != GameType.Generic && request.GameType != GameType.Generic)
        {
            if (chapter.Campaign.GameType != request.GameType)
                throw new BusinessException($"Cannot add {request.GameType} NPC to {chapter.Campaign.GameType} campaign");
        }

        // Créer le NPC basé sur le GameType
        CharacterDnd npcCharacter;

        if (request.GameType == GameType.DnD || chapter.Campaign.GameType == GameType.DnD)
        {
            // Pour D&D, on a besoin des propriétés spécifiques
            var dndProperties = ParseDndProperties(request.DndProperties);
            
            npcCharacter = new CharacterDnd
            {
                UserId = userId,
                Name = request.Name,
                Background = request.Description,
                Life = dndProperties.HitPoints,
                Leveling = dndProperties.Level,
                IsNpc = true,
                ChapterId = request.ChapterId,
                IsHostile = request.IsHostile,
                GameType = request.GameType,
                Tags = request.Tags != null ? JsonSerializer.Serialize(request.Tags) : null,
                CreatedAt = DateTime.UtcNow,
                
                // Propriétés D&D spécifiques
                Class = dndProperties.Class,
                ClassArmor = dndProperties.ArmorClass,
                Strong = dndProperties.Strength,
                Dexterity = dndProperties.Dexterity,
                Constitution = dndProperties.Constitution,
                Intelligence = dndProperties.Intelligence,
                Wisdoms = dndProperties.Wisdom,
                Charism = dndProperties.Charisma
            };
        }
        else
        {
            // Pour Generic/Skyrim, propriétés de base seulement
            npcCharacter = new CharacterDnd
            {
                UserId = userId,
                Name = request.Name,
                Background = request.Description,
                Life = 1, // Valeur par défaut
                Leveling = 1,
                IsNpc = true,
                ChapterId = request.ChapterId,
                IsHostile = request.IsHostile,
                GameType = request.GameType,
                Tags = request.Tags != null ? JsonSerializer.Serialize(request.Tags) : null,
                CreatedAt = DateTime.UtcNow,
                
                // Valeurs par défaut pour D&D (requises par la base)
                Class = "NPC",
                ClassArmor = 10,
                Strong = 10,
                Dexterity = 10,
                Constitution = 10,
                Intelligence = 10,
                Wisdoms = 10,
                Charism = 10
            };
        }

        _context.CharactersDnd.Add(npcCharacter);
        await _context.SaveChangesAsync();

        return await GetNpcViewAsync(npcCharacter.Id);
    }

    public async Task<NpcView> UpdateNpcAsync(int id, NpcUpdateRequest request, int userId)
    {
        var npc = await _context.CharactersDnd
            .Include(c => c.Chapter)
            .ThenInclude(ch => ch!.Campaign)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsNpc);

        if (npc == null)
            throw new BusinessException("NPC not found");

        if (npc.Chapter?.Campaign.CreatedById != userId)
            throw new BusinessException("You don't have permission to update this NPC");

        // Mettre à jour les propriétés
        if (!string.IsNullOrEmpty(request.Name))
            npc.Name = request.Name;

        if (!string.IsNullOrEmpty(request.Description))
            npc.Background = request.Description;

        if (request.IsHostile.HasValue)
            npc.IsHostile = request.IsHostile.Value;

        if (request.Tags != null)
            npc.Tags = JsonSerializer.Serialize(request.Tags);

        // Mettre à jour les propriétés D&D si fournies
        if (!string.IsNullOrEmpty(request.DndProperties))
        {
            var dndProperties = ParseDndProperties(request.DndProperties);
            npc.Class = dndProperties.Class;
            npc.ClassArmor = dndProperties.ArmorClass;
            npc.Strong = dndProperties.Strength;
            npc.Dexterity = dndProperties.Dexterity;
            npc.Constitution = dndProperties.Constitution;
            npc.Intelligence = dndProperties.Intelligence;
            npc.Wisdoms = dndProperties.Wisdom;
            npc.Charism = dndProperties.Charisma;
            npc.Life = dndProperties.HitPoints;
            npc.Leveling = dndProperties.Level;
        }

        await _context.SaveChangesAsync();

        return await GetNpcViewAsync(id);
    }

    public async Task DeleteNpcAsync(int id, int userId)
    {
        var npc = await _context.CharactersDnd
            .Include(c => c.Chapter)
            .ThenInclude(ch => ch!.Campaign)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsNpc);

        if (npc == null)
            throw new BusinessException("NPC not found");

        if (npc.Chapter?.Campaign.CreatedById != userId)
            throw new BusinessException("You don't have permission to delete this NPC");

        // Vérifier s'il y a des ContentBlocks liés
        var linkedBlocks = await _context.ContentBlocks
            .CountAsync(cb => cb.CharacterId == id);

        if (linkedBlocks > 0)
            throw new BusinessException("Cannot delete NPC that has linked dialogue blocks. Remove the blocks first.");

        _context.CharactersDnd.Remove(npc);
        await _context.SaveChangesAsync();
    }

    public async Task<List<NpcView>> GetNpcsByChapterAsync(int chapterId, int userId)
    {
        var chapter = await _context.Chapters
            .Include(c => c.Campaign)
            .FirstOrDefaultAsync(c => c.Id == chapterId);

        if (chapter == null)
            throw new BusinessException("Chapter not found");

        // Vérifier l'accès
        if (chapter.Campaign.CreatedById != userId && !chapter.Campaign.IsPublic)
            throw new BusinessException("You don't have permission to view this chapter's NPCs");

        var npcs = await _context.CharactersDnd
            .Include(c => c.ContentBlocks)
            .Where(c => c.ChapterId == chapterId && c.IsNpc)
            .ToListAsync();

        return npcs.Select(MapToNpcView).ToList();
    }

    public async Task<List<NpcView>> GetHostileNpcsByChapterAsync(int chapterId, int userId)
    {
        var chapter = await _context.Chapters
            .Include(c => c.Campaign)
            .FirstOrDefaultAsync(c => c.Id == chapterId);

        if (chapter == null)
            throw new BusinessException("Chapter not found");

        if (chapter.Campaign.CreatedById != userId)
            throw new BusinessException("You don't have permission to view this chapter's combat NPCs");

        var hostileNpcs = await _context.CharactersDnd
            .Where(c => c.ChapterId == chapterId && c.IsNpc && c.IsHostile)
            .ToListAsync();

        return hostileNpcs.Select(MapToNpcView).ToList();
    }

    public async Task<NpcView?> GetNpcByIdAsync(int id, int userId)
    {
        var npc = await _context.CharactersDnd
            .Include(c => c.Chapter)
            .ThenInclude(ch => ch!.Campaign)
            .Include(c => c.ContentBlocks)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsNpc);

        if (npc == null)
            return null;

        // Vérifier l'accès
        if (npc.Chapter?.Campaign.CreatedById != userId && npc.Chapter?.Campaign.IsPublic != true)
            throw new BusinessException("You don't have permission to view this NPC");

        return MapToNpcView(npc);
    }

    private async Task<NpcView> GetNpcViewAsync(int id)
    {
        var npc = await _context.CharactersDnd
            .Include(c => c.ContentBlocks)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsNpc);

        return MapToNpcView(npc!);
    }

    private static NpcView MapToNpcView(CharacterDnd npc)
    {
        var tags = new List<string>();
        if (!string.IsNullOrEmpty(npc.Tags))
        {
            try
            {
                tags = JsonSerializer.Deserialize<List<string>>(npc.Tags) ?? new List<string>();
            }
            catch
            {
                // Si la désérialisation échoue, on garde une liste vide
            }
        }

        return new NpcView
        {
            Id = npc.Id,
            ChapterId = npc.ChapterId ?? 0,
            Name = npc.Name,
            Description = npc.Background,
            GameType = npc.GameType,
            IsHostile = npc.IsHostile,
            Tags = tags,
            DndProperties = SerializeDndProperties(npc),
            GenericProperties = null, // TODO: implémenter si nécessaire
            IsSystemCharacter = npc.IsSystemCharacter,
            CreatedAt = npc.CreatedAt,
            DialogueBlocksCount = npc.ContentBlocks?.Count ?? 0
        };
    }

    private static string SerializeDndProperties(CharacterDnd npc)
    {
        var properties = new
        {
            Class = npc.Class,
            ArmorClass = npc.ClassArmor,
            HitPoints = npc.Life,
            Level = npc.Leveling,
            Strength = npc.Strong,
            Dexterity = npc.Dexterity,
            Constitution = npc.Constitution,
            Intelligence = npc.Intelligence,
            Wisdom = npc.Wisdoms,
            Charisma = npc.Charism
        };

        return JsonSerializer.Serialize(properties);
    }

    private static DndNpcProperties ParseDndProperties(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return new DndNpcProperties
            {
                Class = "NPC",
                ArmorClass = 10,
                HitPoints = 1,
                Level = 1,
                Strength = 10,
                Dexterity = 10,
                Constitution = 10,
                Intelligence = 10,
                Wisdom = 10,
                Charisma = 10
            };
        }

        try
        {
            return JsonSerializer.Deserialize<DndNpcProperties>(json) ?? new DndNpcProperties();
        }
        catch
        {
            return new DndNpcProperties();
        }
    }

    private class DndNpcProperties
    {
        public string Class { get; set; } = "NPC";
        public int ArmorClass { get; set; } = 10;
        public int HitPoints { get; set; } = 1;
        public int Level { get; set; } = 1;
        public int Strength { get; set; } = 10;
        public int Dexterity { get; set; } = 10;
        public int Constitution { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int Wisdom { get; set; } = 10;
        public int Charisma { get; set; } = 10;
    }
}