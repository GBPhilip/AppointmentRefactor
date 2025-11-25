using Refactor.Validation;

/// <summary>
/// Validates that the slot end time is before the diary day's end time.
/// </summary>
public class SlotEndBeforeDiaryEndRule : IDiaryDayRule
{
 public string? Validate(GetAvailableSlotRequest request, DiaryDay? diaryDay)
 => diaryDay != null && request.SlotEnd > diaryDay.EndTime
 ? "slotEnd must be before the diary end time for the day."
 : null;
}
