using Microsoft.AspNetCore.Mvc;

namespace Refactor.Tests.DiaryControllerTest.GetAvailableSlotTests
{
    public class WhenMinutesNotMultipleOf15Plus10Tests
    {
        private GetAvailableSlotTestHelpers GetAvailableSlotTestHelpers;

        public WhenMinutesNotMultipleOf15Plus10Tests()
        {
            GetAvailableSlotTestHelpers = new GetAvailableSlotTestHelpers();
        }
        [Theory]
        [InlineData(15)]
        [InlineData(24)]
        public async Task ReturnsBadRequest_WhenMinutesOutOfRange(int minutes)
        {
            var context = GetAvailableSlotTestHelpers.GetInMemoryContext([]);
            var controller = new DiaryController(context);
            var day = new DateOnly(2025, 10, 8);

            var result = await controller.GetAvailableSlot(minutes, day, new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));
            Assert.IsType<BadRequestObjectResult>(result);
        }


    }
}
