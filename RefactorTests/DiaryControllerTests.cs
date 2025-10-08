using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Refactor.Tests
{
    public class DiaryControllerTests
    {
        private DiaryContext GetInMemoryContext(List<DiarySlot> slots)
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
        public async Task ReturnsAvailableSlot_WhenNoClash()
        {
            var slots = new List<DiarySlot>();
            var context = GetInMemoryContext(slots);
            var controller = new DiaryController(context);

            var day = new DateTime(2025, 10, 8);
            var result = await controller.GetAvailableSlot(30, day, new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0));

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic slot = okResult.Value;
            Assert.Equal(day.Date + new TimeSpan(9, 0, 0), slot.Start);
            Assert.Equal(day.Date + new TimeSpan(9, 30, 0), slot.End);
        }

        [Fact]
        public async Task ReturnsNotFound_WhenNoSlotAvailable()
        {
            var day = new DateTime(2025, 10, 8);
            var slots = new List<DiarySlot>
            {
                new DiarySlot
                {
                    StartTime = day.Date + new TimeSpan(9, 0, 0),
                    EndTime = day.Date + new TimeSpan(10, 0, 0)
                }
            };
            var context = GetInMemoryContext(slots);
            var controller = new DiaryController(context);

            var result = await controller.GetAvailableSlot(30, day, new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0));

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task ReturnsAvailableSlot_AfterExistingMeeting()
        {
            var day = new DateTime(2025, 10, 8);
            var slots = new List<DiarySlot>
            {
                new DiarySlot
                {
                    StartTime = day.Date + new TimeSpan(9, 0, 0),
                    EndTime = day.Date + new TimeSpan(9, 30, 0)
                }
            };
            var context = GetInMemoryContext(slots);
            var controller = new DiaryController(context);

            var result = await controller.GetAvailableSlot(30, day, new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0));

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic slot = okResult.Value;
            Assert.Equal(day.Date + new TimeSpan(9, 30, 0), slot.Start);
            Assert.Equal(day.Date + new TimeSpan(10, 0, 0), slot.End);
        }

        [Fact]
        public async Task ReturnsBadRequest_WhenMinutesOutOfRange()
        {
            var context = GetInMemoryContext(new List<DiarySlot>());
            var controller = new DiaryController(context);
            var day = new DateTime(2025, 10, 8);

            var result = await controller.GetAvailableSlot(0, day, new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0));
            Assert.IsType<BadRequestObjectResult>(result);

            result = await controller.GetAvailableSlot(121, day, new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0));
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}