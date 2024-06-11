using BenchmarkDotNet.Attributes;

namespace InMa.Benchmarks;

public class Benchmark
{
    public const long value0 = 12422L;
    public const long value1 = 12387612387L;
    public const long value2 = 1237612361298L;
    public const long value3 = 1173639293334345626L;
    
    [Params(value1, value0, value2)]
    public long number;

    [Benchmark]
    public string NewMine()
    {
        return number.NewBytesAsText();
    }
    
    [Benchmark]
    public string NewMine_Optimized()
    {
        return number.NewBytesAsText_Optimized();
    }
}

public static class Ext
{
    #region slower stuff
    // public static string ByteSize(this long size)
    // {
    //
    //     if (size == 0)
    //     {
    //         return string.Format(formatTemplate, 0, sizeSuffixes[0]);
    //     }
    //
    //     var absSize = Math.Abs((double)size);
    //     var fpPower = Math.Log(absSize, 1024);
    //     var intPower = (int)fpPower;
    //     var iUnit = intPower >= sizeSuffixes.Length
    //         ? sizeSuffixes.Length - 1
    //         : intPower;
    //     var normSize = absSize / Math.Pow(1024, iUnit);
    //
    //     return string.Format(
    //         formatTemplate, normSize, sizeSuffixes[iUnit]);
    // }
    
    // private static string[] _units = ["Bytes", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];

    // public static string BytesAsText(this long fileSizeBytes)
    // {
    //
    //     for (int i = 0; i < _units.Length; i++)
    //     {
    //         if (fileSizeBytes / Math.Pow(1024, i + 1) < 1)
    //             return string.Format(
    //                 formatTemplate, fileSizeBytes / Math.Pow(1024, i), _units[i]);
    //     }
    //
    //     return string.Format(
    //         formatTemplate, fileSizeBytes / Math.Pow(1024, _units.Length - 1), _units[^1]);
    // }
    //
    // public static string NewBytesAsText_ReadOnly(this long fileSizeBytes)
    // {
    //     if (fileSizeBytes <= 0)
    //     {
    //         return string.Format(formatTemplate, 0, _units[0]);
    //     }
    //     
    //     var fpPower = Math.Log(fileSizeBytes, 1024);
    //     var index = fpPower > _units.Length ? _units.Length - 1 : (int)fpPower;
    //     
    //     var value = fileSizeBytes / Math.Pow(1024, index);
    //     
    //     return string.Format(formatTemplate, value, _units[index]);
    // }
    #endregion

    private static string[] sizeSuffixes = ["Bytes", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];
    
    private const string formatTemplate = "{0:0.00} {1}";
    
    public static string NewBytesAsText(this long fileSizeBytes)
    {
        if (fileSizeBytes <= 0)
        {
            return string.Format(formatTemplate, 0, sizeSuffixes[0]);
        }
        
        var index = Math.Log(fileSizeBytes, 1024);
        
        var indexNorm = index > sizeSuffixes.Length ? sizeSuffixes.Length - 1 : (int)index;
        
        var value = fileSizeBytes / Math.Pow(1024, indexNorm);
        
        return string.Format(formatTemplate, value, sizeSuffixes[indexNorm]);
    }
    
    public static string NewBytesAsText_Optimized(this long fileSizeBytes)
    {
        if (fileSizeBytes <= 0)
        {
            return string.Format(formatTemplate, 0, sizeSuffixes[0]);
        }
        
        var index = (int)Math.Log(fileSizeBytes, 1024);
        
        var value = fileSizeBytes / Math.Pow(1024, index);
        
        return string.Format(formatTemplate, value, sizeSuffixes[index]);
    }
}