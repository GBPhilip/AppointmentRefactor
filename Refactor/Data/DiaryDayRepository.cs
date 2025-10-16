using System;
using System.Threading.Tasks;
public class DiaryDayRepository : IDiaryDayRepository
{
    private readonly DiaryContext _context;
    public DiaryDayRepository(DiaryContext context) => _context = context;
    public Task<DiaryDay?> GetDiaryDayAsync(DateOnly day) =>
        _context.DiaryDays.FindAsync(day).AsTask();
}
