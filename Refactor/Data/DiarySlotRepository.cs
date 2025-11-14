using Microsoft.EntityFrameworkCore;

namespace Refactor.Data;

public class DiarySlotRepository(DiaryContext context) : IDiarySlotRepository
{
    public async Task<List<DiarySlot>> GetSlotsForDayAsync(DateOnly day) =>
        await context.DiarySlots
            .Where(x => DateOnly.FromDateTime(x.StartTime) == day)
            .OrderBy(x => x.StartTime)
            .ToListAsync();
}