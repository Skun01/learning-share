using Domain.Enums;

namespace Domain.Constants;

public static class SRSIntervals
{
    /// <summary>
    /// SRS intervals for each level (Bunpro style)
    /// </summary>
    public static readonly Dictionary<SRSLevel, TimeSpan> Intervals = new()
    {
        { SRSLevel.New, TimeSpan.Zero },
        { SRSLevel.Streak1, TimeSpan.FromHours(4) },      // 4 giờ
        { SRSLevel.Streak2, TimeSpan.FromHours(8) },      // 8 giờ
        { SRSLevel.Streak3, TimeSpan.FromDays(1) },       // 1 ngày
        { SRSLevel.Streak4, TimeSpan.FromDays(2) },       // 2 ngày
        { SRSLevel.Streak5, TimeSpan.FromDays(4) },       // 4 ngày
        { SRSLevel.Streak6, TimeSpan.FromDays(8) },       // 8 ngày
        { SRSLevel.Streak7, TimeSpan.FromDays(14) },      // 2 tuần
        { SRSLevel.Streak8, TimeSpan.FromDays(30) },      // 1 tháng
        { SRSLevel.Streak9, TimeSpan.FromDays(60) },      // 2 tháng
        { SRSLevel.Streak10, TimeSpan.FromDays(120) },    // 4 tháng
        { SRSLevel.Streak11, TimeSpan.FromDays(180) },    // 6 tháng
        { SRSLevel.Burned, TimeSpan.FromDays(365 * 20) }  // 20 năm (Burned)
    };

    /// <summary>
    /// Get next review date based on level
    /// </summary>
    public static DateTime GetNextReviewDate(SRSLevel level)
    {
        return DateTime.UtcNow.Add(Intervals[level]);
    }
}
