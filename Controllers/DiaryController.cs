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

        // Find all slots that overlap with the requested window
        var existingMeetings = await _context.DiarySlots
            .Where(s => s.StartTime.Date == day.Date &&
                        s.EndTime > startDateTime &&
                        s.StartTime < endDateTime)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        var current = startDateTime;
        while (current.AddMinutes(minutes) <= endDateTime)
        {
            var next = current.AddMinutes(minutes);
            bool conflict = existingMeetings.Any(m =>
                m.StartTime < next && m.EndTime > current);

            if (!conflict)
                return Ok(new { Start = current, End = next });

            // Move to the end of the next conflicting meeting
            var nextMeeting = existingMeetings.First(m => m.StartTime < next && m.EndTime > current);
            current = nextMeeting.EndTime;
        }

        return NotFound("No available slot found.");
    }
}