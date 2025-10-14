using Microsoft.AspNetCore.Mvc;

namespace Refactor.Tests.DiaryControllerTest.GetAvailableSlotTests
{
    public class WhenEmptyDiaryForSpecifiedTimeSlotTests
    {
        private GetAvailableSlotTestHelpers GetAvailableSlotTestHelpers;

        public WhenEmptyDiaryForSpecifiedTimeSlotTests()
        {
            GetAvailableSlotTestHelpers = new GetAvailableSlotTestHelpers();
        }

        [Fact]
        public async Task ReturnsOK()
        {
            var slots = new List<DiarySlot>();
            var context = GetAvailableSlotTestHelpers.GetInMemoryContext(slots);
            var controller = new DiaryController(context);

            var day = new DateOnly(2025, 10, 8);
            var result = await controller.GetAvailableSlot(30, day, new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ReturnsAvailableSlot()
        {
            var slots = new List<DiarySlot>();
            var context = GetAvailableSlotTestHelpers.GetInMemoryContext(slots);
            var controller = new DiaryController(context);

            var day = new DateOnly(2025, 10, 8);
            var result = await controller.GetAvailableSlot(30, day, new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));
            var slot = (result as OkObjectResult).Value as AvailableSlotDto;
            Assert.Multiple(() =>
                {
                    Assert.Equal(day.ToDateTime(new TimeOnly(9, 0, 0)), slot.Start);
                    Assert.Equal(day.ToDateTime(new TimeOnly(9, 30, 0)), slot.End);
                }
            );
        }
    }
}
