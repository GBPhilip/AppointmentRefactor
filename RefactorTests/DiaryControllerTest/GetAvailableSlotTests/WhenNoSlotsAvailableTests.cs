using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Refactor.Tests.DiaryControllerTest.GetAvailableSlotTests
{
    public class WhenNoSlotsAvailableTests
    {
        private GetAvailableSlotTestHelpers GetAvailableSlotTestHelpers;
        public WhenNoSlotsAvailableTests()
        {
            GetAvailableSlotTestHelpers = new GetAvailableSlotTestHelpers();
        }

        [Fact]
        public async Task ReturnsNotFound()
        {
            var day = new DateOnly(2025, 10, 8);
            var diaryDay = new DiaryDay
            {
                Date = day,
                StartTime = new TimeOnly(9, 0, 0),
                EndTime = new TimeOnly(17, 0, 0)
            };
            var slots = new List<DiarySlot>
            {
                new() {
                    StartTime = day.ToDateTime(new TimeOnly(9, 0, 0)),
                    EndTime = day.ToDateTime(new TimeOnly(10, 0, 0))
                }
            };
            var context = GetAvailableSlotTestHelpers.GetInMemoryContext(slots, diaryDay);
            var controller = new DiaryController(context);

            var result = await controller.GetAvailableSlot(25, day, new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}