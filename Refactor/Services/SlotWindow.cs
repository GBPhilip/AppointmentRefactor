public class SlotWindow
{
    public TimeOnly Start { get; }
    public TimeOnly End { get; }

    public SlotWindow(TimeOnly start, TimeOnly end)
    {
        if (end < start)
            throw new ArgumentException("End time must not be before start time.", nameof(end));
        Start = start;
        End = end;
    }
}