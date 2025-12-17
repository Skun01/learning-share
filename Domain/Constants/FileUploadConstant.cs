namespace Domain.Constants;

public static class FileUploadConstant
{
    public static readonly List<string> ALLOW_EXTENSIONS = [".jpg", ".jpeg", ".png", ".gif"];
    public const int MAX_STORAGE = 2 * 1024 * 1024;
    public const string AVATAR_FOLDER = "avatars";
}
