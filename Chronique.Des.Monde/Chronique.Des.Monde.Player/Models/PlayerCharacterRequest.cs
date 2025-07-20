namespace Chronique.Des.Monde.Player.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public record class PlayerCharacterRequest
{
    public int UserId { get; set; }

    public string Picture { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Background { get; set; } = string.Empty;
    
    public string Class { get; set; } = string.Empty;

    public int Life { get; set; }

    public int Leveling { get; set; }

    public int ClassArmor { get; set; }

    public int Strong { get; set; }

    public int Dexterity { get; set; }

    public int Constitution { get; set; }

    public int Intelligence { get; set; }

    public int Wisdoms { get; set; }

    public int Charism { get; set; }
}
