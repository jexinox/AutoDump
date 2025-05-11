using ConsoleAppFramework;

namespace Guts.CLI.Commands;

[RegisterCommands("server")]
public class ServerCommands
{
    public void Set(string url)
    {
        SettingsStorage.SetServerUrl(url);
    }

    public void Get()
    {
        Console.WriteLine(SettingsStorage.GetServerUrl());
    }
}