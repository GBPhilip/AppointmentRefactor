using System;
using System.Collections.Generic;
using System.Threading.Tasks;
public interface IDiarySlotRepository
{
    Task<List<DiarySlot>> GetSlotsForDayAsync(DateOnly day);
}
  