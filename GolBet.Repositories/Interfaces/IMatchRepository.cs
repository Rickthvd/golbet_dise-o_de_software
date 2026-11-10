// GolBet.Repositories/Interfaces/IMatchRepository.cs
using GolBet.Entities;
using GolBet.Entities.Enums;

namespace GolBet.Repositories.Interfaces;

public interface IMatchRepository : IGenericRepository<Match>
{
    /// <summary>
    /// All matches with both teams loaded, optionally filtered by status
    /// and by a text that must appear in the home or away team name.
    /// </summary>
    Task<IEnumerable<Match>> GetAllWithTeamsAsync(MatchStatus? status = null, string? search = null);

    Task<Match?> GetByIdWithDetailsAsync(int id);
}