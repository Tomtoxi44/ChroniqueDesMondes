using System.ComponentModel.DataAnnotations;

namespace Chronique.Des.Mondes.Data.Models;

public class Users
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string UserEmail { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty; 
}