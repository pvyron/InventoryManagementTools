namespace InMa.Shopping.DomainExtensions;

public static class LongExtensions
{
    static readonly string FormatTemplate = "{0:0.00} {1}";
    static readonly string[] Units = ["Bytes", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];
    
    public static string BytesAsText(this long fileSizeBytes)
    {
        if (fileSizeBytes <= 0)
        {
            return string.Format(FormatTemplate, 0, Units[0]);
        }
        
        var index = Math.Log(fileSizeBytes, 1024);
        
        var indexNorm = index > Units.Length ? Units.Length - 1 : (int)index;
        
        var value = fileSizeBytes / Math.Pow(1024, indexNorm);
        
        return string.Format(FormatTemplate, value, Units[indexNorm]);
    }
}