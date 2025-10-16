using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
public class DiarySlotRepository : IDiarySlotRepository
{
    private readonly DiaryContext _context;
    public DiarySlotRepository(DiaryContext context) => _context = context;
    public async Task<List<DiarySlot>> GetSlotsForDayAsync(DateOnly day) =>
        await _context.DiarySlots
            .Where(x => DateOnly.FromDateTime(x.StartTime) == day)
            .OrderBy(x => x.StartTime)
            .ToListAsync();
}
