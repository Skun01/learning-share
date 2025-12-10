namespace Domain.Constants;

public static class MessageConstant
{
    public static class CommonMessage
    {
        public const string INTERNAL_SERVER_ERROR = "Common_505";   // Lỗi Server
        public const string NOT_FOUND = "Common_404"; // không tìm thấy
    }

    public static class UserMessage
    {
        public const string EMAIL_ALREADY_EXISTS = "User_Email_409"; // email đã tồn tại
    }

    public static class AuthMessage
    {
        public const string INVALID_LOGIN = "INVALID_400";
    }
}
