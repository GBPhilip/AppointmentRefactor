using Microsoft.AspNetCore.Mvc;

namespace Refactor.Tests.GetAvailableSlotTests.ReturnsAvailableSlot
{
    public class AfterExistingMeeting
    {
        private GetAvailableSlotTestHelpers GetAvailableSlotTestHelpers;

        public AfterExistingMeeting()
        {
            GetAvailableSlotTestHelpers = new GetAvailableSlotTestHelpers();
        }

        [Fact]
        public async Task ReturnsAvailableSlot_AfterExistingMeeting()
        {
            var day = new DateOnly(2025, 10, 8);
            var slots = new List<DiarySlot>
            {
                new DiarySlot
                {
                    StartTime = day.ToDateTime(new TimeOnly(9, 0, 0)),
                    EndTime = day.ToDateTime(new TimeOnly(9, 30, 0))
                }
            };
            var context = GetAvailableSlotTestHelpers.GetInMemoryContext(slots);
            var controller = new DiaryController(context);

            var result = await controller.GetAvailableSlot(30, day, new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));
            var slot = (result as OkObjectResult).Value as AvailableSlotDto;
            Assert.Multiple(() =>
                {
                    Assert.Equal(day.ToDateTime(new TimeOnly(9, 30, 0)), slot.Start);
                    Assert.Equal(day.ToDateTime(new TimeOnly(10, 0, 0)), slot.End);
                }
            );
        }
    }
}