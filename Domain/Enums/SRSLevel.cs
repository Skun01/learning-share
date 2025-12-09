namespace Domain.Enums;

public enum SRSLevel
{
    New = 0,             // Mới tinh (Chưa học)

    // Begginer
    Streak1 = 1,         // Chờ 4 giờ
    Streak2 = 2,         // Chờ 8 giờ
    Streak3 = 3,         // Chờ 24 giờ (1 ngày)

    // Adept
    Streak4 = 4,         // Chờ 2 ngày
    Streak5 = 5,         // Chờ 4 ngày
    Streak6 = 6,         // Chờ 8 ngày

    // Seasoned
    Streak7 = 7,         // Chờ 2 tuần (14 ngày)
    Streak8 = 8,         // Chờ 1 tháng
    Streak9 = 9,         // Chờ 2 tháng

    // Exert
    Streak10 = 10,       // Chờ 4 tháng
    Streak11 = 11,       // Chờ 6 tháng

    // Master
    Burned = 12          // Không bao giờ hiện lại
}
