using Microsoft.AspNetCore.Mvc;

using Refactor.Controllers;
using Refactor.Data;
using Refactor.Validation;

using Xunit;

namespace Refactor.Tests.DiaryControllerTest.GetAvailableSlotTests
{
    public class WhenMeetingMustFinishBeforeDiaryEndTime
    {
        private readonly GetAvailableSlotTestHelpers _getAvailableSlotTestHelpers;
        private readonly DiaryDayValidator _diaryDayValidator;
        private readonly IDiarySlotRepository _diarySlotRepository;
        private readonly IDiaryDayRepository _diaryDayRepository;
        private readonly DateOnly _testDay = new(2025, 10, 8);
        private readonly DiaryDay _testDiaryDay = new()
        {
            Date = new DateOnly(2025, 10, 8),
            StartTime = new TimeOnly(9, 0, 0),
            EndTime = new TimeOnly(17, 0, 0)
        };

        private readonly DiaryController _sut;

        private readonly DiaryContext _context;
        private readonly DiaryService _diaryService;

        public WhenMeetingMustFinishBeforeDiaryEndTime()
        {
            _getAvailableSlotTestHelpers = new GetAvailableSlotTestHelpers();
            _diaryDayValidator = new DiaryDayValidator(
            [
                new DiaryDayExistsRule(),

            ]);
            _context = _getAvailableSlotTestHelpers.GetInMemoryContext([], _testDiaryDay);
            _diarySlotRepository = new DiarySlotRepository(_context);
            _diaryDayRepository = new DiaryDayRepository(_context);
            _diaryService = new DiaryService(_diaryDayRepository, _diarySlotRepository);
            _sut = new DiaryController(_diaryService, _diaryDayValidator);
        }

        [Fact]
        public async Task ReturnsSlotThatFinishesBeforeDiaryEndTime()
        {
            var window = new SlotWindow(new TimeOnly(16, 30, 0), new TimeOnly(17, 30, 0));
            var result = await _sut.GetAvailableSlot(
                 new GetAvailableSlotRequest
                 {
                     Minutes = 25,
                     Day = _testDay,
                     SlotStart = window.Start,
                     SlotEnd = window.End
                 });
            var slot = (result as OkObjectResult)?.Value as AvailableSlotDto;

            Assert.Multiple(() =>
            {
                Assert.NotNull(slot);
                Assert.Equal(_testDay.ToDateTime(new TimeOnly(16, 30, 0)), slot.Start);
                Assert.Equal(_testDay.ToDateTime(new TimeOnly(16, 55, 0)), slot.End);
                Assert.True(slot.End <= _testDay.ToDateTime(_testDiaryDay.EndTime));
            });
        }

        [Fact]
        public async Task DoesNotReturnSlotIfMeetingWouldFinishAfterDiaryEndTime()
        {
            var window = new SlotWindow(new TimeOnly(16, 45, 0), new TimeOnly(17, 0, 0));
            var result = await _sut.GetAvailableSlot(
                 new GetAvailableSlotRequest
                 {
                     Minutes = 25,
                     Day = _testDay,
                     SlotStart = window.Start,
                     SlotEnd = window.End
                 });
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}