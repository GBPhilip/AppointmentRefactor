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
        DateOnly day,
        TimeOnly slotStart,
        TimeOnly slotEnd)
    {
        if (minutes <= 0 || minutes > 120)
            return BadRequest("Minutes must be between 1 and 120.");

        if (slotStart >= slotEnd)
            return BadRequest("slotStart must be earlier than slotEnd.");

        var startDate = day.ToDateTime(slotStart);
        var endTime = day.ToDateTime(slotEnd);

        var items = await _context.DiarySlots
            .Where(x => DateOnly.FromDateTime(x.StartTime) == day)
            .OrderBy(x => x.StartTime)
            .ToListAsync();

        var current = startDate;
        while (current <= endTime)
        {
            var endTimeCandidate = current.AddMinutes(minutes);

            bool clash = items.Any(y =>
                y.StartTime < endTimeCandidate && y.EndTime > current);

            if (!clash)
                return Ok(new { Start = current, End = endTimeCandidate });

            var next = items.FirstOrDefault(y => y.StartTime >= current);
            current = next != null && next.EndTime > current
                ? next.EndTime
                : current.AddMinutes(1);
        }

        return NotFound("No available slot found.");
    }
}