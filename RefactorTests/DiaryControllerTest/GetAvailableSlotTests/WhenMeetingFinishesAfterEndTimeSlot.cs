using Microsoft.AspNetCore.Mvc;
using Refactor.Data;
using Xunit;

namespace Refactor.Tests.DiaryControllerTest.GetAvailableSlotTests
{
    public class WhenMeetingFinishesAfterEndTimeSlot
    {
        [Fact]
        public async Task MeetingFinishingAfterEndTimeSlot_DoesNotAffectAvailableSlots()
        {
            DateOnly testDay = new(2025, 10, 8);
            DiaryDay testDiaryDay = new()
            {
                Date = new DateOnly(2025, 10, 8),
                StartTime = new TimeOnly(9, 0, 0),
                EndTime = new TimeOnly(17, 0, 0)
            };

            var getAvailableSlotTestHelpers = new GetAvailableSlotTestHelpers();
            var diaryDayValidator = new DiaryDayValidator(
                [
                    new DiaryDayExistsRule(),
                new SlotStartAfterDiaryStartRule(),
                new SlotEndBeforeDiaryEndRule()
                ]);
            var slots = new List<DiarySlot>
            {
                new(startTime: testDay.ToDateTime(new TimeOnly(9, 45, 0)),
                    endTime: testDay.ToDateTime(new TimeOnly(10, 15, 0)))
            };
            var context = getAvailableSlotTestHelpers.GetInMemoryContext(slots, testDiaryDay);
            var diarySlotRepository = new DiarySlotRepository(context);
            var diaryDayRepository = new DiaryDayRepository(context);
            var diaryService = new DiaryService(diaryDayRepository, diarySlotRepository);
            var sut = new DiaryController(diaryService, diaryDayValidator);
            var result = await sut.GetAvailableSlot(
                 new GetAvailableSlotRequest
                 {
                     Minutes = 25,
                     Day = testDay,
                     SlotStart = new TimeOnly(9, 0, 0),
                     SlotEnd = new TimeOnly(10, 0, 0)
                 });

            var slot = (result as OkObjectResult)?.Value as AvailableSlotDto;

            Assert.Multiple(() =>
            {
                Assert.NotNull(slot);
                Assert.True(slot.End <= testDay.ToDateTime(new TimeOnly(9, 45, 0)));
            });
        }
    }
}