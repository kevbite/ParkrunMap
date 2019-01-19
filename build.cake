var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var sln = "./ParkrunMap.sln";
var artifactsDirectory = DirectoryPath.FromString(EnvironmentVariable("BUILD_ARTIFACTSTAGINGDIRECTORY") ?? "./artifacts/");

Task("Clean")
    .Does(() =>
{
    var settings = new DotNetCoreCleanSettings
    {
        Configuration = configuration
    };
    
    DotNetCoreClean(sln, settings);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore(sln, new DotNetCoreRestoreSettings ());
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetCoreBuild(sln, new DotNetCoreBuildSettings
    {
        Configuration = configuration
    });
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration,
        Logger = "trx",
    };

    var projectFiles = GetFiles("./test/**/*.csproj");
    foreach(var file in projectFiles)
    {
        DotNetCoreTest(file.FullPath, settings);
    }
});

Task("Pack")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
     var settings = new DotNetCorePackSettings
     {
        Configuration = configuration,
        OutputDirectory = artifactsDirectory,
        NoBuild = true
     };

     DotNetCorePack(sln, settings);
});

Task("Zip-FunctionsApp")
    .IsDependentOn("Pack")
    .Does(() =>
{
    var publishPath = "./src/ParkrunMap.FunctionsApp/bin/Release/netstandard2.0/publish/";
    var zipArtifact = artifactsDirectory.GetFilePath("./ParkrunMap.FunctionsApp.zip");
    Information($"Zipping {publishPath} to {zipArtifact}");

    Zip(publishPath, zipArtifact);
});

Task("Default")
    .IsDependentOn("Zip-FunctionsApp");

RunTarget(target);