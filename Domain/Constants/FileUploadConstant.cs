namespace Domain.Constants;

public static class FileUploadConstant
{
    public static readonly List<string> ALLOW_IMAGE_EXTENSIONS = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    public static readonly List<string> ALLOW_AUDIO_EXTENSIONS = [".mp3", ".wav", ".ogg", ".m4a"];
    public const int MAX_STORAGE = 2 * 1024 * 1024;
    public const string AVATAR_FOLDER = "avatars";
    public const string IMAGE_FOLDER = "images";
    public const string AUDIO_FOLDER = "audio";
}
