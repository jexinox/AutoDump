using System.Text;

namespace Guts.CLI;

public class SettingsStorage
{
    private static readonly string SettingsFolderPath =
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/guts-cli";
    private static readonly string UrlFilePath = SettingsFolderPath + "/server-url";
    
    public static string? GetServerUrl()
    {
        return File.Exists(UrlFilePath) 
            ? File.ReadAllText(UrlFilePath, Encoding.UTF8) 
            : null;
    }

    public static void SetServerUrl(string url)
    {
        if (!Directory.Exists(SettingsFolderPath))
        {
            Directory.CreateDirectory(SettingsFolderPath);
        }
        
        File.WriteAllText(UrlFilePath, url, Encoding.UTF8);
    }
}