using System;
using System.Collections.Generic;
using System.Linq;

public class SlotFinder
{
    private static readonly int[] AllowedMinutes = { 0, 15, 30, 45 };

    public static AvailableSlotDto? FindAvailableSlot(
        int minutes,
        DateOnly day,
        TimeOnly slotStart,
        TimeOnly slotEnd,
        DiaryDay diaryDay,
        List<DiarySlot> items)
    {
        var diaryStartDateTime = day.ToDateTime(diaryDay.StartTime);
        var diaryEndDateTime = day.ToDateTime(diaryDay.EndTime);
        var endTime = day.ToDateTime(slotEnd);
        if (endTime > diaryEndDateTime)
            endTime = diaryEndDateTime;
        var startDate = day.ToDateTime(slotStart);

        var current = startDate;
        if (!AllowedMinutes.Contains(current.Minute))
        {
            var nextAllowed = AllowedMinutes.FirstOrDefault(m => m > current.Minute);
            if (nextAllowed == 0)
                current = current.AddHours(1);
            else
                current = new DateTime(current.Year, current.Month, current.Day, current.Hour, nextAllowed, 0);
        }

        while (current <= endTime)
        {
            var endTimeCandidate = current.AddMinutes(minutes);
            if (current < diaryStartDateTime || endTimeCandidate > diaryEndDateTime)
            {
                current = current.AddMinutes(15);
                continue;
            }
            bool clash = items.Any(y =>
                y.StartTime < endTimeCandidate && y.EndTime > current);
            if (!clash && endTimeCandidate <= endTime)
                return new AvailableSlotDto { Start = current, End = endTimeCandidate };
            current = current.AddMinutes(15);
        }
        return null;
    }
}
