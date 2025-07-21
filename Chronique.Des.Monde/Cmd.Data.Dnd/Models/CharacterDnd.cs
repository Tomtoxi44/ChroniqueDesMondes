using System.ComponentModel.DataAnnotations;
using Chronique.Des.Mondes.Data.Models;

namespace Cmd.Data.Dnd.Models;

public class CharacterDnd : ACharacter
{
    [MaxLength(50)] public string Class { get; set; } = string.Empty;

    [Required] [Range(0, 50)] public int ClassArmor { get; set; }

    [Required] [Range(0, 20)] public int Strong { get; set; }

    public int AdditionalStrong { get; set; }

    [Required] [Range(0, 20)] public int Dexterity { get; set; }

    public int AdditionalDexterity { get; set; }

    [Required] [Range(0, 20)] public int Constitution { get; set; }

    public int AdditionalConstitution { get; set; }

    [Required] [Range(0, 20)] public int Intelligence { get; set; }

    public int AdditionalIntelligence { get; set; }

    [Required] [Range(0, 20)] public int Wisdoms { get; set; }

    public int AdditionalWisdoms { get; set; }

    [Required] [Range(0, 20)] public int Charism { get; set; }

    public int AdditionalCharism { get; set; }
}