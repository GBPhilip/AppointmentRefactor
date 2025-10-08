using Microsoft.EntityFrameworkCore;

public class DiaryContext : DbContext
{
    public DiaryContext(DbContextOptions<DiaryContext> options) : base(options) { }

    public DbSet<DiarySlot> DiarySlots { get; set; }
}