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

        var startDate = day.Date + slotStart;
        var endTime = day.Date + slotEnd;

        // Get all meetings for the day, ordered by start time
        var items = await _context.DiarySlots
            .Where(x => x.StartTime.Date == day.Date)
            .OrderBy(x => x.StartTime)
            .ToListAsync();

        var current = startDate;
        while (current <= endTime)
        {
            var endTimeCandidate = current.AddMinutes(minutes);

            // Check for overlap with any existing meeting
            bool clash = items.Any(y =>
                y.StartTime < endTimeCandidate && y.EndTime > current);

            if (!clash)
                return Ok(new { Start = current, End = endTimeCandidate });

            // Move to the end of the next conflicting meeting or increment by 1 minute if none found
            var next = items.FirstOrDefault(y => y.StartTime >= current);
            current = next != null && next.EndTime > current
                ? next.EndTime
                : current.AddMinutes(1);
        }

        return NotFound("No available slot found.");
    }
}