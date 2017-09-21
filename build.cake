#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = EnvironmentVariable ("APPVEYOR_BUILD_VERSION") ?? Argument("version", "1.0.0");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/CodeQuasar.Xamarin.Plugins.Location/bin") + Directory(configuration);
var artifactDir = Directory("./Build");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
	CleanDirectory(artifactDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./src/CodeQuasar.Xamarin.Plugins.Location.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./src/CodeQuasar.Xamarin.Plugins.Location.sln", settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild("./src/CodeQuasar.Xamarin.Plugins.Location.sln", settings =>
        settings.SetConfiguration(configuration));
    }
});

Task ("NuGet")
	.IsDependentOn ("Run-Unit-Tests")
	.Does (() =>
{
    if(!DirectoryExists("./Build/nuget/"))
        CreateDirectory("./Build/nuget");
    
	Console.WriteLine($"Using version {version}");
	
	try
	{
		NuGetPack ("./nuget/CodeQuasar.Xamarin.Plugins.Location.nuspec", new NuGetPackSettings { 
			Version = version,
			Verbosity = NuGetVerbosity.Detailed,
			OutputDirectory = "./Build/nuget/"
		});
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
	}
});


Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    // NUnit3("./src/**/bin/" + configuration + "/*.Tests.dll", new NUnit3Settings {
        // NoResults = true
        // });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("NuGet");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
