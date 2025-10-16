using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the diary configuration for a specific day, including start and end times.
/// </summary>
public class DiaryDay
{
    [Key]
    public DateOnly Date { get; set; }

    /// <summary>
    /// The start time for diary slots on this day.
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// The end time for diary slots on this day.
    /// </summary>
    public TimeOnly EndTime { get; set; }
}