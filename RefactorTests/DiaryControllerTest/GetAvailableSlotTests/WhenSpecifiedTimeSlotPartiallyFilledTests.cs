using Microsoft.AspNetCore.Mvc;
using Refactor.Data;

namespace Refactor.Tests.DiaryControllerTest.GetAvailableSlotTests
{
    public class WhenSpecifiedTimeSlotPartiallyFilledTests
    {
        private GetAvailableSlotTestHelpers _getAvailableSlotTestHelpers;
        private DiaryDayValidator _diaryDayValidator;
        private IDiarySlotRepository _diarySlotRepository;
        private IDiaryDayRepository _diaryDayRepository;
        private readonly DateOnly _testDay = new(2025, 10, 8);
        private readonly DiaryDay _testDiaryDay = new()
        {
            Date = new DateOnly(2025, 10, 8),
            StartTime = new TimeOnly(9, 0, 0),
            EndTime = new TimeOnly(17, 0, 0)
        };

        private readonly DiaryController _sut;

        private DiaryContext _context;
        private readonly DiaryService _diaryService;

        public WhenSpecifiedTimeSlotPartiallyFilledTests()
        {
            _getAvailableSlotTestHelpers = new GetAvailableSlotTestHelpers();
            _diaryDayValidator = new DiaryDayValidator(
            [
                new DiaryDayExistsRule(),
                new SlotStartAfterDiaryStartRule(),
                new SlotEndBeforeDiaryEndRule()
            ]);
            var slots = new List<DiarySlot>
            {
                new(startTime: _testDay.ToDateTime(new TimeOnly(9, 0, 0)),
                    endTime: _testDay.ToDateTime(new TimeOnly(9, 25, 0)))
            };
            _context = _getAvailableSlotTestHelpers.GetInMemoryContext(slots, _testDiaryDay);
            _diarySlotRepository = new DiarySlotRepository(_context);
            _diaryDayRepository = new DiaryDayRepository(_context);
            _diaryService = new DiaryService(_diaryDayRepository, _diarySlotRepository);
            _sut = new DiaryController(_diaryService, _diaryDayValidator);

        }

        [Fact]
        public async Task ReturnsOK()
        {
            var window = new SlotWindow(new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));
            var result = await _sut.GetAvailableSlot(
                 new GetAvailableSlotRequest
                 {
                     Minutes = 25,
                     Day = _testDay,
                     SlotStart = window.Start,
                     SlotEnd = window.End
                 });

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ReturnsAvailableSlot_AfterExistingMeeting()
        {
            var window = new SlotWindow(new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));
            var result = await _sut.GetAvailableSlot(
                 new GetAvailableSlotRequest
                 {
                     Minutes = 25,
                     Day = _testDay,
                     SlotStart = window.Start,
                     SlotEnd = window.End
                 });
            var slot = (result as OkObjectResult).Value as AvailableSlotDto;
            Assert.Multiple(() =>
                {
                    Assert.Equal(_testDay.ToDateTime(new TimeOnly(9, 30, 0)), slot.Start);
                    Assert.Equal(_testDay.ToDateTime(new TimeOnly(9, 55, 0)), slot.End);
                }
            );
        }
    }
}