/// <summary>
/// Validates that the slot start time is after the diary day's start time.
/// </summary>
public class SlotStartAfterDiaryStartRule : IDiaryDayRule
{
 public string? Validate(GetAvailableSlotRequest request, DiaryDay? diaryDay)
 => diaryDay != null && request.SlotStart < diaryDay.StartTime
 ? "slotStart must be after the diary start time for the day."
 : null;
}
