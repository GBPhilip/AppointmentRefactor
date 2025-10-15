using Microsoft.AspNetCore.Mvc;

namespace Refactor.Tests.DiaryControllerTest.GetAvailableSlotTests
{
    public class WhenStartSlotIsAfterEndSlotTest
    {
        private GetAvailableSlotTestHelpers GetAvailableSlotTestHelpers;

        public WhenStartSlotIsAfterEndSlotTest()
        {
            GetAvailableSlotTestHelpers = new GetAvailableSlotTestHelpers();
        }

        [Fact]
        public async Task ReturnsBadRequest()
        {
            var context = GetAvailableSlotTestHelpers.GetInMemoryContext([]);
            var controller = new DiaryController(context);
            var day = new DateOnly(2025, 10, 8);

            var result = await controller.GetAvailableSlot(25, day, new TimeOnly(9, 30, 0), new TimeOnly(9, 0, 0));
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
