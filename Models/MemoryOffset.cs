using System;

namespace MGSPW_MC_Cheat_Trainer.Models;

public struct MemoryOffset
{
    public int Start { get; set; }
    public int End { get; set; }
    public int Length { get; set; }

    public MemoryOffset(int offsetStart) : this(offsetStart, offsetStart)
    {
    }

    public MemoryOffset(int offsetStart, int offsetEnd)
    {
        Start = offsetStart;
        End = offsetEnd;
        Length = Math.Abs(offsetEnd - offsetStart) + 1;
    }
}