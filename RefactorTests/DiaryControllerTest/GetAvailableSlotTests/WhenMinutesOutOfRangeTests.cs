using Microsoft.AspNetCore.Mvc;

namespace Refactor.Tests.DiaryControllerTest.GetAvailableSlotTests
{
    public class WhenMinutesOutOfRangeTests
    {
        private GetAvailableSlotTestHelpers GetAvailableSlotTestHelpers;

        public WhenMinutesOutOfRangeTests()
        {
            GetAvailableSlotTestHelpers = new GetAvailableSlotTestHelpers();
        }
        [Theory]
        [InlineData(0)]
        [InlineData(121)]
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
