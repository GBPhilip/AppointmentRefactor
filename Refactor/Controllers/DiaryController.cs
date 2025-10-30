using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// API controller for managing diary slots and availability.
/// </summary>
[ApiController]
[Route("[controller]")]
public class DiaryController : ControllerBase
{
    private readonly DiaryContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiaryController"/> class.
    /// </summary>
    /// <param name="context">The diary database context.</param>
    public DiaryController(DiaryContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the first available diary slot of the specified duration within the given time window.
    /// The slot will start at a valid quarter-hour (00, 15, 30, or 45 minutes past the hour).
    /// The slotStart must be after the diary start time and the meeting must finish before the diary end time for that day.
    /// </summary>
    /// <param name="minutes">The duration of the slot in minutes. Must be a multiple of 15 plus 10 (e.g., 10, 25, 40, ...).</param>
    /// <param name="day">The date to search for available slots.</param>
    /// <param name="slotStartTime">The earliest time to consider for a slot.</param>
    /// <param name="slotEnd">The latest time to consider for a slot.</param>
    /// <returns>
    /// An <see cref="AvailableSlotDto"/> representing the available slot if found; otherwise, a <see cref="NotFoundResult"/>.
    /// Returns <see cref="BadRequestResult"/> if input parameters are invalid.
    /// </returns>
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableSlot(
        int minutes,
        DateOnly day,
        TimeOnly slotStartTime,
        TimeOnly slotEnd)
    {
        if (slotStartTime >= slotEnd)
            return BadRequest("slotStart must be earlier than slotEnd.");

        if (minutes <= 0 || minutes > 120)
            return BadRequest("Minutes must be between 1 and 120.");

        if ((minutes - 10) < 0 || (minutes - 10) % 15 != 0)
            return BadRequest("Minutes must be a multiple of 15 plus 10 (e.g., 10, 25, 40, ...).");

        var diaryDay = await _context.Set<DiaryDay>().FindAsync(day);

        if (diaryDay == null)
            return BadRequest("Diary day information not found.");

        var diaryStartDateTime = day.ToDateTime(diaryDay.StartTime);
        var diaryEndDateTime = day.ToDateTime(diaryDay.EndTime);

        if (slotStartTime < diaryDay.StartTime)
            return BadRequest("slotStart must be after the diary start time for the day.");

        if (slotEnd > diaryDay.EndTime)
            return BadRequest("slotEnd must be before the diary end time for the day.");

        var maxSlotEnd = diaryEndDateTime.AddMinutes(-(minutes));

        var allowedMinutes = new[] { 0, 15, 30, 45 };
        int minute = maxSlotEnd.Minute;
        if (!allowedMinutes.Contains(minute))
        {
            int prevAllowed = allowedMinutes.Where(m => m < minute).DefaultIfEmpty(45).Max();
            maxSlotEnd = new DateTime(maxSlotEnd.Year, maxSlotEnd.Month, maxSlotEnd.Day, maxSlotEnd.Hour, prevAllowed, 0);
        }

        var endTime = day.ToDateTime(slotEnd);
        if (endTime > diaryEndDateTime)
            endTime = diaryEndDateTime;

        var startDate = day.ToDateTime(slotStartTime); 

        var items = await _context.DiarySlots
            .Where(x => DateOnly.FromDateTime(x.StartTime) == day)
            .OrderBy(x => x.StartTime)
            .ToListAsync() ?? [];

        var current = startDate;
        if (!allowedMinutes.Contains(current.Minute))
        {
            var nextAllowed = allowedMinutes.FirstOrDefault(m => m > current.Minute);
            if (nextAllowed == 0)
                current = current.AddHours(1);
            else
                current = new DateTime(current.Year, current.Month, current.Day, current.Hour, nextAllowed, 0);
        }

        while (current <= endTime)
        {
            var endTimeCandidate = current.AddMinutes(minutes);

            if (current < diaryStartDateTime || endTimeCandidate > diaryEndDateTime)
            {
                current = current.AddMinutes(15);
                continue;
            }

            bool clash = items.Any(y =>
                y.StartTime < endTimeCandidate && y.EndTime > current);

            if (!clash && endTimeCandidate <= endTime)
                return Ok(new AvailableSlotDto { Start = current, End = endTimeCandidate });

            current = current.AddMinutes(15);
        }

        return NotFound("No available slot found.");
    }
}