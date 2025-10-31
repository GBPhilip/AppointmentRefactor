using Microsoft.AspNetCore.Mvc;

namespace Refactor.Tests.DiaryControllerTest.GetAvailableSlotTests
{
    public class WhenStartSlotIsAfterEndSlotTest
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

        public WhenStartSlotIsAfterEndSlotTest()
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

        [Fact]
        public async Task ReturnsBadRequest()
        {

            var result = await _sut.GetAvailableSlot(
                 new GetAvailableSlotRequest
                 { 
                     Minutes = 25, 
                     Day = _testDay, 
                     SlotStart = new TimeOnly(9, 30, 0), 
                     SlotEnd = new TimeOnly(9, 0, 0)
                 });
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
