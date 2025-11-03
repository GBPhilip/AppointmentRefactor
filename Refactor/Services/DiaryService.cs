public class DiaryService : IDiaryService
{
    private readonly IDiaryDayRepository _diaryDayRepository;
    private readonly IDiarySlotRepository _diarySlotRepository;

    public DiaryService(IDiaryDayRepository diaryDayRepository, IDiarySlotRepository diarySlotRepository)
    {
        _diaryDayRepository = diaryDayRepository;
        _diarySlotRepository = diarySlotRepository;
    }

    public async Task<DiaryDay?> GetDiaryDayAsync(DateOnly day)
    {
        return await _diaryDayRepository.GetDiaryDayAsync(day);
    }

    public async Task<AvailableSlotDto?> GetAvailableSlotAsync(
        int minutes,
        DateOnly day,
        SlotWindow window,
        DiaryDay diaryDay)
    {
        var items = await _diarySlotRepository.GetSlotsForDayAsync(day);
        return SlotFinder.FindAvailableSlot(minutes, day, window.Start, window.End, diaryDay, items);
    }
}