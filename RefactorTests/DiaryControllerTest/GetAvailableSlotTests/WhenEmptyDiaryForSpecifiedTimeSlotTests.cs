using Microsoft.AspNetCore.Mvc;

namespace Refactor.Tests.DiaryControllerTest.GetAvailableSlotTests
{
    public class WhenEmptyDiaryForSpecifiedTimeSlotTests
    {
        private readonly GetAvailableSlotTestHelpers GetAvailableSlotTestHelpers = new();

        [Fact]
        public async Task ReturnsOK()
        {
            var slots = new List<DiarySlot>();
            var day = new DateOnly(2025, 10, 8);
            var diaryDay = new DiaryDay
            {
                Date = day,
                StartTime = new TimeOnly(9, 0, 0),
                EndTime = new TimeOnly(17, 0, 0)
            };
            var context = GetAvailableSlotTestHelpers.GetInMemoryContext(slots, diaryDay);
            var controller = new DiaryController(context);

            var result = await controller.GetAvailableSlot(25, day, new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ReturnsAvailableSlot()
        {
            var slots = new List<DiarySlot>();
            var day = new DateOnly(2025, 10, 8);
            var diaryDay = new DiaryDay
            {
                Date = day,
                StartTime = new TimeOnly(9, 0, 0),
                EndTime = new TimeOnly(17, 0, 0)
            };
            var context = GetAvailableSlotTestHelpers.GetInMemoryContext(slots, diaryDay);
            var controller = new DiaryController(context);

            var result = await controller.GetAvailableSlot(25, day, new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));
            var slot = (result as OkObjectResult).Value as AvailableSlotDto;
            Assert.Multiple(() =>
                {
                    Assert.Equal(day.ToDateTime(new TimeOnly(9, 0, 0)), slot.Start);
                    Assert.Equal(day.ToDateTime(new TimeOnly(9, 25, 0)), slot.End);
                }
            );
        }
    }
}
