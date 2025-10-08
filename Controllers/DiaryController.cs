using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public class DiaryController : ControllerBase
{
    private readonly DiaryContext _context;

    public DiaryController(DiaryContext context)
    {
        _context = context;
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableSlot(
        int minutes,
        DateTime day,
        TimeSpan slotStart,
        TimeSpan slotEnd)
    {
        if (minutes <= 0 || minutes > 120)
            return BadRequest("Minutes must be between 1 and 120.");

        var startDateTime = day.Date + slotStart;
        var endDateTime = day.Date + slotEnd;

        // Get all meetings for the day, ordered by start time
        var existingMeetings = await _context.DiarySlots
            .Where(s => s.StartTime.Date == day.Date)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        var current = startDateTime;
        while (current <= endDateTime)
        {
            var proposedEnd = current.AddMinutes(minutes);

            // Check for overlap with any existing meeting
            bool conflict = existingMeetings.Any(m =>
                m.StartTime < proposedEnd && m.EndTime > current);

            if (!conflict)
                return Ok(new { Start = current, End = proposedEnd });

            // Move to the end of the next conflicting meeting or increment by 1 minute if none found
            var nextMeeting = existingMeetings.FirstOrDefault(m => m.StartTime >= current);
            current = nextMeeting != null && nextMeeting.EndTime > current
                ? nextMeeting.EndTime
                : current.AddMinutes(1);
        }

        return NotFound("No available slot found.");
    }
}