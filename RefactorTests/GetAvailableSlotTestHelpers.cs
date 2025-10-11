using Microsoft.EntityFrameworkCore;

namespace Refactor.Tests
{
    internal class GetAvailableSlotTestHelpers
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
    }
}
