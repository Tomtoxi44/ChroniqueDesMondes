namespace Chronique.Des.Mondes.Data.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerCharacter
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    public string Picture { get; set; } =string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Background {  get; set; } = string.Empty;

    [Required]
    [Range(0, 20)]
    public int Leveling { get; set; }

    [Required]
    [Range(0, 50)]
    public int ClassArmor { get; set; }

    [Required]
    [Range(0, 20)]
    public int Strong { get; set; }

    [Required]
    [Range(0, 20)]
    public int Dexterity { get; set; }

    [Required]
    [Range(0, 20)]
    public int Constitution { get; set; }

    [Required]
    [Range(0, 20)]
    public int Intelligence { get; set; }

    [Required]
    [Range(0, 20)]
    public int Wisdoms { get; set; }

    [Required]
    [Range(0, 20)]
    public int Charism { get; set; }

    public Users? Users { get; set; }

}
