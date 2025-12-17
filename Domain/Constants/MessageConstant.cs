namespace Domain.Constants;

public static class MessageConstant
{
    public static class CommonMessage
    {
        public const string INTERNAL_SERVER_ERROR = "Common_505";   // Lỗi Server
        public const string NOT_FOUND = "Common_404"; // không tìm thấy
        public const string INVALID = "Common_400"; // không hợp lệ
    }

    public static class UserMessage
    {
        public const string EMAIL_ALREADY_EXISTS = "User_Email_409"; // email đã tồn tại
        public const string USERNAME_ALREADY_EXISTSS = "Username_409"; // username đã tồn tại
    }

    public static class AuthMessage
    {
        public const string INVALID_LOGIN = "Invalid_400";  // Tài khoản hoặc mật khẩu không đúng
        public const string RESET_PASSWORD_TOKEN_EXPIRED = "Reset_Password_Token_Expired_400"; // reset password token hết hạn
        public const string CURRENT_PASSWORD_INVALID = "Current_Password_Invalid_400";
    }

    public static class FileUploadMessage
    {
        public const string INVALID_EXTENSION = "INVALID_EXTENSION_400";
        public const string STORAGE_EXCEED = "STORAGE_EXCEED_400";
    }
}
