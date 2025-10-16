using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Request model for retrieving the first available diary slot of a specified duration within a given time window.
/// </summary>
public class GetAvailableSlotRequest
{
    /// <summary>
    /// The duration of the slot in minutes. Must be between 1 and 120.
    /// </summary>
    [Range(1, 120, ErrorMessage = "Minutes must be between 1 and 120.")]
    public int Minutes { get; set; }

    /// <summary>
    /// The date to search for available slots.
    /// </summary>
    [Required]
    public DateOnly Day { get; set; }

    /// <summary>
    /// The earliest time to consider for a slot.
    /// </summary>
    [Required]
    public TimeOnly SlotStart { get; set; }

    /// <summary>
    /// The latest time to consider for a slot.
    /// </summary>
    [Required]
    public TimeOnly SlotEnd { get; set; }
}
