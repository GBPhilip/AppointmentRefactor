using Moq;

namespace Refactor.Tests.DiaryServiceTests.GetAvailableSlotsAysncTest
{
    public class WhenEmptyDiaryForSpecifiedTimeSlotTests
    {

        [Fact]
        public async Task ReturnsAvailableSlot()
        {
            DateOnly testDay = new(2025, 10, 8);
            DiaryDay testDiaryDay = new()
            {
                Date = new DateOnly(2025, 10, 8),
                StartTime = new TimeOnly(9, 0, 0),
                EndTime = new TimeOnly(17, 0, 0)
            };

            var diaryDayRepository = new Mock<IDiaryDayRepository>();
            var diarySlotRepository = new Mock<IDiarySlotRepository>();
            var sut = new DiaryService(diaryDayRepository.Object, diarySlotRepository.Object); 
            diarySlotRepository.Setup(x => x.GetSlotsForDayAsync(testDay))
                .ReturnsAsync([]);
            var window = new SlotWindow(new TimeOnly(9, 0, 0), new TimeOnly(10, 0, 0));
            var result = await sut.GetAvailableSlotAsync(25, testDay, window, testDiaryDay);

            Assert.Multiple(() =>
                {
                    Assert.Equal(testDay.ToDateTime(new TimeOnly(9, 0, 0)), result.Start);
                    Assert.Equal(testDay.ToDateTime(new TimeOnly(9, 25, 0)), result.End);
                }
            );
        }
    }
}
