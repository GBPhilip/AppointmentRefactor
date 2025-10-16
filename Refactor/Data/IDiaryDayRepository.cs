using System;
using System.Threading.Tasks;
public interface IDiaryDayRepository
{
    Task<DiaryDay?> GetDiaryDayAsync(DateOnly day);
}
