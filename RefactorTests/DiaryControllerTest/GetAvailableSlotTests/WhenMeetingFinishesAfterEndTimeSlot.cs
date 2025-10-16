using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Refactor.Tests.DiaryControllerTest.GetAvailableSlotTests
{
    public class WhenMeetingFinishesAfterEndTimeSlot
    {
        private GetAvailableSlotTestHelpers GetAvailableSlotTestHelpers;

        public WhenMeetingFinishesAfterEndTimeSlot()
        {
            GetAvailableSlotTestHelpers = new GetAvailableSlotTestHelpers();
        }

        [Fact]
        public async Task MeetingFinishingAfterEndTimeSlot_DoesNotAffectAvailableSlots()
        {
            var day = new DateOnly(2025, 10, 8);
            var slots = new List<DiarySlot>
            {
                new(startTime: day.ToDateTime(new TimeOnly(9, 45, 0)),
                    endTime: day.ToDateTime(new TimeOnly(10, 15, 0)))
            };
            var diaryDay = new DiaryDay
            {
                Date = day,
                StartTime = new TimeOnly(9, 0, 0),
                EndTime = new TimeOnly(17, 0, 0)
            };
            var context = GetAvailableSlotTestHelpers.GetInMemoryContext(slots, diaryDay);
            var controller = new DiaryController(context);

            var result = await controller.GetAvailableSlot(25, day, new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));
            var slot = (result as OkObjectResult)?.Value as AvailableSlotDto;

            Assert.Multiple(() =>
            {
                Assert.NotNull(slot);
                Assert.True(slot.End <= day.ToDateTime(new TimeOnly(9, 45, 0)));
            });
        }
    }
}