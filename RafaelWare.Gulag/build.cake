var target = Argument("target", "AfterBuildTask");

var newVersionNumber = Argument("version", $"{DateTime.Now.Year}.{DateTime.Now.Month}.{DateTime.Now.Day}.{String.Format("{0:D2}", DateTime.Now.Hour)}{String.Format("{0:D2}", DateTime.Now.Minute)}");
var buildConfig = Argument("buildConfig", "Release");
var nuGetOutputDir = Argument("nuGetOutputDir", "./NuGet");

Task("UpdateVersionNumberTask")
    .Does(() =>
    {
        //get all csproj files in the current directory and update their version number
        var csProjFiles = GetFiles("*.csproj");
        foreach (var csProjFile in csProjFiles)
        {
            //Print the current version numbers of this project
            Information($"Project: '{csProjFile}' FileVersion: {XmlPeek(csProjFile, "Project/PropertyGroup/FileVersion")}");
            Information($"Project: '{csProjFile}' AssemblyVersion: {XmlPeek(csProjFile, "Project/PropertyGroup/AssemblyVersion")}");

			//update the project version node
			XmlPoke(csProjFile, "Project/PropertyGroup/Version", newVersionNumber);

            //update the file version node
            XmlPoke(csProjFile, "Project/PropertyGroup/FileVersion", newVersionNumber);

            //update the assembly version node
            XmlPoke(csProjFile, "Project/PropertyGroup/AssemblyVersion", newVersionNumber);
        }

        Information($"Updated FileVersion and AssemblyVersion to: {newVersionNumber}");
    });

Task("CreateNuGetFolderTask")
    .Does(() =>
    {
        CreateDirectory(nuGetOutputDir);

        Information($"NuGet output directory created at: {nuGetOutputDir}");
    });

Task("MakeNuGetPackageTask")
    .IsDependentOn("CreateNuGetFolderTask")
    .Does(() =>
    {
        //Build and pack the current project
        var csProjFiles = GetFiles("*.csproj");
        foreach (var csProjFile in csProjFiles)
        {
            DotNetCorePack(csProjFile.GetDirectory().FullPath, new DotNetCorePackSettings()
            {
                Configuration = buildConfig,
                OutputDirectory = nuGetOutputDir,
                ArgumentCustomization = args => args.Append($"/p:PackageVersion={newVersionNumber}"),
            });
        }

        Information("NuGet package created");
    });

Task("PushNuGetPackageTask")
    .IsDependentOn("MakeNuGetPackageTask")
    .Does(() =>
    {
        //parse the csproj file of each project to get needed values for the NuGet metadata
        var csProjFiles = GetFiles("*.csproj");
        foreach (var csProjFile in csProjFiles)
        {
            var packageFileName = $"{XmlPeek(csProjFile, "Project/PropertyGroup/PackageId")}.{XmlPeek(csProjFile, "Project/PropertyGroup/AssemblyVersion")}.nupkg";
            var packagePath = $"{nuGetOutputDir}/{packageFileName}";

            NuGetPush(packagePath, new NuGetPushSettings 
            {
                Source = "https://rafaelware.pkgs.visualstudio.com/_packaging/RafaelWareToolkit-NugetFeed/nuget/v3/index.json",
                ApiKey = "VSTS",
            });

            Information($"NuGet package {packageFileName} pushed to feed successfully");
        }
    });

Task("AfterBuildTask")
    .IsDependentOn("PushNuGetPackageTask")
    .Does(() => 
    {
        Information("All done :)");
        Information("Good Bye");
    });

RunTarget(target);