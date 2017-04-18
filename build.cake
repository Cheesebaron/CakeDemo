var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

Task("Clean")
    .Does(() =>
{
    CleanDirectories(new[] { 
        "./artifacts/TestResults",
        "./artifacts/Build/iOS", 
        "./artifacts/Build/Android" 
    });
});

Task("NuGet-Package-Restore")
    .Does(() =>
{
    NuGetRestore("./src/cakedemos.sln");
});

Task("Build-iOS")
    .IsDependentOn("NuGet-Package-Restore")
    .IsDependentOn("Clean")
    .Does(() => 
{
     MSBuild("./src/cakedemos.sln", new MSBuildSettings()
        .SetConfiguration("Debug")
        .WithProperty("Platform", "iPhoneSimulator")
        .WithProperty("BuildIPA", "True")
    );

    MSBuild("./src/cakedemos.sln", new MSBuildSettings()
        .SetConfiguration(configuration)
        .WithProperty("Platform", "iPhone")
        .WithProperty("BuildIPA", "True")
    );

    CopyFile(
        "./src/iOS/bin/iPhone/Release/cakedemos.iOS.ipa", 
        "./artifacts/Build/iOS/cakedemos.iOS.ipa");

    #addin "Cake.Compression"
    #addin nuget:?package=SharpZipLib&version=0.86.0
    ZipCompress(
        "./src/iOS/bin/iPhone/Release/cakedemos.iOS.dSYM",
        "./artifacts/Build/iOS/cakedemos.iOS.dSYM.zip");
});

Task("Build-Android")
    .IsDependentOn("NuGet-Package-Restore")
    .IsDependentOn("Clean")
    .Does(() => 
{
    MSBuild("./src/Droid/cakedemos.Droid.csproj", new MSBuildSettings()
        .SetConfiguration(configuration)
        .WithTarget("SignAndroidPackage")
    );

    CopyFile(
        "./src/Droid/bin/Release/dk.ostebaronen.cakedemos-Signed.apk",
        "./artifacts/Build/Android/cakedemos-Signed.apk");
});

Task("Run-UITests")
    .IsDependentOn("Build-iOS")
    .Does(() => 
{
    #tool nuget:?package=NUnit.Runners&version=2.6.4
    NUnit("./src/UITests/bin/Debug/cakedemos.UITests.dll");
    MoveFile("./TestResult.xml", "./artifacts/TestResults/UITest.xml");
});

Task("Default")
  .IsDependentOn("Build-Android")
  .IsDependentOn("Run-UITests");

RunTarget(target);