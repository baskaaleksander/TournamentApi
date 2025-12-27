namespace TournamentApi.Models;

public class Tournament
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public TournamentStatus Status { get; set; }

    public ICollection<Bracket> Brackets { get; set; } = new List<Bracket>();
}

