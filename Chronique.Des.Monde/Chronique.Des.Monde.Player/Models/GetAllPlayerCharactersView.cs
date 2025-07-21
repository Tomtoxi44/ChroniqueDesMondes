namespace Chronique.Des.Mondes.Player.Models;

public record class GetAllPlayerCharactersView
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Picture { get; set; } = string.Empty;

    public int Life {  get; set; }

    public int Leveling { get; set; }

    public string Class {  get; set; } = string.Empty;

}
