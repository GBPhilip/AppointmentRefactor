using Microsoft.AspNetCore.Mvc;

namespace Refactor.Tests.DiaryControllerTest.GetAvailableSlotTests
{
    public class WhenMinutesOutOfRangeTests
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

        public WhenMinutesOutOfRangeTests()
        {
            _getAvailableSlotTestHelpers = new GetAvailableSlotTestHelpers();
            _diaryDayValidator = new DiaryDayValidator(
            [
                new DiaryDayExistsRule(),
                new SlotStartAfterDiaryStartRule(),
                new SlotEndBeforeDiaryEndRule()
            ]);
            _context = _getAvailableSlotTestHelpers.GetInMemoryContext([], _testDiaryDay);
            _diarySlotRepository = new DiarySlotRepository(_context);
            _diaryDayRepository = new DiaryDayRepository(_context);
            _diaryService = new DiaryService(_diaryDayRepository, _diarySlotRepository);
            _sut = new DiaryController(_diaryService, _diaryDayValidator);

        }

        [Theory]
        [InlineData(0)]
        [InlineData(121)]
        public async Task ReturnsBadRequest_WhenMinutesOutOfRange(int minutes)
        {
            var window = new SlotWindow(new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));
            var result = await _sut.GetAvailableSlot(
                 new GetAvailableSlotRequest
                 {
                     Minutes = minutes,
                     Day = _testDay,
                     SlotStart = window.Start,
                     SlotEnd = window.End
                 });
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
