#tool nuget:?package=NUnit.ConsoleRunner&version=3.11.1
#load build/tools.cake
using System.Runtime.InteropServices;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("CleanSrc")
    .Does(() =>
{
    Information("Cleaning Src");
    DotNetCoreClean("src/src.csproj");
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
    DotNetCoreClean("build/BranchConfigurator.csproj");
});
Task("CleanScheduler")
    .IsDependentOn("CleanSrc")
    .Does(() =>
{
    Information("Cleaning Scheduler");
    DotNetCoreClean("build/Scheduler.csproj");
});
Task("CleanServer")
    .IsDependentOn("CleanSrc")
    .Does(() =>
{
    Information("Cleaning Server");
    DotNetCoreClean("build/Server.csproj");
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
    .Does(() =>
{
    Information("Building source");
    DotNetCoreBuild("src/src.csproj");
    if (DirectoryExists("build/bin/Debug/netcoreapp3.0/wwwroot")) {
        DeleteDirectory("build/bin/Debug/netcoreapp3.0/wwwroot", new DeleteDirectorySettings {
            Recursive = true
        });
    }
    CopyDirectory("src/wwwroot/", "build/bin/Debug/netcoreapp3.0/wwwroot/");
    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        CopyFile("src/lib/libpidexists.dylib", "build/bin/Debug/netcoreapp3.0/libpidexists.dylib");
    else
        CopyFile("src/lib/libpidexists.so", "build/bin/Debug/netcoreapp3.0/libpidexists.so");
});
Task("BuildServer")
    .IsDependentOn("BuildSrc")
    .Does(() =>
{
    Information("Building Server");
    DotNetCoreBuild("build/Server.csproj");
});
Task("BuildScheduler")
    .IsDependentOn("BuildSrc")
    .Does(() =>
{
    Information("Building Scheduler");
    DotNetCoreBuild("build/Scheduler.csproj");
});
Task("BuildBranchConfigurator")
    .IsDependentOn("BuildSrc")
    .Does(() =>
{
    Information("Building Branch Configurator");
    DotNetCoreBuild("build/BranchConfigurator.csproj");
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
