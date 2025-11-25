namespace Refactor.Validation
{

    /// <summary>
    /// Validates that the DiaryDay exists.
    /// </summary>
    public class DiaryDayExistsRule : IDiaryDayRule
    {
        public string? Validate(GetAvailableSlotRequest request, DiaryDay? diaryDay)
        => diaryDay == null ? "Diary day information not found." : null;
    }
}