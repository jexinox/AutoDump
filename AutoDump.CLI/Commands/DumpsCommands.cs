using ConsoleAppFramework;
using Refit;

namespace AutoDump.CLI.Commands;

[RegisterCommands("reports")]
public class DumpsCommands
{
    private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    
    public async Task Get(string appLocator, string? reportsFolder = null)
    {
        var url = SettingsStorage.GetServerUrl();
        if (url is null)
        {
            Console.WriteLine("Please specify server url with \"server set\" command");
            return;
        }

        var serverClient = RestService.For<IAutoDumpServerClient>(url);
        var metadatas = await serverClient.SearchMetadatas(appLocator);
        for (var i = 0; i < metadatas.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {metadatas[i]}");
        }
        
        Console.Write("Please choose metadata: ");
        var metaNum = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
        var meta = metadatas[metaNum];
        var reports = await serverClient.GetReports(meta.DumpId.Value);
        var html = 
"""
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Memory Report</title>
  <style>
      body {
          font-family: Arial, sans-serif;
          margin: 20px;
          line-height: 1.6;
      }
      h1, h2 {
          color: #2c3e50;
      }
      table {
          border-collapse: collapse;
          width: 100%;
          margin-bottom: 20px;
      }
      th, td {
          border: 1px solid #ddd;
          padding: 8px;
          text-align: left;
      }
      th {
          background-color: #f2f2f2;
      }
      .exception {
          background-color: #ffeeee;
          padding: 10px;
          margin-bottom: 10px;
          border-left: 4px solid #ff5252;
      }
  </style>
</head>
<body>
  <h1>Memory Report</h1>
  
  <h2>Types Top-10 by Size</h2>
  <table>
      <thead>
          <tr>
              <th>Type</th>
              <th>Size</th>
          </tr>
      </thead>
      <tbody>
          <typesBySizes>
      </tbody>
  </table>
  
  <h2>Types Top-10 by Retained Size</h2>
  <table>
      <thead>
          <tr>
              <th>Type</th>
              <th>Retained Size</th>
          </tr>
      </thead>
      <tbody>
          <typesByRetainedSizes>
      </tbody>
  </table>
  
  <h2>Boxed Types Top-10 by Size</h2>
  <table>
      <thead>
          <tr>
              <th>Boxed Type</th>
              <th>Size</th>
          </tr>
      </thead>
      <tbody>
          <boxedTypes>
      </tbody>
  </table>
  
  <h2>Generation Sizes</h2>
  <table>
      <thead>
          <tr>
              <th>Generation</th>
              <th>Total Size</th>
              <th>Used Size</th>
          </tr>
      </thead>
      <tbody>
          <generationsSizes>
      </tbody>
  </table>
  
  <h2>Unhandled Exceptions</h2>
  <div>
      <exceptions>
  </div>
</body>
</html>
""";
        for (var i = 0; i < reports.Length; i++)
        {
            var report = reports[i];
            var outHtml = html
                .Replace("<typesBySizes>", 
                    string.Join("", 
                        report.TypesTopBySize.Select(pair => $"<tr><td>{pair.Type}</td><td>{pair.Size}</td></tr>")))
                .Replace("<typesByRetainedSizes>", 
                    string.Join("", 
                        report.TypesTopByRetainedSize.Select(pair => $"<tr><td>{pair.Type}</td><td>{pair.Size}</td></tr>")))
                .Replace("<boxedTypes>", 
                    string.Join("", 
                        report.BoxedTypesTopBySize.Select(pair => $"<tr><td>{pair.Type}</td><td>{pair.Size}</td></tr>")))
                .Replace("<generationsSizes>", 
                    string.Join("", 
                        report.GenerationsSizes.Select(size => $"<tr><td>{size.Generation}</td><td>{size.TotalSize}</td><td>{size.UsedSize}</td></tr>")))
                .Replace("<exceptions>", 
                    string.Join("", 
                        report.Exceptions.Select(exception => $"<div class=\"exception\"><h3>Thread ID: {exception.ManagedThreadId}</h3><pre>{exception.StackTrace}</pre></div>")));
            var reportsPath = reportsFolder ?? DesktopPath + "/autodump-reports";
            if (!Directory.Exists(reportsPath))
            {
                Directory.CreateDirectory(reportsPath);
            }

            var time = DateTime.Now;
            await File.WriteAllTextAsync(reportsPath + $"/Report_{appLocator}_{time}_{i + 1}.html", outHtml);
        }
    }
}