using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Refactor.Tests
{
    public class DiaryControllerTests
    {
        internal DiaryContext GetInMemoryContext(List<DiarySlot> slots)
        {
            var options = new DbContextOptionsBuilder<DiaryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new DiaryContext(options);
            context.DiarySlots.AddRange(slots);
            context.SaveChanges();
            return context;
        }
        [Fact]
        public async Task ReturnsNotFound_WhenNoSlotAvailable()
        {
            var day = new DateOnly(2025, 10, 8);
            var slots = new List<DiarySlot>
            {
                new DiarySlot
                {
                    StartTime = day.ToDateTime(new TimeOnly(9, 0, 0)),
                    EndTime = day.ToDateTime(new TimeOnly(10, 0, 0))
                }
            };
            var context = GetInMemoryContext(slots);
            var controller = new DiaryController(context);

            var result = await controller.GetAvailableSlot(30, day, new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task ReturnsBadRequest_WhenMinutesOutOfRange()
        {
            var context = GetInMemoryContext(new List<DiarySlot>());
            var controller = new DiaryController(context);
            var day = new DateOnly(2025, 10, 8);

            var result = await controller.GetAvailableSlot(0, day, new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}