using System.Diagnostics;
using System.Reflection;

namespace Guts.Tests;

/// <summary>
/// Locates, builds, runs and inspects the student-created 'Bank.UI.Tests' project.
/// The project is intentionally NOT referenced at compile time, since students must create it themselves.
/// </summary>
internal static class TestProjectLocator
{
    private const string TestProjectName = "Bank.UI.Tests";
    private const string SolutionFileName = "Bank.slnx";

    private static readonly object CommandLock = new object();
    private static readonly Lazy<string> SolutionDirectoryLazy = new(FindSolutionDirectory);
    private static readonly Lazy<bool> BuildLazy = new(BuildTestProjectOnce);
    private static readonly Lazy<Assembly> TestAssemblyLazy = new(LoadTestAssembly);
    private static readonly Lazy<string> TrxResultsPathLazy = new(RunTestsAndGetTrxPath);
    private static readonly Lazy<string> CoberturaPathLazy = new(CollectCoverageAndGetCoberturaPath);

    public static string SolutionDirectory => SolutionDirectoryLazy.Value;
    public static string TestProjectDirectory => Path.Combine(SolutionDirectory, TestProjectName);
    public static string? TestProjectFile => File.Exists(Path.Combine(TestProjectDirectory, $"{TestProjectName}.csproj"))
        ? Path.Combine(TestProjectDirectory, $"{TestProjectName}.csproj")
        : null;

    public static bool TestProjectExists => TestProjectFile is not null;

    public static Assembly TestAssembly => TestAssemblyLazy.Value;

    public static string TrxResultsPath => TrxResultsPathLazy.Value;

    /// <summary>Path to a cobertura.xml report, computed once and cached for the whole test run.</summary>
    public static string CoberturaPath => CoberturaPathLazy.Value;

    private static string FindSolutionDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && directory.GetFiles(SolutionFileName).Length == 0)
        {
            directory = directory.Parent;
        }

        if (directory is null)
        {
            throw new InvalidOperationException($"Could not locate the '{SolutionFileName}' solution file starting from '{AppContext.BaseDirectory}'.");
        }

        return directory.FullName;
    }

    private static bool BuildTestProjectOnce()
    {
        if (!TestProjectExists)
        {
            throw new InvalidOperationException(
                $"The '{TestProjectName}' project was not found in '{SolutionDirectory}'. " +
                $"Add a '{TestProjectName}' class library project to the solution first.");
        }

        string relativeProjectFile = Path.Combine(TestProjectName, $"{TestProjectName}.csproj");
        var (exitCode, output) = RunDotnet($"build \"{relativeProjectFile}\" --configuration Debug");
        if (exitCode != 0)
        {
            throw new InvalidOperationException($"Building '{TestProjectName}' failed:{Environment.NewLine}{output}");
        }

        return true;
    }

    private static Assembly LoadTestAssembly()
    {
        _ = BuildLazy.Value;

        string? assemblyPath = Directory
            .EnumerateFiles(TestProjectDirectory, $"{TestProjectName}.dll", SearchOption.AllDirectories)
            .Where(path => path.Contains(Path.Combine("bin", "Debug"), StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .FirstOrDefault();

        if (assemblyPath is null)
        {
            throw new InvalidOperationException($"Could not find a compiled '{TestProjectName}.dll' after building the project.");
        }

        return Assembly.LoadFrom(assemblyPath);
    }

    private static string RunTestsAndGetTrxPath()
    {
        _ = BuildLazy.Value;

        string relativeProjectFile = Path.Combine(TestProjectName, $"{TestProjectName}.csproj");
        string resultsDirectory = Path.Combine(Path.GetTempPath(), "GutsTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(resultsDirectory);
        string trxFilePath = Path.Combine(resultsDirectory, "results.trx");

        var (_, output) = RunDotnet(
            $"test \"{relativeProjectFile}\" --configuration Debug --no-build --results-directory \"{resultsDirectory}\" --report-xunit-trx --report-xunit-trx-filename \"results.trx\"");

        string? trxPath = File.Exists(trxFilePath)
            ? trxFilePath
            : Directory.EnumerateFiles(resultsDirectory, "*.trx", SearchOption.AllDirectories).FirstOrDefault();

        if (trxPath is null)
        {
            string testProjectBin = Path.Combine(TestProjectDirectory, "bin");
            if (Directory.Exists(testProjectBin))
            {
                trxPath = Directory.EnumerateFiles(testProjectBin, "*.trx", SearchOption.AllDirectories)
                    .OrderByDescending(File.GetLastWriteTimeUtc)
                    .FirstOrDefault();
            }
        }

        if (trxPath is null)
        {
            throw new InvalidOperationException($"Running 'dotnet test' on '{TestProjectName}' did not produce a .trx result file.{Environment.NewLine}{output}");
        }

        return trxPath;
    }

    private static string CollectCoverageAndGetCoberturaPath()
    {
        _ = BuildLazy.Value;

        EnsureCoverageToolAvailable();

        // Use a path relative to SolutionDirectory (which has no spaces) so nested quoting in 'dotnet-coverage collect' never breaks on user folders with spaces.
        string relativeProjectFile = Path.Combine(TestProjectName, $"{TestProjectName}.csproj");
        string resultsDirectory = Path.Combine(Path.GetTempPath(), "GutsTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(resultsDirectory);
        string coberturaPath = Path.Combine(resultsDirectory, "coverage.cobertura.xml");

        var (exitCode, output) = RunDotnet(
            $"tool run dotnet-coverage collect \"dotnet test {relativeProjectFile} --configuration Debug --no-build\" " +
            $"--output \"{coberturaPath}\" --output-format cobertura");

        if (exitCode != 0 || !File.Exists(coberturaPath))
        {
            throw new InvalidOperationException($"Collecting code coverage for '{TestProjectName}' failed:{Environment.NewLine}{output}");
        }

        return coberturaPath;
    }

    private static void EnsureCoverageToolAvailable()
    {
        var (probeExitCode, _) = RunDotnet("tool run dotnet-coverage --version");
        if (probeExitCode == 0)
        {
            return;
        }

        var (restoreExitCode, restoreOutput) = RunDotnet("tool restore");
        if (restoreExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Restoring the local 'dotnet-coverage' tool failed. Check '.config/dotnet-tools.json' and NuGet access:{Environment.NewLine}{restoreOutput}");
        }
    }

    private static (int ExitCode, string Output) RunDotnet(string arguments)
    {
        // Serialize dotnet CLI invocations because xUnit runs test classes in parallel against the same project outputs.
        lock (CommandLock)
        {
            var startInfo = new ProcessStartInfo("dotnet", arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = SolutionDirectory
            };

            using var process = Process.Start(startInfo) ?? throw new InvalidOperationException($"Could not start 'dotnet {arguments}'.");
            Task<string> errorTask = process.StandardError.ReadToEndAsync();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            string error = errorTask.GetAwaiter().GetResult();

            return (process.ExitCode, output + error);
        }
    }
}