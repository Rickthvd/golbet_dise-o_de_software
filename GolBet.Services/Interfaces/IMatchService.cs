// GolBet.Services/Interfaces/IMatchService.cs
using GolBet.Entities.Enums;
using GolBet.Services.DTOs;

namespace GolBet.Services.Interfaces;

public interface IMatchService
{
    /// <summary>
    /// Match board: all active matches ordered by date, optionally filtered
    /// by status and by a text that must appear in a team name.
    /// </summary>
    Task<IEnumerable<MatchDto>> GetBoardAsync(MatchStatus? status = null, string? search = null);

    /// <summary>Full detail for a single match, or null if it doesn't exist.</summary>
    Task<MatchDetailDto?> GetDetailAsync(int id);

    // Module 6: writes
    Task<MatchFormDto?> GetForEditAsync(int id);
    Task CreateAsync(MatchFormDto dto);
    Task UpdateAsync(MatchFormDto dto);
    Task DeactivateAsync(int id);
}