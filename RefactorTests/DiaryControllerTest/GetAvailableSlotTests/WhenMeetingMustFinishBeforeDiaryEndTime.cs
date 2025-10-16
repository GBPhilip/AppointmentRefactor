using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Refactor.Tests.DiaryControllerTest.GetAvailableSlotTests
{
    public class WhenMeetingMustFinishBeforeDiaryEndTime
    {
        private GetAvailableSlotTestHelpers GetAvailableSlotTestHelpers;

        public WhenMeetingMustFinishBeforeDiaryEndTime()
        {
            GetAvailableSlotTestHelpers = new GetAvailableSlotTestHelpers();
        }

        [Fact]
        public async Task ReturnsSlotThatFinishesBeforeDiaryEndTime()
        {
            var day = new DateOnly(2025, 10, 8);
            var diaryDay = new DiaryDay
            {
                Date = day,
                StartTime = new TimeOnly(9, 0, 0),
                EndTime = new TimeOnly(17, 0, 0)
            };

            var slotStart = new TimeOnly(16, 30, 0);
            var startDate = day.ToDateTime(slotStart); // Should be 2025-10-08 16:30:00

            var slots = new List<DiarySlot>();
            var context = GetAvailableSlotTestHelpers.GetInMemoryContext(slots, diaryDay);
            var controller = new DiaryController(context);

            var result = await controller.GetAvailableSlot(25, day, new TimeOnly(16, 30, 0), new TimeOnly(17, 0, 0));
            var slot = (result as OkObjectResult)?.Value as AvailableSlotDto;

            Assert.Multiple(() =>
            {
                Assert.NotNull(slot);
                Assert.Equal(day.ToDateTime(new TimeOnly(16, 30, 0)), slot.Start);
                Assert.Equal(day.ToDateTime(new TimeOnly(16, 55, 0)), slot.End);
                Assert.True(slot.End <= day.ToDateTime(diaryDay.EndTime));
            });
        }

        [Fact]
        public async Task DoesNotReturnSlotIfMeetingWouldFinishAfterDiaryEndTime()
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

            // Request a slot that would end after diary end time
            var result = await controller.GetAvailableSlot(25, day, new TimeOnly(16, 45, 0), new TimeOnly(17, 0, 0));
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}