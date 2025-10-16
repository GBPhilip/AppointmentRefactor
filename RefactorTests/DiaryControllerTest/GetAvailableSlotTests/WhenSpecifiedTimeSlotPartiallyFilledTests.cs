using Microsoft.AspNetCore.Mvc;

namespace Refactor.Tests.DiaryControllerTest.GetAvailableSlotTests
{
    public class WhenSpecifiedTimeSlotPartiallyFilledTests
    {
        private GetAvailableSlotTestHelpers GetAvailableSlotTestHelpers;

        public WhenSpecifiedTimeSlotPartiallyFilledTests()
        {
            GetAvailableSlotTestHelpers = new GetAvailableSlotTestHelpers();
        }

        [Fact]
        public async Task ReturnsOK()
        {
            var day = new DateOnly(2025, 10, 8);
            var diaryDay = new DiaryDay
            {
                Date = day,
                StartTime = new TimeOnly(9, 0, 0),
                EndTime = new TimeOnly(17, 0, 0)
            };
            var slots = new List<DiarySlot>();
            var context = GetAvailableSlotTestHelpers.GetInMemoryContext(slots, diaryDay);
            var controller = new DiaryController(context);

            var result = await controller.GetAvailableSlot(25, day, new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ReturnsAvailableSlot_AfterExistingMeeting()
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
                new(startTime: day.ToDateTime(new TimeOnly(9, 0, 0)),
                    endTime: day.ToDateTime(new TimeOnly(9, 25, 0)))
            };
            var context = GetAvailableSlotTestHelpers.GetInMemoryContext(slots, diaryDay);
            var controller = new DiaryController(context);

            var result = await controller.GetAvailableSlot(25, day, new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));
            var slot = (result as OkObjectResult).Value as AvailableSlotDto;
            Assert.Multiple(() =>
                {
                    Assert.Equal(day.ToDateTime(new TimeOnly(9, 30, 0)), slot.Start);
                    Assert.Equal(day.ToDateTime(new TimeOnly(9, 55, 0)), slot.End);
                }
            );
        }
    }
}