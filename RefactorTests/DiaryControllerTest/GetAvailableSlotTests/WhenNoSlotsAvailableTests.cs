using Microsoft.AspNetCore.Mvc;

using Refactor.Controllers;
using Refactor.Data;
using Refactor.Validation;

namespace Refactor.Tests.DiaryControllerTest.GetAvailableSlotTests
{
    public class WhenNoSlotsAvailableTests
    {
        [Fact]
        public async Task ReturnsNotFound()
        {
            var getAvailableSlotTestHelpers = new GetAvailableSlotTestHelpers();
            var diaryDayValidator = new DiaryDayValidator(
           [
               new DiaryDayExistsRule(),

           ]);
            var testDay = new DateOnly(2025, 10, 8);
            var diaryDay = new DiaryDay
            {
                Date = testDay,
                StartTime = new TimeOnly(9, 0, 0),
                EndTime = new TimeOnly(17, 0, 0)
            };
            var slots = new List<DiarySlot>
            {
                new() {
                    StartTime = testDay.ToDateTime(new TimeOnly(9, 0, 0)),
                    EndTime = testDay.ToDateTime(new TimeOnly(10, 0, 0))
                }
            };
            var context = getAvailableSlotTestHelpers.GetInMemoryContext(slots, diaryDay);
            var diarySlotRepository = new DiarySlotRepository(context);
            var diaryDayRepository = new DiaryDayRepository(context);
            var diaryService = new DiaryService(diaryDayRepository, diarySlotRepository);
            var sut = new DiaryController(diaryService, diaryDayValidator);
            var result = await sut.GetAvailableSlot(
                            new GetAvailableSlotRequest
                            {
                                Minutes = 25,
                                Day = testDay,
                                SlotStart = new TimeOnly(9, 0, 0),
                                SlotEnd = new TimeOnly(10, 0, 0)
                            });

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}