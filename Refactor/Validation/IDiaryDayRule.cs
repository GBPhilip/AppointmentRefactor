namespace Refactor.Validation
{

    /// <summary>
    /// Rule interface for validating DiaryDay-related constraints for available slot requests.
    /// </summary>
    public interface IDiaryDayRule
    {
        /// <summary>
        /// Validates the rule against the request and diary day.
        /// Returns an error message if validation fails, otherwise null.
        /// </summary>
        string? Validate(GetAvailableSlotRequest request, DiaryDay? diaryDay);
    }
}