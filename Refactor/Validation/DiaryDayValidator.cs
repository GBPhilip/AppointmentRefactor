using System.Collections.Generic;

/// <summary>
/// Aggregates and applies DiaryDay validation rules for available slot requests.
/// </summary>
public class DiaryDayValidator
{
    private readonly List<IDiaryDayRule> _rules;

    public DiaryDayValidator(IEnumerable<IDiaryDayRule> rules)
    {
        _rules = new List<IDiaryDayRule>(rules);
    }

    /// <summary>
    /// Validates all rules and returns the first error message found, or null if all pass.
    /// </summary>
    public string? Validate(GetAvailableSlotRequest request, DiaryDay? diaryDay)
    {
        foreach (var rule in _rules)
        {
            var error = rule.Validate(request, diaryDay);
            if (error != null)
                return error;
        }
        return null;
    }
}
