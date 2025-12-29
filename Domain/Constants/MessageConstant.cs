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

    public static class UserSettingsMessage
    {
        public const string NOT_SUPPORT_LANGUAGE = "Language_Not_Support_400";
    }

    public static class DeckMessage
    {
        public const string INVALID_DECK_TYPE = "Deck_Invalid_Type_400"; // Loại deck không hợp lệ
        public const string PARENT_DECK_NOT_FOUND = "Deck_Parent_Not_Found_404"; // Deck cha không tồn tại
        public const string PARENT_DECK_PERMISSION_DENIED = "Deck_Parent_Permission_Denied_403"; // Không có quyền tạo deck con
        public const string DECK_NOT_FOUND = "Deck_Not_Found_404"; // Deck không tồn tại
        public const string DECK_PERMISSION_DENIED = "Deck_Permission_Denied_403"; // Không có quyền sửa deck này
    }

    public static class CardMessage
    {
        public const string CARD_NOT_FOUND = "Card_Not_Found_404";
        public const string CARD_INVALID_TYPE = "Card_Invalid_Type_400";
        public const string CARD_PERMISSION_DENIED = "Card_Permission_Denied_403";
        public const string CARD_TYPE_MISMATCH = "Card_Type_Mismatch_400"; // Card type phải trùng với deck type
        public const string EXAMPLE_NOT_FOUND = "Example_Not_Found_404";
    }

    public static class SrsMessage
    {
        public const string CARD_NOT_FOUND = "Srs_Card_Not_Found_404";
        public const string DECK_PERMISSION_DENIED = "Srs_Deck_Permission_Denied_403";
        public const string REVIEW_CORRECT = "Srs_Review_Correct_200";
        public const string REVIEW_INCORRECT = "Srs_Review_Incorrect_200";
        public const string REVIEW_BURNED = "Srs_Review_Burned_200";
    }
}
