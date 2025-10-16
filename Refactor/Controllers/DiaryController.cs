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
    private readonly IDiaryDayRepository _diaryDayRepository;
    private readonly IDiarySlotRepository _diarySlotRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiaryController"/> class.
    /// </summary>
    /// <param name="context">The diary database context.</param>
    /// <param name="diaryDayRepository">The repository for accessing diary day information.</param>
    /// <param name="diarySlotRepository">The repository for accessing diary slot information.</param>
    public DiaryController(DiaryContext context, IDiaryDayRepository diaryDayRepository, IDiarySlotRepository diarySlotRepository)
    {
        _context = context;
        _diaryDayRepository = diaryDayRepository;
        _diarySlotRepository = diarySlotRepository;
    }

    private string ValidateSlotRequest(int minutes, DateOnly day, TimeOnly slotStart, TimeOnly slotEnd, DiaryDay diaryDay)
    {
        if (slotStart >= slotEnd)
            return "slotStart must be earlier than slotEnd.";
        if (minutes <= 0 || minutes > 120)
            return "Minutes must be between 1 and 120.";
        if ((minutes - 10) < 0 || (minutes - 10) % 15 != 0)
            return "Minutes must be a multiple of 15 plus 10 (e.g., 10, 25, 40, ...).";
        if (diaryDay == null)
            return "Diary day information not found.";
        if (slotStart < diaryDay.StartTime)
            return "slotStart must be after the diary start time for the day.";
        if (slotEnd > diaryDay.EndTime)
            return "slotEnd must be before the diary end time for the day.";
        return null;
    }

    /// <summary>
    /// Gets the first available diary slot of the specified duration within the given time window.
    /// The slot will start at a valid quarter-hour (00, 15, 30, or 45 minutes past the hour).
    /// The slotStart must be after the diary start time and the meeting must finish before the diary end time for that day.
    /// </summary>
    /// <param name="minutes">The duration of the slot in minutes. Must be a multiple of 15 plus 10 (e.g., 10, 25, 40, ...).</param>
    /// <param name="day">The date to search for available slots.</param>
    /// <param name="slotStart">The earliest time to consider for a slot.</param>
    /// <param name="slotEnd">The latest time to consider for a slot.</param>
    /// <returns>
    /// An <see cref="AvailableSlotDto"/> representing the available slot if found; otherwise, a <see cref="NotFoundResult"/>.
    /// Returns <see cref="BadRequestResult"/> if input parameters are invalid.
    /// </returns>
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableSlot(
        int minutes,
        DateOnly day,
        TimeOnly slotStart,
        TimeOnly slotEnd)
    {
        var diaryDay = await _diaryDayRepository.GetDiaryDayAsync(day);
        var validationError = ValidateSlotRequest(minutes, day, slotStart, slotEnd, diaryDay);
        if (validationError != null)
            return BadRequest(validationError);

        var items = await _diarySlotRepository.GetSlotsForDayAsync(day) ?? [];

        var slot = SlotFinder.FindAvailableSlot(minutes, day, slotStart, slotEnd, diaryDay, items);
        if (slot != null)
            return Ok(slot);
        return NotFound("No available slot found.");
    }
}