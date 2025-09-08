using System;
using System.IO;

public static class ExceptionLogger
{
    public static void Log(Exception ex, string path = @"C:\temp\exception.txt")
    {
        try
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.AppendAllText(path, DateTime.UtcNow.ToString("o") + " - " + ex.ToString() + Environment.NewLine + new string('-', 80) + Environment.NewLine);
        }
        catch
        {
            // swallow logging errors to avoid masking original exception
        }
    }
}