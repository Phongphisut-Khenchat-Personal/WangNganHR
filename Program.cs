using System.Diagnostics;

var root = Directory.GetCurrentDirectory();
var script = Path.Combine(root, "scripts", "run-web.ps1");

if (!File.Exists(script))
{
    Console.Error.WriteLine("scripts/run-web.ps1 not found. Run from the repo root.");
    return 1;
}

Console.WriteLine("Starting WangNganHR.Web (via scripts/run-web.ps1)...");
Console.WriteLine("API:  dotnet run --project WangNganHR.API");
Console.WriteLine("Desktop: .\\scripts\\run-desktop.ps1");
Console.WriteLine();

using var process = Process.Start(new ProcessStartInfo
{
    FileName = "powershell",
    Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{script}\"",
    WorkingDirectory = root,
    UseShellExecute = false
});

if (process is null)
    return 1;

process.WaitForExit();
return process.ExitCode;
