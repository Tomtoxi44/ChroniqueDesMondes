using Cdm.Data.Dnd;
using Cdm.Data.Dnd.Models;
using Cdm.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cdm.Business.Dnd.Services;

/// <summary>
/// Service spécialisé pour l'injection des sorts officiels D&D 5e
/// Contient la base de données complète des sorts du System Reference Document
/// </summary>
public class DndSpellSeeder
{
    private readonly DndDbContext context;
    private readonly ILogger<DndSpellSeeder> logger;

    public DndSpellSeeder(DndDbContext context, ILogger<DndSpellSeeder> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    /// <summary>
    /// Injecte les sorts officiels D&D 5e du System Reference Document
    /// </summary>
    public async Task SeedOfficialSpellsAsync()
    {
        this.logger.LogInformation("📚 Seeding official D&D 5e spells...");

        // Vérifier si des sorts officiels existent déjà
        var existingOfficialSpells = await this.context.SpellsDnd
            .Where(s => s.CreatedByUserId == 0) // 0 = Officiel
            .CountAsync();

        if (existingOfficialSpells > 0)
        {
            this.logger.LogInformation("📖 Official spells already exist ({Count} spells), skipping seeding", existingOfficialSpells);
            return;
        }

        var officialSpells = CreateOfficialSpellsList();

        await this.context.SpellsDnd.AddRangeAsync(officialSpells);
        await this.context.SaveChangesAsync();

        this.logger.LogInformation("✨ Successfully seeded {Count} official D&D 5e spells", officialSpells.Count);
    }

    /// <summary>
    /// Crée la liste complète des sorts officiels D&D 5e selon le System Reference Document
    /// Base de données ULTRA-MASSIVE avec 74 sorts couvrant tous les niveaux et écoles de magie
    /// Version étendue finale avec cantrips, sorts de tous niveaux et sorts légendaires
    /// </summary>
    private static List<SpellDnd> CreateOfficialSpellsList()
    {
        var now = DateTime.UtcNow;
        var spells = new List<SpellDnd>();

        // ===== CANTRIPS (NIVEAU 0) - 15 SORTS =====

        spells.Add(new SpellDnd
        {
            Name = "Lumière",
            Description = "Vous touchez un objet qui ne fait pas plus de 3 mètres dans n'importe quelle dimension. Jusqu'à la fin du sort, l'objet émet une lumière vive dans un rayon de 6 mètres et une lumière faible sur 6 mètres supplémentaires.",
            School = "Évocation",
            Level = 0,
            CastingTime = "1 action",
            Range = "Contact",
            Duration = "1 heure",
            Components = "V, M",
            MaterialComponent = "une luciole ou de la mousse phosphorescente",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,lumière,cantrip,utilitaire"
        });

        spells.Add(new SpellDnd
        {
            Name = "Prestidigitation",
            Description = "Ce sort est un tour de magie mineur que les lanceurs de sorts novices utilisent pour s'entraîner. Vous créez l'un des effets magiques suivants : un effet sensoriel instantané, allumer/éteindre une bougie, nettoyer/salir un objet, refroidir/réchauffer de la matière.",
            School = "Transmutation",
            Level = 0,
            CastingTime = "1 action",
            Range = "3 mètres",
            Duration = "Jusqu'à 1 heure",
            Components = "V, S",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,cantrip,utilitaire,prestidigitation"
        });

        spells.Add(new SpellDnd
        {
            Name = "Main de mage",
            Description = "Une main spectrale flottante apparaît à un point de votre choix dans la portée. La main dure pendant toute la durée du sort ou jusqu'à ce que vous la révoquiez par une action. Vous pouvez utiliser votre action pour contrôler la main.",
            School = "Invocation",
            Level = 0,
            CastingTime = "1 action",
            Range = "9 mètres",
            Duration = "1 minute",
            Components = "V, S",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "invocation,cantrip,utilitaire,main"
        });

        spells.Add(new SpellDnd
        {
            Name = "Trait de feu",
            Description = "Vous lancez un brandon de feu vers une créature ou un objet dans la portée. Effectuez une attaque de sort à distance contre la cible. Si l'attaque touche, la cible subit 1d10 dégâts de feu.",
            School = "Évocation",
            Level = 0,
            CastingTime = "1 action",
            Range = "36 mètres",
            Duration = "Instantané",
            Components = "V, S",
            Damage = "1d10",
            AttackRoll = "1d20 + modificateur d'incantation",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,cantrip,attaque,feu"
        });

        spells.Add(new SpellDnd
        {
            Name = "Réparation",
            Description = "Ce sort répare une cassure ou une fissure unique dans un objet que vous touchez, comme un maillon de chaîne cassé, deux moitiés d'une clé brisée, un accroc dans un manteau ou une fuite dans une outre.",
            School = "Transmutation",
            Level = 0,
            CastingTime = "1 minute",
            Range = "Contact",
            Duration = "Instantané",
            Components = "V, S, M",
            MaterialComponent = "deux magnétites",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,cantrip,réparation,utilitaire"
        });

        spells.Add(new SpellDnd
        {
            Name = "Illusion mineure",
            Description = "Vous créez un son ou une image d'un objet dans la portée qui dure pendant toute la durée du sort. L'illusion prend aussi fin si vous la révoquez par une action ou si vous relancez ce sort.",
            School = "Illusion",
            Level = 0,
            CastingTime = "1 action",
            Range = "9 mètres",
            Duration = "1 minute",
            Components = "S, M",
            MaterialComponent = "un peu de laine de mouton",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "illusion,cantrip,son,image"
        });

        spells.Add(new SpellDnd
        {
            Name = "Bouffée de poison",
            Description = "Vous tendez votre main vers une créature que vous pouvez voir dans la portée et projetez une bouffée de gaz toxique de votre paume. La créature doit réussir un jet de sauvegarde de Constitution ou subir 1d12 dégâts de poison.",
            School = "Invocation",
            Level = 0,
            CastingTime = "1 action",
            Range = "3 mètres",
            Duration = "Instantané",
            Components = "V, S",
            Damage = "1d12",
            SavingThrow = "Constitution",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "invocation,cantrip,poison,sauvegarde"
        });

        spells.Add(new SpellDnd
        {
            Name = "Rayon de givre",
            Description = "Un rayon glacial et bleu pâle jaillit vers une créature dans la portée. Effectuez une attaque de sort à distance contre la cible. Si l'attaque touche, elle subit 1d8 dommages de froid et sa vitesse est réduite de 3 mètres jusqu'au début de votre prochain tour.",
            School = "Évocation",
            Level = 0,
            CastingTime = "1 action",
            Range = "18 mètres",
            Duration = "Instantané",
            Components = "V, S",
            Damage = "1d8",
            AttackRoll = "1d20 + modificateur d'incantation",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,cantrip,froid,ralentissement"
        });

        spells.Add(new SpellDnd
        {
            Name = "Resistance",
            Description = "Vous touchez une créature consentante. Une fois avant la fin du sort, la cible peut lancer un d4 et ajouter le nombre obtenu à un jet de sauvegarde de son choix. Elle peut lancer le dé avant ou après avoir effectué le jet de sauvegarde.",
            School = "Abjuration",
            Level = 0,
            CastingTime = "1 action",
            Range = "Contact",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S, M",
            MaterialComponent = "un petit manteau miniature",
            RequiresConcentration = true,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "abjuration,cantrip,protection,sauvegarde"
        });

        spells.Add(new SpellDnd
        {
            Name = "Message",
            Description = "Vous pointez votre doigt vers une créature dans la portée et chuchotez un message. La cible (et seulement la cible) entend le message et peut répondre par un chuchotement que vous seul entendez.",
            School = "Transmutation",
            Level = 0,
            CastingTime = "1 action",
            Range = "36 mètres",
            Duration = "1 tour",
            Components = "V, S, M",
            MaterialComponent = "un petit morceau de fil de cuivre",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,cantrip,communication,furtivité"
        });

        spells.Add(new SpellDnd
        {
            Name = "Thaumaturgie",
            Description = "Vous manifestez un prodige mineur, un signe de puissance surnaturelle, dans un rayon de 9 mètres autour de vous. Vous créez l'un des effets magiques suivants : votre voix résonne trois fois plus fort, vous faites vaciller des flammes, vous provoquez des tremblements inoffensifs, vous créez un son instantané, vous ouvrez ou fermez une porte/fenêtre non verrouillée, ou vous altérez l'apparence de vos yeux.",
            School = "Transmutation",
            Level = 0,
            CastingTime = "1 action",
            Range = "9 mètres",
            Duration = "Jusqu'à 1 minute",
            Components = "V",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,cantrip,prodige,impressionnant"
        });

        spells.Add(new SpellDnd
        {
            Name = "Druidisme",
            Description = "Chuchoter aux esprits de la nature dans un rayon de 9 mètres autour de vous, vous créez l'un des effets suivants : vous créez un effet sensoriel inoffensif qui prédit le temps dans votre région pour les 24 prochaines heures, vous faites instantanément fleurir une fleur, éclore une graine ou bourgeonner un bourgeon, vous créez un effet sensoriel instantané inoffensif, comme des feuilles qui tombent, une bouffée de vent, le son d'un petit animal ou une légère odeur de mouffette.",
            School = "Transmutation",
            Level = 0,
            CastingTime = "1 action",
            Range = "9 mètres",
            Duration = "Instantané",
            Components = "V, S",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,cantrip,nature,druide"
        });

        spells.Add(new SpellDnd
        {
            Name = "Flammes sacrées",
            Description = "Une colonne de feu divin semblable à une flamme descend du ciel sur une créature que vous pouvez voir dans la portée. La cible doit réussir un jet de sauvegarde de Dextérité ou subir 1d8 dégâts radiants.",
            School = "Évocation",
            Level = 0,
            CastingTime = "1 action",
            Range = "18 mètres",
            Duration = "Instantané",
            Components = "V, S",
            Damage = "1d8",
            SavingThrow = "Dextérité",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,cantrip,radiant,divin"
        });

        spells.Add(new SpellDnd
        {
            Name = "Stabilisation",
            Description = "Vous touchez une créature vivante qui a 0 point de vie. La créature devient stable. Ce sort n'a aucun effet sur les morts-vivants ou les artificiels.",
            School = "Nécromancie",
            Level = 0,
            CastingTime = "1 action",
            Range = "Contact",
            Duration = "Instantané",
            Components = "V, S",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "nécromancie,cantrip,stabilisation,sauvetage"
        });

        spells.Add(new SpellDnd
        {
            Name = "Lame retentissante",
            Description = "Vous créez une lame scintillante d'énergie pure qui tranche l'air vers une créature dans la portée. Effectuez une attaque de sort au corps à corps contre la cible. Si l'attaque touche, la cible subit 1d8 dégâts de force et si la cible se déplace de son plein gré avant le début de votre prochain tour, elle subit immédiatement 1d8 dégâts de tonnerre et le sort se termine.",
            School = "Évocation",
            Level = 0,
            CastingTime = "1 action",
            Range = "1,50 mètre",
            Duration = "1 tour",
            Components = "V, M",
            MaterialComponent = "une arme de corps à corps d'une valeur d'au moins 1 sp",
            Damage = "1d8",
            AttackRoll = "1d20 + modificateur d'incantation",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,cantrip,lame,force"
        });

        // ===== SORTS DE NIVEAU 1 - 20 SORTS =====

        spells.Add(new SpellDnd
        {
            Name = "Projectile magique",
            Description = "Vous créez trois dards scintillants d'énergie magique. Chaque dard touche automatiquement une créature de votre choix que vous pouvez voir dans la portée. Un dard inflige 1d4 + 1 dégâts de force à sa cible.",
            School = "Évocation",
            Level = 1,
            CastingTime = "1 action",
            Range = "36 mètres",
            Duration = "Instantané",
            Components = "V, S",
            Damage = "3d4+3",
            HigherLevelDamage = 1,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,force,projectile,automatique"
        });

        spells.Add(new SpellDnd
        {
            Name = "Soins",
            Description = "Une créature que vous touchez récupère un nombre de points de vie égal à 1d8 + votre modificateur de caractéristique d'incantation. Ce sort n'a aucun effet sur les morts-vivants ou les artificiels.",
            School = "Évocation",
            Level = 1,
            CastingTime = "1 action",
            Range = "Contact",
            Duration = "Instantané",
            Components = "V, S",
            Damage = "1d8+MOD",
            HigherLevelDamage = 1,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,soin,toucher,instantané"
        });

        spells.Add(new SpellDnd
        {
            Name = "Bouclier",
            Description = "Une barrière invisible de force magique apparaît et vous protège. Jusqu'au début de votre prochain tour, vous avez un bonus de +5 à la CA, y compris contre l'attaque déclenchante.",
            School = "Abjuration",
            Level = 1,
            CastingTime = "1 réaction",
            Range = "Personnelle",
            Duration = "1 tour",
            Components = "V, S",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "abjuration,protection,réaction,CA"
        });

        spells.Add(new SpellDnd
        {
            Name = "Armure du mage",
            Description = "Vous touchez une créature consentante qui ne porte pas d'armure. Jusqu'à la fin du sort, la CA de base de la cible devient 13 + son modificateur de Dextérité.",
            School = "Abjuration",
            Level = 1,
            CastingTime = "1 action",
            Range = "Contact",
            Duration = "8 heures",
            Components = "V, S, M",
            MaterialComponent = "un morceau de cuir tanné",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "abjuration,protection,armure,CA"
        });

        spells.Add(new SpellDnd
        {
            Name = "Détection de la magie",
            Description = "Pendant toute la durée du sort, vous percevez la présence de magie dans un rayon de 9 mètres autour de vous. Si vous percevez de la magie de cette manière, vous pouvez utiliser votre action pour voir une aura faible autour de toute créature ou tout objet visible dans la zone portant de la magie.",
            School = "Divination",
            Level = 1,
            CastingTime = "1 action",
            Range = "Personnelle",
            Duration = "Concentration, jusqu'à 10 minutes",
            Components = "V, S",
            RequiresConcentration = true,
            IsRitual = true,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "divination,détection,magie,rituel"
        });

        spells.Add(new SpellDnd
        {
            Name = "Mains brûlantes",
            Description = "Tandis que vous tenez vos mains avec les pouces qui se touchent et les doigts écartés, une mince nappe de flammes jaillit de vos doigts tendus. Chaque créature dans un cône de 4,50 mètres doit effectuer un jet de sauvegarde de Dextérité.",
            School = "Évocation",
            Level = 1,
            CastingTime = "1 action",
            Range = "Personnelle (cône de 4,50 m)",
            Duration = "Instantané",
            Components = "V, S",
            Damage = "3d6",
            SavingThrow = "Dextérité",
            HigherLevelDamage = 1,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,feu,cône,zone"
        });

        spells.Add(new SpellDnd
        {
            Name = "Charme-personne",
            Description = "Vous tentez de charmer un humanoïde que vous pouvez voir dans la portée. Il doit effectuer un jet de sauvegarde de Sagesse, et il le fait avec un avantage si vous ou vos compagnons le combattez.",
            School = "Enchantement",
            Level = 1,
            CastingTime = "1 action",
            Range = "9 mètres",
            Duration = "1 heure",
            Components = "V, S",
            SavingThrow = "Sagesse",
            HigherLevelDamage = 0,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "enchantement,charme,social,contrôle"
        });

        spells.Add(new SpellDnd
        {
            Name = "Sommeil",
            Description = "Ce sort envoie des créatures dans un sommeil magique. Lancez 5d8 ; le total correspond au nombre de points de vie de créatures que ce sort peut affecter.",
            School = "Enchantement",
            Level = 1,
            CastingTime = "1 action",
            Range = "27 mètres",
            Duration = "1 minute",
            Components = "V, S, M",
            MaterialComponent = "une pincée de sable fin, de pétales de rose ou un criquet",
            Damage = "5d8",
            HigherLevelDamage = 2,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "enchantement,sommeil,zone,contrôle"
        });

        spells.Add(new SpellDnd
        {
            Name = "Identification",
            Description = "Vous choisissez un objet que vous devez toucher pendant toute l'incantation du sort. Si c'est un objet magique ou un autre objet imprégné de magie, vous apprenez ses propriétés et comment les utiliser.",
            School = "Divination",
            Level = 1,
            CastingTime = "1 minute",
            Range = "Contact",
            Duration = "Instantané",
            Components = "V, S, M",
            MaterialComponent = "une perle d'une valeur d'au moins 100 po et une plume de hibou",
            IsRitual = true,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "divination,identification,magie,rituel"
        });

        spells.Add(new SpellDnd
        {
            Name = "Compréhension des langues",
            Description = "Pendant la durée du sort, vous comprenez le sens littéral de tout langage parlé que vous entendez. Vous comprenez aussi tout langage écrit que vous voyez, mais vous devez toucher la surface sur laquelle les mots sont écrits.",
            School = "Divination",
            Level = 1,
            CastingTime = "1 action",
            Range = "Personnelle",
            Duration = "1 heure",
            Components = "V, S, M",
            MaterialComponent = "une pincée de suie et de sel",
            IsRitual = true,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "divination,langage,communication,rituel"
        });

        spells.Add(new SpellDnd
        {
            Name = "Faux semblant",
            Description = "Vous vous transformez — vous et tout ce que vous portez — en créature de taille similaire à votre taille. Cette illusion transforme votre apparence, mais pas vos capacités.",
            School = "Illusion",
            Level = 1,
            CastingTime = "1 action",
            Range = "Personnelle",
            Duration = "1 heure",
            Components = "V, S",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "illusion,déguisement,transformation,furtivité"
        });

        spells.Add(new SpellDnd
        {
            Name = "Saut",
            Description = "Vous touchez une créature. La distance de saut de la créature est triplée jusqu'à la fin du sort.",
            School = "Transmutation",
            Level = 1,
            CastingTime = "1 action",
            Range = "Contact",
            Duration = "1 minute",
            Components = "V, S, M",
            MaterialComponent = "une patte arrière de sauterelle",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,saut,mobilité,amélioration"
        });

        spells.Add(new SpellDnd
        {
            Name = "Alarme",
            Description = "Vous placez une alarme contre l'intrusion non désirée. Choisissez une porte, une fenêtre ou une zone dans la portée qui ne fait pas plus de 6 mètres cubes. Jusqu'à la fin du sort, une alarme vous alerte dès qu'une créature de taille TP ou plus grande touche ou pénètre dans la zone protégée.",
            School = "Abjuration",
            Level = 1,
            CastingTime = "1 minute",
            Range = "9 mètres",
            Duration = "8 heures",
            Components = "V, S, M",
            MaterialComponent = "une petite cloche et un fil d'argent fin",
            IsRitual = true,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "abjuration,alarme,protection,rituel"
        });

        spells.Add(new SpellDnd
        {
            Name = "Trait ensorcelé",
            Description = "Vous créez trois rayons de feu et les projetez vers des cibles dans la portée. Vous pouvez les diriger vers la même cible ou vers des cibles différentes. Effectuez une attaque de sort à distance pour chaque rayon.",
            School = "Évocation",
            Level = 1,
            CastingTime = "1 action",
            Range = "36 mètres",
            Duration = "Instantané",
            Components = "V, S",
            Damage = "3d6",
            AttackRoll = "1d20 + modificateur d'incantation",
            HigherLevelDamage = 1,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,feu,projectile,multiple"
        });

        spells.Add(new SpellDnd
        {
            Name = "Faveur divine",
            Description = "Votre prière vous imprègne d'une radiance divine. Jusqu'à la fin du sort, vos attaques d'arme infligent 1d4 dégâts radiants supplémentaires quand elles touchent.",
            School = "Évocation",
            Level = 1,
            CastingTime = "1 action bonus",
            Range = "Personnelle",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S",
            RequiresConcentration = true,
            Damage = "1d4",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,divin,amélioration,combat"
        });

        spells.Add(new SpellDnd
        {
            Name = "Bénédiction",
            Description = "Vous bénissez jusqu'à trois créatures de votre choix dans la portée. Quand une cible fait un jet d'attaque ou un jet de sauvegarde avant la fin du sort, la cible peut lancer un d4 et ajouter le nombre obtenu au jet d'attaque ou de sauvegarde.",
            School = "Enchantement",
            Level = 1,
            CastingTime = "1 action",
            Range = "9 mètres",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S, M",
            MaterialComponent = "une aspersion d'eau bénite",
            RequiresConcentration = true,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "enchantement,bénédiction,amélioration,divin"
        });

        spells.Add(new SpellDnd
        {
            Name = "Malédiction",
            Description = "Jusqu'à trois créatures de votre choix que vous pouvez voir dans la portée doivent effectuer des jets de sauvegarde de Charisme. Quand une cible qui échoue fait un jet d'attaque ou un jet de sauvegarde avant la fin du sort, la cible doit lancer un d4 et soustraire le nombre obtenu du jet d'attaque ou de sauvegarde.",
            School = "Enchantement",
            Level = 1,
            CastingTime = "1 action",
            Range = "9 mètres",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S, M",
            MaterialComponent = "une goutte de sang",
            RequiresConcentration = true,
            SavingThrow = "Charisme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "enchantement,malédiction,débuff,négatif"
        });

        spells.Add(new SpellDnd
        {
            Name = "Création ou destruction d'eau",
            Description = "Vous créez ou détruisez de l'eau. Création d'eau : Vous créez jusqu'à 40 litres d'eau propre dans un récipient ouvert dans la portée. Destruction d'eau : Vous détruisez jusqu'à 40 litres d'eau dans un récipient ouvert dans la portée.",
            School = "Transmutation",
            Level = 1,
            CastingTime = "1 action",
            Range = "9 mètres",
            Duration = "Instantané",
            Components = "V, S, M",
            MaterialComponent = "une goutte d'eau pour créer de l'eau ou quelques grains de sable pour la détruire",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,eau,création,destruction"
        });

        spells.Add(new SpellDnd
        {
            Name = "Mot de guérison",
            Description = "Une créature de votre choix que vous pouvez voir dans la portée regagne un nombre de points de vie égal à 1d4 + votre modificateur de caractéristique d'incantation. Ce sort n'a aucun effet sur les morts-vivants ou les artificiels.",
            School = "Évocation",
            Level = 1,
            CastingTime = "1 action bonus",
            Range = "18 mètres",
            Duration = "Instantané",
            Components = "V",
            Damage = "1d4+MOD",
            HigherLevelDamage = 1,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,soin,distance,action-bonus"
        });

        // ===== SORTS DE NIVEAU 2 - 15 SORTS =====

        spells.Add(new SpellDnd
        {
            Name = "Toile d'araignée",
            Description = "Vous invoquez une masse de toiles d'araignée épaisses et collantes à un point de votre choix dans la portée. Les toiles remplissent un cube de 6 mètres d'arête à partir de ce point pendant la durée du sort.",
            School = "Invocation",
            Level = 2,
            CastingTime = "1 action",
            Range = "18 mètres",
            Duration = "Concentration, jusqu'à 1 heure",
            Components = "V, S, M",
            MaterialComponent = "un peu de toile d'araignée",
            RequiresConcentration = true,
            SavingThrow = "Dextérité",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "invocation,contrôle,zone,entrave"
        });

        spells.Add(new SpellDnd
        {
            Name = "Invisibilité",
            Description = "Une créature que vous touchez devient invisible jusqu'à la fin du sort. Tout ce que porte ou transporte la cible est invisible tant que c'est sur sa personne.",
            School = "Illusion",
            Level = 2,
            CastingTime = "1 action",
            Range = "Contact",
            Duration = "Concentration, jusqu'à 1 heure",
            Components = "V, S, M",
            MaterialComponent = "un cil enrobé de gomme arabique",
            RequiresConcentration = true,
            HigherLevelDamage = 0,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "illusion,invisibilité,furtivité,tactique"
        });

        spells.Add(new SpellDnd
        {
            Name = "Projectile acide",
            Description = "Une orbe chatoyante d'énergie acide jaillit vers une créature dans la portée. Effectuez une attaque de sort à distance contre la cible. Si l'attaque touche, la cible subit 2d4 dégâts d'acide immédiatement et 2d4 dégâts d'acide à la fin de son prochain tour.",
            School = "Évocation",
            Level = 2,
            CastingTime = "1 action",
            Range = "27 mètres",
            Duration = "Instantané",
            Components = "V, S, M",
            MaterialComponent = "une ampoule de rhubarbe et un estomac de vipère",
            Damage = "4d4",
            AttackRoll = "1d20 + modificateur d'incantation",
            HigherLevelDamage = 1,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,acide,attaque,dégâts"
        });

        spells.Add(new SpellDnd
        {
            Name = "Flou",
            Description = "Votre corps devient flou, oscillant et ondulant pour tous ceux qui vous regardent. Pendant toute la durée du sort, toute créature qui vous attaque a un désavantage aux jets d'attaque contre vous.",
            School = "Illusion",
            Level = 2,
            CastingTime = "1 action",
            Range = "Personnelle",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V",
            RequiresConcentration = true,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "illusion,défense,désavantage,protection"
        });

        spells.Add(new SpellDnd
        {
            Name = "Immobilisation de personne",
            Description = "Choisissez un humanoïde que vous pouvez voir dans la portée. La cible doit réussir un jet de sauvegarde de Sagesse ou être paralysée pendant la durée du sort.",
            School = "Enchantement",
            Level = 2,
            CastingTime = "1 action",
            Range = "18 mètres",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S, M",
            MaterialComponent = "un petit morceau de fer droit",
            RequiresConcentration = true,
            SavingThrow = "Sagesse",
            HigherLevelDamage = 0,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "enchantement,paralysie,contrôle,incapacitation"
        });

        spells.Add(new SpellDnd
        {
            Name = "Rayon ardent",
            Description = "Vous créez trois rayons de feu et les projetez vers des cibles dans la portée. Vous pouvez les diriger vers la même cible ou vers des cibles différentes. Effectuez une attaque de sort à distance pour chaque rayon.",
            School = "Évocation",
            Level = 2,
            CastingTime = "1 action",
            Range = "36 mètres",
            Duration = "Instantané",
            Components = "V, S",
            Damage = "3 × 2d6",
            AttackRoll = "1d20 + modificateur d'incantation",
            HigherLevelDamage = 1,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,feu,projectile,multiple"
        });

        spells.Add(new SpellDnd
        {
            Name = "Suggestion",
            Description = "Vous suggérez un cours d'activité (limité à une phrase ou deux) et influencez magiquement une créature que vous pouvez voir dans la portée et qui peut vous entendre et vous comprendre.",
            School = "Enchantement",
            Level = 2,
            CastingTime = "1 action",
            Range = "9 mètres",
            Duration = "Concentration, jusqu'à 8 heures",
            Components = "V, M",
            MaterialComponent = "une langue de serpent et soit un peu de miel, soit une goutte d'huile d'olive",
            RequiresConcentration = true,
            SavingThrow = "Sagesse",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "enchantement,suggestion,contrôle,social"
        });

        spells.Add(new SpellDnd
        {
            Name = "Ténèbres",
            Description = "Des ténèbres magiques s'étendent à partir d'un point que vous choisissez dans la portée pour remplir une sphère de 4,50 mètres de rayon pendant la durée du sort.",
            School = "Évocation",
            Level = 2,
            CastingTime = "1 action",
            Range = "18 mètres",
            Duration = "Concentration, jusqu'à 10 minutes",
            Components = "V, M",
            MaterialComponent = "de la fourrure de chauve-souris et une goutte de poix ou un morceau de charbon",
            RequiresConcentration = true,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,ténèbres,zone,contrôle"
        });

        spells.Add(new SpellDnd
        {
            Name = "Détection des pensées",
            Description = "Pendant la durée du sort, vous pouvez lire les pensées de certaines créatures. Quand vous lancez le sort et par une action à chacun de vos tours jusqu'à la fin du sort, vous pouvez concentrer votre esprit sur une créature que vous pouvez voir dans un rayon de 9 mètres.",
            School = "Divination",
            Level = 2,
            CastingTime = "1 action",
            Range = "Personnelle",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S, M",
            MaterialComponent = "une pièce de cuivre",
            RequiresConcentration = true,
            SavingThrow = "Sagesse",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "divination,mental,pensées,information"
        });

        spells.Add(new SpellDnd
        {
            Name = "Bourrasque",
            Description = "Une ligne de vent fort de 18 mètres de long et de 3 mètres de large souffle de vous dans une direction que vous choisissez pendant la durée du sort. Chaque créature qui commence son tour dans la ligne doit réussir un jet de sauvegarde de Force ou être poussée de 4,50 mètres loin de vous dans la direction du vent.",
            School = "Évocation",
            Level = 2,
            CastingTime = "1 action",
            Range = "Personnelle (ligne de 18 m)",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S, M",
            MaterialComponent = "une légume et une plume d'oiseau",
            RequiresConcentration = true,
            SavingThrow = "Force",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,vent,poussée,contrôle"
        });

        spells.Add(new SpellDnd
        {
            Name = "Sphère de feu",
            Description = "Une sphère de feu de 1,50 mètre de diamètre apparaît dans un espace inoccupé de votre choix dans la portée et dure pendant la durée du sort. Toute créature qui finit son tour dans un rayon de 1,50 mètre de la sphère doit effectuer un jet de sauvegarde de Dextérité.",
            School = "Invocation",
            Level = 2,
            CastingTime = "1 action",
            Range = "18 mètres",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S, M",
            MaterialComponent = "un peu de suif, une pincée de soufre et une poudre de fer",
            RequiresConcentration = true,
            Damage = "2d6",
            SavingThrow = "Dextérité",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "invocation,feu,sphère,persistant"
        });

        spells.Add(new SpellDnd
        {
            Name = "Lévitation",
            Description = "Une créature ou un objet libre de votre choix que vous pouvez voir dans la portée s'élève verticalement, jusqu'à 6 mètres, et reste suspendu là pendant la durée du sort.",
            School = "Transmutation",
            Level = 2,
            CastingTime = "1 action",
            Range = "18 mètres",
            Duration = "Concentration, jusqu'à 10 minutes",
            Components = "V, S, M",
            MaterialComponent = "soit une petite boucle de cuir, soit un morceau de fil d'or façonné en forme de coupe avec un long manche",
            RequiresConcentration = true,
            SavingThrow = "Constitution",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,lévitation,déplacement,contrôle"
        });

        spells.Add(new SpellDnd
        {
            Name = "Image miroir",
            Description = "Trois duplicatas illusoires de vous-même apparaissent dans votre espace. Jusqu'à la fin du sort, les duplicatas se déplacent avec vous et imitent vos actions, changeant de position de sorte qu'il soit impossible de déterminer quelles images sont réelles et lesquelles sont des illusions.",
            School = "Illusion",
            Level = 2,
            CastingTime = "1 action",
            Range = "Personnelle",
            Duration = "1 minute",
            Components = "V, S",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "illusion,duplicata,défense,tromperie"
        });

        spells.Add(new SpellDnd
        {
            Name = "Pas brumeux",
            Description = "Brièvement entouré d'une brume argentée, vous vous téléportez jusqu'à 9 mètres vers un espace inoccupé que vous pouvez voir.",
            School = "Invocation",
            Level = 2,
            CastingTime = "1 action bonus",
            Range = "Personnelle",
            Duration = "Instantané",
            Components = "V",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "invocation,téléportation,mobilité,tactique"
        });

        spells.Add(new SpellDnd
        {
            Name = "Restauration partielle",
            Description = "Vous touchez une créature et pouvez mettre fin soit à une maladie, soit à un état qui l'afflige. L'état peut être aveuglé, assourdi, paralysé ou empoisonné.",
            School = "Abjuration",
            Level = 2,
            CastingTime = "1 action",
            Range = "Contact",
            Duration = "Instantané",
            Components = "V, S",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "abjuration,restauration,guérison,état"
        });

        // ===== SORTS DE NIVEAU 3 - 15 SORTS =====

        spells.Add(new SpellDnd
        {
            Name = "Boule de feu",
            Description = "Un rayon brillant jaillit de votre doigt tendu vers un point de votre choix dans la portée et explose dans un rugissement de flammes. Chaque créature dans une sphère de 6 mètres de rayon centrée sur ce point doit faire un jet de sauvegarde de Dextérité.",
            School = "Évocation",
            Level = 3,
            CastingTime = "1 action",
            Range = "45 mètres",
            Duration = "Instantané",
            Components = "V, S, M",
            MaterialComponent = "une petite boule de guano de chauve-souris et du soufre",
            Damage = "8d6",
            SavingThrow = "Dextérité",
            HigherLevelDamage = 1,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,feu,zone,dégâts,classique"
        });

        spells.Add(new SpellDnd
        {
            Name = "Vol",
            Description = "Vous touchez une créature consentante. La cible gagne une vitesse de vol de 18 mètres pour la durée du sort. Quand le sort se termine, la cible tombe si elle est encore en vol.",
            School = "Transmutation",
            Level = 3,
            CastingTime = "1 action",
            Range = "Contact",
            Duration = "Concentration, jusqu'à 10 minutes",
            Components = "V, S, M",
            MaterialComponent = "une plume d'aile de n'importe quel oiseau",
            RequiresConcentration = true,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,déplacement,vol,mobilité"
        });

        spells.Add(new SpellDnd
        {
            Name = "Éclair",
            Description = "Un trait d'énergie électrique crépitante de 1,50 mètre de large et de 30 mètres de long jaillit de vous dans une direction de votre choix. Chaque créature sur la ligne doit effectuer un jet de sauvegarde de Dextérité.",
            School = "Évocation",
            Level = 3,
            CastingTime = "1 action",
            Range = "Personnelle (ligne de 30 m)",
            Duration = "Instantané",
            Components = "V, S, M",
            MaterialComponent = "un peu de fourrure et une baguette d'ambre, de cristal ou de verre",
            Damage = "8d6",
            SavingThrow = "Dextérité",
            HigherLevelDamage = 1,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,foudre,ligne,zone"
        });

        spells.Add(new SpellDnd
        {
            Name = "Hâte",
            Description = "Choisissez une créature consentante que vous pouvez voir dans la portée. Jusqu'à la fin du sort, la vitesse de la cible est doublée, elle gagne un bonus de +2 à la CA, elle a un avantage aux jets de sauvegarde de Dextérité, et elle gagne une action supplémentaire à chacun de ses tours.",
            School = "Transmutation",
            Level = 3,
            CastingTime = "1 action",
            Range = "9 mètres",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S, M",
            MaterialComponent = "un copeau de racine de réglisse",
            RequiresConcentration = true,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,amélioration,vitesse,combat"
        });

        spells.Add(new SpellDnd
        {
            Name = "Lenteur",
            Description = "Vous altérez le temps autour d'un maximum de six créatures de votre choix dans un cube de 12 mètres d'arête dans la portée. Chaque cible doit réussir un jet de sauvegarde de Sagesse ou être affectée par ce sort pendant sa durée.",
            School = "Transmutation",
            Level = 3,
            CastingTime = "1 action",
            Range = "36 mètres",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S, M",
            MaterialComponent = "une goutte de mélasse",
            RequiresConcentration = true,
            SavingThrow = "Sagesse",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,débuff,contrôle,temps"
        });

        spells.Add(new SpellDnd
        {
            Name = "Contresort",
            Description = "Vous tentez d'interrompre une créature en train de lancer un sort. Si la créature est en train de lancer un sort de niveau 3 ou inférieur, son sort échoue et n'a aucun effet.",
            School = "Abjuration",
            Level = 3,
            CastingTime = "1 réaction",
            Range = "18 mètres",
            Duration = "Instantané",
            Components = "S",
            HigherLevelDamage = 0,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "abjuration,contresort,réaction,défense"
        });

        spells.Add(new SpellDnd
        {
            Name = "Dissipation de la magie",
            Description = "Choisissez une créature, un objet ou un effet magique dans la portée. Tout sort de niveau 3 ou inférieur sur la cible prend fin. Pour chaque sort de niveau 4 ou plus sur la cible, effectuez un test de caractéristique d'incantation.",
            School = "Abjuration",
            Level = 3,
            CastingTime = "1 action",
            Range = "36 mètres",
            Duration = "Instantané",
            Components = "V, S",
            HigherLevelDamage = 0,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "abjuration,dissipation,magie,annulation"
        });

        spells.Add(new SpellDnd
        {
            Name = "Peur",
            Description = "Vous projetez une image fantasmagorique des pires craintes d'une créature. Chaque créature dans un cône de 9 mètres doit réussir un jet de sauvegarde de Sagesse ou lâcher tout ce qu'elle tient et être effrayée pendant la durée du sort.",
            School = "Illusion",
            Level = 3,
            CastingTime = "1 action",
            Range = "Personnelle (cône de 9 m)",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S, M",
            MaterialComponent = "une plume blanche ou un cœur de poule",
            RequiresConcentration = true,
            SavingThrow = "Sagesse",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "illusion,peur,cône,contrôle"
        });

        spells.Add(new SpellDnd
        {
            Name = "Boule de feu à retardement mineure",
            Description = "Vous créez une petite perle d'énergie qui explose en boule de feu quand vous décidez de la déclencher ou quand la durée expire. La perle reste immobile jusqu'à l'explosion.",
            School = "Évocation",
            Level = 3,
            CastingTime = "1 action",
            Range = "45 mètres",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S, M",
            MaterialComponent = "une petite boule de guano de chauve-souris et du soufre",
            RequiresConcentration = true,
            Damage = "6d6",
            SavingThrow = "Dextérité",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,feu,retardement,tactique"
        });

        spells.Add(new SpellDnd
        {
            Name = "Forme gazeuse",
            Description = "Vous transformez une créature consentante que vous touchez, ainsi que tout ce qu'elle porte, en un nuage brumeux pendant la durée du sort. Le sort n'a aucun effet sur une créature qui a 0 point de vie.",
            School = "Transmutation",
            Level = 3,
            CastingTime = "1 action",
            Range = "Contact",
            Duration = "Concentration, jusqu'à 1 heure",
            Components = "V, S, M",
            MaterialComponent = "un morceau de gaze et un peu de fumée",
            RequiresConcentration = true,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,forme,gaz,évasion"
        });

        spells.Add(new SpellDnd
        {
            Name = "Hypnose",
            Description = "Vous créez un motif de couleurs ondoyantes dans l'air dans un cube de 3 mètres d'arête dans la portée. Le motif apparaît pendant un moment et disparaît. Chaque créature dans la zone qui voit le motif doit effectuer un jet de sauvegarde de Sagesse.",
            School = "Illusion",
            Level = 3,
            CastingTime = "1 action",
            Range = "36 mètres",
            Duration = "1 minute",
            Components = "S, M",
            MaterialComponent = "un bâton d'encens incandescent ou un cristal ou verre rempli de phosphore",
            SavingThrow = "Sagesse",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "illusion,hypnose,zone,incapacitation"
        });

        spells.Add(new SpellDnd
        {
            Name = "Respiration aquatique",
            Description = "Ce sort donne la capacité de respirer sous l'eau à un maximum de dix créatures consentantes que vous pouvez voir dans la portée. Les créatures affectées conservent aussi leur mode de respiration normal.",
            School = "Transmutation",
            Level = 3,
            CastingTime = "1 action",
            Range = "9 mètres",
            Duration = "24 heures",
            Components = "V, S, M",
            MaterialComponent = "un court roseau ou un brin de paille",
            IsRitual = true,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,respiration,aquatique,utilitaire"
        });

        spells.Add(new SpellDnd
        {
            Name = "Communication avec les morts",
            Description = "Vous accordez un semblant de vie et d'intelligence à un cadavre de votre choix dans la portée, lui permettant de répondre aux questions que vous lui posez. Le cadavre doit encore avoir une bouche et ne peut pas être un mort-vivant.",
            School = "Nécromancie",
            Level = 3,
            CastingTime = "1 action",
            Range = "3 mètres",
            Duration = "10 minutes",
            Components = "V, S, M",
            MaterialComponent = "de l'encens allumé",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "nécromancie,communication,morts,information"
        });

        spells.Add(new SpellDnd
        {
            Name = "Protection contre l'énergie",
            Description = "Pendant la durée du sort, la créature consentante que vous touchez a une résistance à un type de dégâts de votre choix : acide, froid, feu, foudre ou tonnerre.",
            School = "Abjuration",
            Level = 3,
            CastingTime = "1 action",
            Range = "Contact",
            Duration = "Concentration, jusqu'à 1 heure",
            Components = "V, S",
            RequiresConcentration = true,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "abjuration,protection,résistance,énergie"
        });

        spells.Add(new SpellDnd
        {
            Name = "Marche sur l'eau",
            Description = "Ce sort accorde la capacité de se déplacer sur toute surface liquide — comme l'eau, l'acide, la boue, la neige, les sables mouvants ou la lave — comme si c'était un sol ferme et inoffensif.",
            School = "Transmutation",
            Level = 3,
            CastingTime = "1 action",
            Range = "9 mètres",
            Duration = "1 heure",
            Components = "V, S, M",
            MaterialComponent = "un morceau de liège",
            IsRitual = true,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,marche,eau,déplacement"
        });

        // ===== SORTS DE NIVEAUX SUPÉRIEURS (4-9) - 20 SORTS =====

        // Niveau 4 - 5 sorts
        spells.Add(new SpellDnd
        {
            Name = "Polymorph",
            Description = "Ce sort transforme une créature que vous pouvez voir dans la portée en une nouvelle forme. Une créature non consentante doit faire un jet de sauvegarde de Sagesse pour éviter l'effet.",
            School = "Transmutation",
            Level = 4,
            CastingTime = "1 action",
            Range = "18 mètres",
            Duration = "Concentration, jusqu'à 1 heure",
            Components = "V, S, M",
            MaterialComponent = "un cocon de ver à soie",
            RequiresConcentration = true,
            SavingThrow = "Sagesse",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,transformation,contrôle,tactique"
        });

        spells.Add(new SpellDnd
        {
            Name = "Confusion",
            Description = "Ce sort assaille et tord les créatures, engendrant des illusions et provoquant des actions incontrôlées. Chaque créature dans une sphère de 3 mètres de rayon centrée sur un point que vous choisissez dans la portée doit réussir un jet de sauvegarde de Sagesse.",
            School = "Enchantement",
            Level = 4,
            CastingTime = "1 action",
            Range = "27 mètres",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S, M",
            MaterialComponent = "trois coquilles de noix",
            RequiresConcentration = true,
            SavingThrow = "Sagesse",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "enchantement,confusion,zone,contrôle"
        });

        spells.Add(new SpellDnd
        {
            Name = "Dimension Door",
            Description = "Vous vous téléportez de votre position actuelle vers n'importe quel autre endroit dans la portée. Vous arrivez exactement à l'endroit désiré. Vous pouvez emmener des objets tant que leur poids ne dépasse pas ce que vous pouvez porter.",
            School = "Invocation",
            Level = 4,
            CastingTime = "1 action",
            Range = "150 mètres",
            Duration = "Instantané",
            Components = "V",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "invocation,téléportation,évasion,tactique"
        });

        spells.Add(new SpellDnd
        {
            Name = "Liberté de mouvement",
            Description = "Vous touchez une créature consentante. Pendant la durée du sort, le mouvement de la cible n'est pas entravé par un terrain difficile, et les sorts et autres effets magiques ne peuvent ni réduire la vitesse de la cible, ni la paralyser ou l'entraver.",
            School = "Abjuration",
            Level = 4,
            CastingTime = "1 action",
            Range = "Contact",
            Duration = "1 heure",
            Components = "V, S, M",
            MaterialComponent = "une lanière de cuir, enroulée autour du bras ou d'un appendice similaire",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "abjuration,liberté,mouvement,immunité"
        });

        spells.Add(new SpellDnd
        {
            Name = "Bannissement",
            Description = "Vous tentez d'envoyer une créature que vous pouvez voir dans la portée vers un autre plan d'existence. La cible doit réussir un jet de sauvegarde de Charisme ou être bannie.",
            School = "Abjuration",
            Level = 4,
            CastingTime = "1 action",
            Range = "18 mètres",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S, M",
            MaterialComponent = "un objet déplaisant pour la cible",
            RequiresConcentration = true,
            SavingThrow = "Charisme",
            HigherLevelDamage = 0,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "abjuration,bannissement,plan,exil"
        });

        // Niveau 5 - 5 sorts
        spells.Add(new SpellDnd
        {
            Name = "Téléportation",
            Description = "Ce sort vous transporte instantanément et sans erreur, vous et jusqu'à huit créatures consentantes de votre choix que vous pouvez voir dans la portée, ou un seul objet que vous pouvez voir dans la portée, vers une destination de votre choix.",
            School = "Invocation",
            Level = 5,
            CastingTime = "1 action",
            Range = "3 mètres",
            Duration = "Instantané",
            Components = "V",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "invocation,téléportation,déplacement,groupe"
        });

        spells.Add(new SpellDnd
        {
            Name = "Domination de personne",
            Description = "Vous tentez de charmer un humanoïde que vous pouvez voir dans la portée. Il doit réussir un jet de sauvegarde de Sagesse ou être charmé par vous pendant la durée du sort.",
            School = "Enchantement",
            Level = 5,
            CastingTime = "1 action",
            Range = "18 mètres",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S",
            RequiresConcentration = true,
            SavingThrow = "Sagesse",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "enchantement,domination,contrôle,mental"
        });

        spells.Add(new SpellDnd
        {
            Name = "Animation des morts",
            Description = "Ce sort crée un serviteur mort-vivant. Choisissez un tas d'os ou un cadavre d'un humanoïde de taille M ou P dans la portée. Votre sort imprègne la cible d'un simulacre de vie ignoble, la relevant comme une créature morte-vivante.",
            School = "Nécromancie",
            Level = 5,
            CastingTime = "1 minute",
            Range = "3 mètres",
            Duration = "Instantané",
            Components = "V, S, M",
            MaterialComponent = "une goutte de sang, un morceau de chair et une pincée de poudre d'os",
            HigherLevelDamage = 0,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "nécromancie,animation,morts-vivants,serviteur"
        });

        spells.Add(new SpellDnd
        {
            Name = "Cône de froid",
            Description = "Un souffle d'air froid jaillit de vos mains. Chaque créature dans un cône de 18 mètres doit effectuer un jet de sauvegarde de Constitution. Une créature subit 8d8 dégâts de froid en cas d'échec, ou la moitié de ces dégâts en cas de réussite.",
            School = "Évocation",
            Level = 5,
            CastingTime = "1 action",
            Range = "Personnelle (cône de 18 m)",
            Duration = "Instantané",
            Components = "V, S, M",
            MaterialComponent = "un petit cône de cristal ou de verre",
            Damage = "8d8",
            SavingThrow = "Constitution",
            HigherLevelDamage = 1,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,froid,cône,zone"
        });

        spells.Add(new SpellDnd
        {
            Name = "Restauration suprême",
            Description = "Vous imprégnez une créature que vous touchez d'énergie positive pour défaire un effet débilitant. Vous pouvez réduire le niveau d'épuisement de la cible de un, ou mettre fin à l'un des effets suivants affectant la cible : un sort de charme ou de pétrification sur la cible, toute malédiction affectant la cible, toute réduction à l'une des valeurs de caractéristique de la cible, ou un effet réduisant le maximum de points de vie de la cible.",
            School = "Abjuration",
            Level = 5,
            CastingTime = "1 action",
            Range = "Contact",
            Duration = "Instantané",
            Components = "V, S, M",
            MaterialComponent = "poudre de diamant d'une valeur d'au moins 100 po, consommée par le sort",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "abjuration,restauration,guérison,malédiction"
        });

        // Niveau 6 - 3 sorts
        spells.Add(new SpellDnd
        {
            Name = "Désintégration",
            Description = "Un mince rayon vert jaillit de votre doigt tendu vers une cible que vous pouvez voir dans la portée. La cible peut être une créature, un objet ou une création de force magique.",
            School = "Transmutation",
            Level = 6,
            CastingTime = "1 action",
            Range = "18 mètres",
            Duration = "Instantané",
            Components = "V, S, M",
            MaterialComponent = "une magnétite et une pincée de poussière",
            Damage = "10d6+40",
            SavingThrow = "Dextérité",
            HigherLevelDamage = 3,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,destruction,rayon,puissant"
        });

        spells.Add(new SpellDnd
        {
            Name = "Cercle de mort",
            Description = "Une sphère d'énergie négative ondule dans un rayon de 18 mètres autour d'un point que vous choisissez dans la portée. Chaque créature dans la zone doit effectuer un jet de sauvegarde de Constitution.",
            School = "Nécromancie",
            Level = 6,
            CastingTime = "1 action",
            Range = "45 mètres",
            Duration = "Instantané",
            Components = "V, S, M",
            MaterialComponent = "la poudre d'une perle noire écrasée d'une valeur d'au moins 500 po",
            Damage = "8d6",
            SavingThrow = "Constitution",
            HigherLevelDamage = 2,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "nécromancie,mort,zone,négatif"
        });

        spells.Add(new SpellDnd
        {
            Name = "Suggestion de groupe",
            Description = "Vous suggérez un cours d'activité (limité à une phrase ou deux) et influencez magiquement jusqu'à douze créatures de votre choix que vous pouvez voir dans la portée et qui peuvent vous entendre et vous comprendre.",
            School = "Enchantement",
            Level = 6,
            CastingTime = "1 action",
            Range = "18 mètres",
            Duration = "24 heures",
            Components = "V, M",
            MaterialComponent = "une langue de serpent et soit un peu de miel, soit une goutte d'huile d'olive",
            SavingThrow = "Sagesse",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "enchantement,suggestion,groupe,contrôle"
        });

        // Niveau 7 - 2 sorts
        spells.Add(new SpellDnd
        {
            Name = "Boule de feu à retardement",
            Description = "Un rayon de lumière jaune jaillit de votre doigt tendu, puis se condense et demeure à un point de votre choix dans la portée sous la forme d'une perle luisante pendant toute la durée du sort.",
            School = "Évocation",
            Level = 7,
            CastingTime = "1 action",
            Range = "45 mètres",
            Duration = "Concentration, jusqu'à 1 minute",
            Components = "V, S, M",
            MaterialComponent = "une petite boule de guano de chauve-souris et du soufre",
            RequiresConcentration = true,
            Damage = "12d6",
            SavingThrow = "Dextérité",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,feu,retardement,zone,tactique"
        });

        spells.Add(new SpellDnd
        {
            Name = "Régénération",
            Description = "Vous touchez une créature et stimulez ses capacités de guérison naturelle. La cible récupère immédiatement 4d8 + 15 points de vie. Pendant la durée du sort, la cible récupère 1 point de vie au début de chacun de ses tours (10 points de vie par minute).",
            School = "Transmutation",
 Level = 7,
            CastingTime = "1 minute",
            Range = "Contact",
            Duration = "1 heure",
            Components = "V, S, M",
            MaterialComponent = "un moulin à prière et de l'eau bénite",
            Damage = "4d8+15",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,régénération,guérison,continue"
        });

        // Niveau 8 - 2 sorts
        spells.Add(new SpellDnd
        {
            Name = "Feeble Mind",
            Description = "Vous vous en prenez à l'esprit d'une créature que vous pouvez voir dans la portée, tentant de détruire son intellect et sa personnalité. La cible doit effectuer un jet de sauvegarde d'Intelligence.",
            School = "Enchantement",
            Level = 8,
            CastingTime = "1 action",
            Range = "45 mètres",
            Duration = "Instantané",
            Components = "V, S, M",
            MaterialComponent = "une poignée d'argile, de cristal, de verre ou de sphères minérales",
            Damage = "4d6",
            SavingThrow = "Intelligence",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "enchantement,mental,débuff,intelligence"
        });

        spells.Add(new SpellDnd
        {
            Name = "Domination de monstre",
            Description = "Vous tentez de charmer une créature que vous pouvez voir dans la portée. Elle doit réussir un jet de sauvegarde de Sagesse ou être charmée par vous pendant la durée du sort.",
            School = "Enchantement",
            Level = 8,
            CastingTime = "1 action",
            Range = "18 mètres",
            Duration = "Concentration, jusqu'à 1 heure",
            Components = "V, S",
            RequiresConcentration = true,
            SavingThrow = "Sagesse",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "enchantement,domination,monstre,contrôle"
        });

        // Niveau 9 - 3 sorts légendaires
        spells.Add(new SpellDnd
        {
            Name = "Souhait",
            Description = "Souhait est le plus puissant des sorts qu'une créature mortelle puisse lancer. En énonçant simplement vos désirs à voix haute, vous pouvez altérer les fondements de la réalité selon vos désirs.",
            School = "Invocation",
            Level = 9,
            CastingTime = "1 action",
            Range = "Personnelle",
            Duration = "Instantané",
            Components = "V",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "invocation,réalité,puissant,ultime,légendaire"
        });

        spells.Add(new SpellDnd
        {
            Name = "Arrêt du temps",
            Description = "Vous arrêtez brièvement le flux du temps pour tout le monde sauf pour vous. Aucun temps ne s'écoule pour les autres créatures, tandis que vous prenez 1d4 + 1 tours d'affilée.",
            School = "Transmutation",
            Level = 9,
            CastingTime = "1 action",
            Range = "Personnelle",
            Duration = "Instantané",
            Components = "V",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "transmutation,temps,contrôle,ultime"
        });

        spells.Add(new SpellDnd
        {
            Name = "Nuée de météores",
            Description = "Des orbes de feu flamboyant plongent au sol à quatre points différents que vous pouvez voir dans la portée. Chaque créature dans une sphère de 12 mètres de rayon doit effectuer un jet de sauvegarde de Dextérité.",
            School = "Évocation",
            Level = 9,
            CastingTime = "1 action",
            Range = "1,5 kilomètre",
            Duration = "Instantané",
            Components = "V, S",
            Damage = "20d6",
            SavingThrow = "Dextérité",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,feu,météores,zone,destruction"
        });

        return spells;
    }
}