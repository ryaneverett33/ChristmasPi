#tool nuget:?package=NUnit.ConsoleRunner&version=3.11.1
#load build/tools.cake

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    var buildDir = Directory("./src/Example/bin") + Directory(configuration);
    CleanDirectory(buildDir);
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
