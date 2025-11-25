namespace Refactor.Services
{
    public static class SlotFinder
    {
        public static AvailableSlotDto? FindAvailableSlot(
            int slotLengthRequired,
            DateOnly day,
            TimeOnly slotEarliestStartTime,
            TimeOnly slotLatestStartEndTime,
            DiaryDay diaryDay,
            List<DiarySlot> items)
        {
            var diaryStartDateTime = day.ToDateTime(diaryDay.StartTime);
            var diaryEndDateTime = day.ToDateTime(diaryDay.EndTime);
            var latestSlotEndDateTime = DetermineLatestSlotEndDateTime(day, slotLatestStartEndTime, diaryEndDateTime);
            var startDate = day.ToDateTime(slotEarliestStartTime);

            var slotStartDateTime = DetermineEarliestAllowedStartTime(startDate);

            while (slotStartDateTime <= latestSlotEndDateTime)
            {
                var endTimeCandidate = slotStartDateTime.AddMinutes(slotLengthRequired);
                if (slotStartDateTime < diaryStartDateTime || endTimeCandidate > diaryEndDateTime)
                {
                    slotStartDateTime = slotStartDateTime.AddMinutes(15);
                    continue;
                }
                bool clash = items.Any(y =>
                    y.StartTime < endTimeCandidate && y.EndTime > slotStartDateTime);
                if (!clash && endTimeCandidate <= latestSlotEndDateTime)
                    return new AvailableSlotDto { Start = slotStartDateTime, End = endTimeCandidate };
                slotStartDateTime = slotStartDateTime.AddMinutes(15);
            }
            return null;
        }

        private static DateTime DetermineLatestSlotEndDateTime(DateOnly day, TimeOnly latestSlotStartEndTime, DateTime diaryEndDateTime)
        {
            var slotLatestEndDateTime = day.ToDateTime(latestSlotStartEndTime);
            if (slotLatestEndDateTime > diaryEndDateTime)
                slotLatestEndDateTime = diaryEndDateTime;
            return slotLatestEndDateTime;
        }

        private static DateTime DetermineEarliestAllowedStartTime(DateTime startDate)
        {
            int[] allowedMinutes = { 0, 15, 30, 45 };
            var earliestAllowedStartDateTime = startDate;
            if (!allowedMinutes.Contains(earliestAllowedStartDateTime.Minute))
            {
                var nextAllowed = allowedMinutes.FirstOrDefault(m => m > earliestAllowedStartDateTime.Minute);
                if (nextAllowed == 0)
                    earliestAllowedStartDateTime = earliestAllowedStartDateTime.AddHours(1);
                else
                    earliestAllowedStartDateTime = new DateTime(earliestAllowedStartDateTime.Year, earliestAllowedStartDateTime.Month, earliestAllowedStartDateTime.Day, earliestAllowedStartDateTime.Hour, nextAllowed, 0, DateTimeKind.Unspecified);
            }

            return earliestAllowedStartDateTime;
        }
    }
}