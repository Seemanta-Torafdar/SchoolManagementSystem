namespace SchoolManagement.BLL.Helpers;

public static class FileValidationHelper
{
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private static readonly string[] AllowedDocumentExtensions = { ".pdf" };

    public static bool IsValidImage(string fileName, long fileSize, out string errorMessage)
    {
        errorMessage = string.Empty;
        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !AllowedImageExtensions.Contains(ext))
        {
            errorMessage = "Invalid image type. Allowed formats: JPG, JPEG, PNG, WEBP.";
            return false;
        }

        if (fileSize > 5 * 1024 * 1024) // 5MB
        {
            errorMessage = "Image file size exceeds the 5MB limit.";
            return false;
        }

        return true;
    }

    public static bool IsValidDocument(string fileName, long fileSize, out string errorMessage)
    {
        errorMessage = string.Empty;
        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !AllowedDocumentExtensions.Contains(ext))
        {
            errorMessage = "Invalid document format. Only PDF files are allowed.";
            return false;
        }

        if (fileSize > 10 * 1024 * 1024) // 10MB
        {
            errorMessage = "Document file size exceeds the 10MB limit.";
            return false;
        }

        return true;
    }

    public static string GenerateUniqueFileName(string originalFileName)
    {
        var ext = Path.GetExtension(originalFileName).ToLowerInvariant();
        return $"{Guid.NewGuid():N}_{DateTime.UtcNow.Ticks}{ext}";
    }
}

