#tool nuget:?package=NUnit.ConsoleRunner&version=3.11.1
#load build/tools.cake
using System.Runtime.InteropServices;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var releaseNumber = Argument("release-number", "");

DotNetCoreCleanSettings cleanSettings = null;
DotNetCoreBuildSettings buildSettings = null;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("DotnetSettings")
    .Does(() =>
{
    if (cleanSettings != null && buildSettings != null)
        return;
    if (configuration != "Debug" && configuration != "Release") {
        throw new Exception("Invalid build configuration, valid configurations are: Debug, Release");
    }
    cleanSettings = new DotNetCoreCleanSettings {
        Framework = "netcoreapp3.0",
        Configuration = configuration
    };
    buildSettings = new DotNetCoreBuildSettings {
        Framework = "netcoreapp3.0",
        Configuration = configuration
    };
});
Task("CleanSrc")
    .IsDependentOn("DotnetSettings")
    .Does(() =>
{
    Information("Cleaning Src");
    DotNetCoreClean("src/src.csproj", cleanSettings);
    if (DirectoryExists("build/bin/Debug/netcoreapp3.0/wwwroot")) {
        DeleteDirectory("build/bin/Debug/netcoreapp3.0/wwwroot", new DeleteDirectorySettings {
            Recursive = true
        });
    }
});
Task("CleanBranchConfigurator")
    .IsDependentOn("CleanSrc")
    .Does(() =>
{
    Information("Cleaning Branch Configurator");
    DotNetCoreClean("build/BranchConfigurator.csproj", cleanSettings);
});
Task("CleanScheduler")
    .IsDependentOn("CleanSrc")
    .Does(() =>
{
    Information("Cleaning Scheduler");
    DotNetCoreClean("build/Scheduler.csproj", cleanSettings);
});
Task("CleanServer")
    .IsDependentOn("CleanSrc")
    .Does(() =>
{
    Information("Cleaning Server");
    DotNetCoreClean("build/Server.csproj", cleanSettings);
});
Task("Clean")
    .IsDependentOn("CleanSrc")
    .Does(() =>
{
    RunTarget("CleanServer");
    RunTarget("CleanScheduler");
    RunTarget("CleanBranchConfigurator");
});

Task("BuildSrc")
    .IsDependentOn("DotnetSettings")
    .Does(() =>
{
    Information("Building source");
    DotNetCoreBuild("src/src.csproj", buildSettings);
    if (DirectoryExists($"build/bin/{configuration}/netcoreapp3.0/wwwroot")) {
        DeleteDirectory($"build/bin/{configuration}/netcoreapp3.0/wwwroot", new DeleteDirectorySettings {
            Recursive = true
        });
    }
    CopyDirectory("src/wwwroot/", $"build/bin/{configuration}/netcoreapp3.0/wwwroot/");
    if (configuration == "Debug") {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            CopyFile("src/lib/libpidexists.dylib", $"build/bin/{configuration}/netcoreapp3.0/libpidexists.dylib");
        else
            CopyFile("src/lib/libpidexists.so", $"build/bin/{configuration}/netcoreapp3.0/libpidexists.so");
    }
    var files = GetFiles("src/Services/*");
    foreach(var file in files)
    {
        CopyFile(file, $"build/bin/{configuration}/netcoreapp3.0/{file.GetFilename()}");
    }
});
Task("BuildServer")
    .IsDependentOn("BuildSrc")
    .Does(() =>
{
    Information("Building Server");
    DotNetCoreBuild("build/Server.csproj", buildSettings);
});
Task("BuildScheduler")
    .IsDependentOn("BuildSrc")
    .Does(() =>
{
    Information("Building Scheduler");
    DotNetCoreBuild("build/Scheduler.csproj", buildSettings);
});
Task("BuildBranchConfigurator")
    .IsDependentOn("BuildSrc")
    .Does(() =>
{
    Information("Building Branch Configurator");
    DotNetCoreBuild("build/BranchConfigurator.csproj", buildSettings);
});

Task("BuildAll")
    .IsDependentOn("BuildSrc")
    .Does(() =>
{
    Information("Building all targets");
    RunTarget("BuildServer");
    RunTarget("BuildScheduler");
    RunTarget("BuildBranchConfigurator");
});

Task("Release")
    .Does(() => {
        if (releaseNumber == "")
            throw new Exception("Cannot create release, no release number was given");
        if (!IsValidReleaseNumber(releaseNumber))
            throw new Exception("Invalid release number, should be of the form {major}.{minor}");
        configuration = "Release";
        RunTarget("DotnetSettings");
        RunTarget("BuildServer");
        RunTarget("BuildScheduler");
        var buildDirectory = $"build/bin/{configuration}/netcoreapp3.0/";
        var outputZip = $"build/ChristmasPi.{releaseNumber}.zip";
        Zip(buildDirectory, outputZip);
        Information($"Created release {releaseNumber} at {outputZip}");
});

// Just skeleton code, not currently used
Task("TestSrc")
    .IsDependentOn("BuildSrc")
    .Does(() =>
{
    NUnit3(
        "./src/**/bin/" + configuration + "/*.Tests.dll",
        new NUnit3Settings
        {
            NoResults = true
        });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("BuildAll");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
Information("Running target");
init();
RunTarget(target);
