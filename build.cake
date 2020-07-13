#tool nuget:?package=NUnit.ConsoleRunner&version=3.11.1

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

Task("Restore-Packages-src")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("src/src.csproj");
});

Task("Restore-Packages-Scheduler")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("Scheduler.csproj");
});

Task("Restore-Packages-BranchConfigurator")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("BranchConfigurator.csproj");
});

Task("RestoreAll")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-Packages-src")
    .IsDependentOn("Restore-Packages-Scheduler")
    .IsDependentOn("Restore-Packages-BranchConfigurator");

Task("BuildAll")
    .IsDependentOn("RestoreAll")
    .Does(() =>
{
    DotNetCoreBuild("src/src.csproj");
    DotNetCoreBuild("Server.csproj");
    DotNetCoreBuild("Scheduler.csproj");
    DotNetCoreBuild("BranchConfigurator.csproj")
});

Task("Run-UnitTests")
    .IsDependentOn("Build")
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
    .IsDependentOn("Run-UnitTests");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
