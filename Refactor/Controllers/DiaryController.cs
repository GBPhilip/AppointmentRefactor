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
        if (slotStart >= slotEnd)
            return BadRequest("slotStart must be earlier than slotEnd.");

        if (minutes <= 0 || minutes > 120)
            return BadRequest("Minutes must be between 1 and 120.");

        if ((minutes - 10) < 0 || (minutes - 10) % 15 != 0)
            return BadRequest("Minutes must be a multiple of 15 plus 10 (e.g., 10, 25, 40, ...).");

        var startDate = day.ToDateTime(slotStart);
        var endTime = day.ToDateTime(slotEnd);

        var items = await _context.DiarySlots
            .Where(x => DateOnly.FromDateTime(x.StartTime) == day)
            .OrderBy(x => x.StartTime)
            .ToListAsync();

        var current = startDate;
        var allowedMinutes = new[] { 0, 15, 30, 45 };
        if (!allowedMinutes.Contains(current.Minute))
        {
            var nextAllowed = allowedMinutes.FirstOrDefault(m => m > current.Minute);
            if (nextAllowed == 0) 
                current = new DateTime(current.Year, current.Month, current.Day, current.Hour + 1, 0, 0);
            else
                current = new DateTime(current.Year, current.Month, current.Day, current.Hour, nextAllowed, 0);
        }

        while (current < endTime)
        {
            var endTimeCandidate = current.AddMinutes(minutes);

            bool clash = items.Any(y =>
                y.StartTime < endTimeCandidate && y.EndTime > current);

            if (!clash && endTimeCandidate <= endTime)
                return Ok(new AvailableSlotDto { Start = current, End = endTimeCandidate });

            current = current.AddMinutes(15); // Always move to next quarter-hour
        }

        return NotFound("No available slot found.");
    }
}