using Microsoft.EntityFrameworkCore;

namespace Refactor.Tests
{
    internal class GetAvailableSlotTestHelpers
    {
        internal DiaryContext GetInMemoryContext(List<DiarySlot> slots, DiaryDay diaryDay)
        {
            var options = new DbContextOptionsBuilder<DiaryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new DiaryContext(options);
            context.DiarySlots.AddRange(slots);
            if (diaryDay != null)
            {
                context.Set<DiaryDay>().Add(diaryDay);
            }
            context.SaveChanges();
            return context;
        }
    }
}
