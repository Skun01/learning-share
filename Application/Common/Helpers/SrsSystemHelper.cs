using Domain.Enums;

namespace Application.Common.Helpers;

public static class SrsSystemHelper
{
    public static TimeSpan GetInterval(SRSLevel level)
    {
        return level switch
        {
            SRSLevel.New => TimeSpan.Zero,
            
            SRSLevel.Streak1 => TimeSpan.FromHours(4),
            SRSLevel.Streak2 => TimeSpan.FromHours(8),
            SRSLevel.Streak3 => TimeSpan.FromDays(1),
            
            SRSLevel.Streak4 => TimeSpan.FromDays(2),
            SRSLevel.Streak5 => TimeSpan.FromDays(4),
            SRSLevel.Streak6 => TimeSpan.FromDays(8),
            
            SRSLevel.Streak7 => TimeSpan.FromDays(14),
            SRSLevel.Streak8 => TimeSpan.FromDays(30), // 1 tháng
            SRSLevel.Streak9 => TimeSpan.FromDays(60), // 2 tháng
            
            SRSLevel.Streak10 => TimeSpan.FromDays(120), // 4 tháng
            SRSLevel.Streak11 => TimeSpan.FromDays(180), // 6 tháng
            
            SRSLevel.Burned => TimeSpan.MaxValue,
            
            _ => TimeSpan.FromHours(4) 
        };
    }

    // Logic tụt hạng khi trả lời SAI
    public static SRSLevel CalculatePenalty(SRSLevel currentLevel)
    {
        if (currentLevel <= SRSLevel.Streak4) return SRSLevel.Streak1;
        if (currentLevel <= SRSLevel.Streak7) return SRSLevel.Streak3; 
        return SRSLevel.Streak5;
    }
}
