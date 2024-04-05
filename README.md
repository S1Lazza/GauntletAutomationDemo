
# Gauntlet Demo - Unreal Engine 5.2

## Software Requirements
- Visual Studio 2019 or 2022 and [following components](https://dev.epicgames.com/documentation/en-us/unreal-engine/setting-up-visual-studio-development-environment-for-cplusplus-projects-in-unreal-engine?application_version=5.0) - 2022 is recommended
- [Android SDK and Java](https://dev.epicgames.com/documentation/en-us/unreal-engine/setting-up-unreal-engine-projects-for-android-development?application_version=5.0) 
- [Unreal Engine 5.2 Source Code](https://github.com/EpicGames/Signup)

## Additional Notes
- The Unreal project setup paragraph has been added for completion and is not needed to follow in case this repo is downloaded.
- The folder *EngineSource* contains the added/modified engine files
- The folder *Scripts* contains batch scripts to run Gauntlet on the target builds and generate the performance graph data

</br>

## Automation Project Setup

To gather and generate graph using the performance data, we are gonna use the Performance Report Tool built-in in Unreal. <br>
[Building the engine](https://github.com/EpicGames/UnrealEngine/tree/5.2) will automatically generate the needed binaries, you can double check if that's the case by looking at the following path *Engine\Binaries\DotNET\CsvTools*, and look for the *PerfreportTool.exe* file.
If you cannot find the binaries at the location specified, manually rebuild the project solution by going to *Engine\Source\Programs\CSVTools*

At this point you need to add a new C# project to the source engine. <br>
Check the [following documentation](https://dev.epicgames.com/documentation/en-us/unreal-engine/create-an-automation-project-in-unreal-engine?application_version=5.2) on how to do that. <br>

Follow the steps listed under the <ins>Create an Automation Project</ins> and <ins>Configure your Automation Project
Adjust Build Settings</ins> paragraphs.<br>

Next, right click your project and select Properties. <br>
Go to the Build tab and enter this for Output path:
<i>../../../Binaries/DotNET/AutomationScripts/ </i>

Open your .csproj file and add the following code:

```
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Development|AnyCPU' ">
    <DefineConstants>$(DefineConstants);TRACE</DefineConstants>
    <Optimize>true</Optimize>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <DefineConstants>$(DefineConstants);DEBUG;TRACE</DefineConstants>
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
  </PropertyGroup>
```
Without this code, VS will complain about an incorrect configuration of the project, setting the automation project configuration to Debug instead of the newly created Develop.

Close Visual Studio, go to your engine's root directory and run GenerateProjectFiles.bat <br>
Now open UE5.sln in VS: your project should be under *UE5/Programs/Automation*

Right click on your project to add some references via Add -> Reference

Click on the Projects tab on the left and check these three projects:
- AutomationUtils.Automation
- Gauntlet.Automation
- UnrealBuildTool

At this point you should have a file equal to the one contained into the folder: EngineSource/AutomationProject/GauntletDemo.Automation.csproj

Now open the .cs file you renamed above.
Check the following file: EngineSource/AutomationProject/PerformanceGraphData.cs <br>
and replace the needed code to match your class name.

You can also look at the example ElementalDemoTest.cs that can be found under Gauntlet.Automation/Unreal/Game/Samples for additional example

## Unreal Project Setup

Now you'll need to pull in Gauntlet as a dependency in your game project.

Edit your game's .uproject file and add this under Plugins:

```
{
"Name": "Gauntlet",
"Enabled": true
}
```

Next, open build.cs and put this in:

```
PrivateDependencyModuleNames.AddRange(new string[] { "Gauntlet" });
```

Now create two new source files in your game, let's call them CustomGauntletController.h and CustomGauntletController.cpp.<br>
The name needs to match the one found in the .cs file
You can put them anywhere you want under the game's Source folder.

Gauntlet is now ready to run.

## Additional Engine Changes

You can now use the .bat scripts contained in the Scripts folder to run Gauntlet on target builds.
Every time a test is run, the performance data will be saved in a folder PerfTests contained in the direcotry of the project.
However by default, the PerfTest tool does only generate graphs for 30 and 60 fps as target.
You'll need to edit the 2 files to add new framerate if needed. It is possible to play around with them to add additional data and compress more data together.
Please refers to the files.

However, to allow to recognize your specific target framerate you'll have to do additional changes, please refer to the file to check how I managed to have the tool targeting quest.

Finally, specifically for Quest 2 there's another trick to 
