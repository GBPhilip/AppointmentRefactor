using Microsoft.AspNetCore.Mvc;

/// <summary>
/// API controller for managing diary slots and availability.
/// </summary>
[ApiController]
[Route("[controller]")]
public class DiaryController : ControllerBase
{
    private readonly IDiaryService _diaryService;
    private readonly DiaryDayValidator _diaryDayValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiaryController"/> class.
    /// </summary>
    /// <param name="diaryService">The service for accessing diary information and availability.</param>
    /// <param name="diaryDayValidator">The validator for diary day rules.</param>
    public DiaryController(IDiaryService diaryService, DiaryDayValidator diaryDayValidator)
    {
        _diaryService = diaryService;
        _diaryDayValidator = diaryDayValidator;
    }

    private string? ValidateSlotParameters(TimeOnly slotStart, TimeOnly slotEnd, int minutes)
    {
        if (slotStart >= slotEnd)
            return "slotStart must be earlier than slotEnd.";
        if ((minutes - 10) < 0 || (minutes - 10) % 15 != 0)
            return "Minutes must be a multiple of 15 plus 10 (e.g., 10, 25, 40, ...).";
        return null;
    }

    /// <summary>
    /// Gets the first available diary slot of the specified duration within the given time window.
    /// The slot will start at a valid quarter-hour (00, 15, 30, or 45 minutes past the hour).
    /// The slotStart must be after the diary start time and the meeting must finish before the diary end time for that day.
    /// </summary>
    /// <param name="request">The request object containing the parameters for the operation.</param>
    /// <returns>
    /// An <see cref="AvailableSlotDto"/> representing the available slot if found; otherwise, a <see cref="NotFoundResult"/>.
    /// Returns <see cref="BadRequestResult"/> if input parameters are invalid.
    /// </returns>
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableSlot([FromQuery] GetAvailableSlotRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var paramError = ValidateSlotParameters(request.SlotStart, request.SlotEnd, request.Minutes);
        if (paramError != null)
            return BadRequest(paramError);

        var diaryDay = await _diaryService.GetDiaryDayAsync(request.Day);
        var diaryDayError = _diaryDayValidator.Validate(request, diaryDay);
        if (diaryDayError != null)
            return BadRequest(diaryDayError);

        var slot = await _diaryService.GetAvailableSlotAsync(request.Minutes, request.Day, request.SlotStart, request.SlotEnd, diaryDay);
        if (slot != null)
            return Ok(slot);
        return NotFound("No available slot found.");
    }
}