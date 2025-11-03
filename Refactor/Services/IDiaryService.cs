public interface IDiaryService
{
    Task<DiaryDay?> GetDiaryDayAsync(DateOnly day);
    Task<AvailableSlotDto?> GetAvailableSlotAsync(int minutes, DateOnly day, SlotWindow window, DiaryDay diaryDay);
}