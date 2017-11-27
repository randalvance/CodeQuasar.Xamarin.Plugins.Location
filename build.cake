#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = GetVersion();

Information($"Version to use is {version}");

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
	
	NuGetPack ("./nuget/CodeQuasar.Xamarin.Plugins.Location.nuspec", new NuGetPackSettings { 
		Version = version,
		Verbosity = NuGetVerbosity.Detailed,
		OutputDirectory = "./Build/nuget/"
	});
});

Task ("DeployToNuget")
	.IsDependentOn("NuGet")
	.Does(() =>
{
	var source = EnvironmentVariable ("NUGET_SOURCE");
	var token = EnvironmentVariable ("NUGET_TOKEN");
	var packages = GetFiles(artifactDir.ToString() + "/nuget/*.nupkg");
	
	NuGetPush(packages, new NuGetPushSettings { Source = source, ApiKey = token });
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
    .IsDependentOn("DeployToNuget");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

//////////////////////////////////////////////////////////////////////
// Utilities
//////////////////////////////////////////////////////////////////////
string GetVersion()
{
	var buildNumberString = EnvironmentVariable ("APPVEYOR_BUILD_VERSION") ?? Argument("version", "1.0.1-alpha-2");
	var versionRegex = @"(\d+\.\d+\.(\d+))((\-[A-Za-z]+\-)(\d+))*";
	var regex = new System.Text.RegularExpressions.Regex(versionRegex);
	var match = regex.Match(buildNumberString);
	var groups = match.Groups.Cast<System.Text.RegularExpressions.Group>().Where(g => !string.IsNullOrEmpty(g.Value)).ToList();
	
	if (groups.Count > 3)
	{
		var semVer = groups[1];
		var suffix = groups[4];
		var buildNumber = groups[5];
		var version = $"{semVer}{suffix}{buildNumber.Value.PadLeft(4, '0')}";
		
		return version;
	}
	
	return buildNumberString;
}