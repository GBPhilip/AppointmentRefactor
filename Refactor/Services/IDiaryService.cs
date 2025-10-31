public interface IDiaryService
{
    Task<DiaryDay?> GetDiaryDayAsync(DateOnly day);
    Task<AvailableSlotDto?> GetAvailableSlotAsync(int minutes, DateOnly day, TimeOnly slotStart, TimeOnly slotEnd, DiaryDay diaryDay);
}