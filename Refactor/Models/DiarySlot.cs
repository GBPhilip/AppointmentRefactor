using System;

public class DiarySlot
{
    public DiarySlot()
    {
    }

    public DiarySlot(DateTime startTime, DateTime endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    // Add other relevant properties if needed
}