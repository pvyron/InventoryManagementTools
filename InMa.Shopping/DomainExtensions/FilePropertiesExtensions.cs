using InMa.Shopping.Data.Repositories.Abstractions;
using InMa.Shopping.Data.Repositories.Models;
using InMa.Shopping.ViewModels;

namespace InMa.Shopping.DomainExtensions;

public static class FilePropertiesExtensions
{
    public static FileProperties GetFileUploadProperties(this SharedFileInputProperties inputProperties)
    {
        var nameAsSpan = inputProperties.Name.AsSpan();
        
        var outputProperties = new FileProperties
        {
            FileName = Path.GetFileNameWithoutExtension(nameAsSpan).ToString(),
            FileExtension = Path.GetExtension(nameAsSpan).ToString(),
            OriginalName = inputProperties.OriginalName,
            ContentType = inputProperties.ContentType,
            LastModified = inputProperties.LastModified,
            FileSizeBytes = inputProperties.FileSizeBytes,
        };

        return outputProperties;
    }
}