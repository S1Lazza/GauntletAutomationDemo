
# Gauntlet Demo - Unreal Engine 5.2

## Software Requirements
- Visual Studio 2019 or 2022 and [following components](https://dev.epicgames.com/documentation/en-us/unreal-engine/setting-up-visual-studio-development-environment-for-cplusplus-projects-in-unreal-engine?application_version=5.0) - 2022 is recommended
- [Android SDK and Java](https://dev.epicgames.com/documentation/en-us/unreal-engine/setting-up-unreal-engine-projects-for-android-development?application_version=5.0) 
- [Unreal Engine 5.2 Source Code](https://github.com/EpicGames/Signup)

</br>

## Important Notes
- The <ins>Unreal Project Setup</ins> section has been included to provide a complete explanation, but can be skipped, as it is already integrated into the project repository.
- The folder **EngineSource** contains the added/modified engine files required for the Gauntlet setup.
- The folder **Scripts** contains multiple batch scripts to run Gauntlet on target builds and generate the performance graph data.

</br>

## Automation Project Setup

To gather the performance data and generate graphs, we will use the built-in **Performance Report Tool** in Unreal Engine. <br>
[Building the engine](https://github.com/EpicGames/UnrealEngine/tree/5.2) will automatically generate the needed binaries to allow us to use it. <br> 
You can verify their presence by navigating to *Engine\Binaries\DotNET\CsvTools* and checking for the **PerfreportTool.exe** file. <br>
If the binaries are not found, you need to manually rebuild the project solution located at *Engine\Source\Programs\CSVTools*.

Next, you'll need to add a new C# project to the source engine. <br>
Refer to the [following documentation](https://dev.epicgames.com/documentation/en-us/unreal-engine/create-an-automation-project-in-unreal-engine?application_version=5.2) for instructions on how to do this. <br>
Follow the steps outlined under the <ins>Create an Automation Project</ins> and <ins>Configure your Automation Project Adjust Build Settings</ins> sections.<br>

Proceed by right-clicking on your recently created project in the **Solution Explorer** and choose **Properties**. <br> 
Navigate to the **Build** tab, under the **Ouput** section, and input the following in the **Base output path** field: <i>../../../Binaries/DotNET/AutomationScripts/</i>

Then, double-click on the project to open the **.csproj** file and include the following code:

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
This code is required to prevent Visual Studio from encountering configuration errors and defaulting the automation project configuration to Debug instead of the desired **Develop**.

Once this is done, close Visual Studio, navigate to your engine's root directory, and execute the  **GenerateProjectFiles.bat**, then, reopen the **UE5.sln**. <br>
Your project should now be visible under *UE5/Programs/Automation* in the Solution Explorer.

Next, right-click on your project and navigate to *Add->Reference*. <br> 
Switch to the **Projects** tab on the left and select the following three projects:

- AutomationUtils.Automation
- Gauntlet.Automation
- UnrealBuildTool

By following these steps, you should have a file identical to the one found in the folder *EngineSourceChanges/CustomAutomationProject/GauntletDemo.Automation.csproj*

Now, open the **.cs** project file that was renamed in step 6 of the <ins>Create an Automation Project</ins> section. <br> 
Compare it with the file located at *EngineSource/AutomationProject/PerformanceGraphData.cs* and update the code to match with your class name.

For further guidance, refer to the examples provided located at *Gauntlet.Automation/Unreal/Game/Samples* in the **Solution Explorer**, with particular attention to the **ElementalDemoTest.cs** one.

</br>

## Unreal Project Setup

Before starting, make sure to associate the project with the source engine by right-clicking on the .uproject file and select **Switch Unreal Engine Version**. <br>
To integrate Gauntlet in your game project:

- Edit your game's **.uproject** file and add the following under Plugins:

```
{
    "Name": "Gauntlet",
    "Enabled": true
}
```

- Open the **build.cs** file, found under *Source/ProjectName/* and add the following:

```
PrivateDependencyModuleNames.AddRange(new string[] { "Gauntlet" });
```

- Create a new C++ class inheriting from **GauntletTestController**. <br>
You can place it anywhere within your game's Source folder, just ensure that its name matches the one specified into the **.cs** file for the **GauntletController** variable. <br>
In the example project, the choosen name is **CustomGauntletController**.

With these changes, Gauntlet is now ready to run within your game project.

</br>

## Engine Changes

If you intend to run Gauntlet on Windows builds without further modifications, everything is set up and you can skip the explanation below. <br>
Utilize the **RunGauntletWindowsBuild.bat** file located in the **Scripts** folder to execute Gauntlet on target builds. 

Just be sure to adjust the paths according to your engine's and project's location and name and be sure the **test** command in the scripts matches the name of the class specified in the automation project. 

Each time a test is executed, the performance data will be stored in the folder specified in the **uploaddir** command, open the **.xml** file to visualize them.

### Generate performance graphs for specific target FPS with custom data

The **Performance Report Tool** possess significant capabilities, but it also comes with limitations:
- By default the tool generates graphs only against 30 and 60 fps targets. This can be limiting, especially when dealing with VR development.
- Graphs may be challenging to interpret due to crowded data curves.
- The tool generates an Excel file inside the output directory, within a CSV folder. Upon inspection, you'll find that this file contains more information than what is reported by the graphs with data that are not displayed.

Luckily for us, it is possible to modify these behaviors. <br>

To customize the information and appearance of the graphs, navigate to *Engine\Binaries\DotNET\CsvTools* and examine the **ReportGraphs.xml** and **ReportTypes.xml** files. <br>
Compare them with the versions found in the *EngineSourceChanges\CSVTools* folder. <br>
The **ReportGraphs.xml** file controls the types of graphs available, while the **ReportTypes.xml** file determines the reports generated. Adjust them as needed to suit your requirements.

Editing these files will enable the **Performance Report Tool** to include additional data, but it won't instruct it when to do so. <br>
To achieve that, you'll need to modify an additional engine file: *Engine\Source\Runtime\Core\Private\ProfilingDebugging\CsvProfiler.cpp*. <br>
Refer again to the file contained in *EngineSourceChanges\CSVTools* for guidance, starting from line 2909.

```
// GauntletDemo: Check if the build is running on Meta Quest Devices, by comparing against some platform attributes
// If that's the case, hardcode the target framerate

FString TargetPlatformCPU = "Snapdragon";
FString TargetPlatformName = "Quest";
FString TargetPlatformOS = "Android";

FString PlatformSpec = FPlatformMisc::GetDeviceMakeAndModel();
FString PlatformOS = FPlatformMisc::GetUBTPlatform();

if (PlatformOS.Contains(TargetPlatformOS, ESearchCase::IgnoreCase) &&
   (PlatformSpec.Contains(TargetPlatformName, ESearchCase::IgnoreCase) ||
   PlatformSpec.Contains(TargetPlatformCPU, ESearchCase::IgnoreCase)))
	{
	  // Minimum target FPS required by Meta Quest applications
		TargetFPS = 72;
	}
	else
	{
	    // Original code
	}
```
The code above is an example, far from perfect, but it shows where to look and what to do to allow the tool to target the custom graphs.
After making the changes, be sure to build (NOT rebuild) the engine again to generate the needed binaries.

### Having Gauntlet running successfully on MetaQuest Devices

The customized Gauntlet test is operational. <br>
However, on Meta Quest devices, there's an issue when attempting to extend the duration of the test over a certain threshold (~10 seconds from testing), resulting in a failed test. <br>
This has not been confirmed on other Android VR devices, but they may potentially present the same issue.

Navigate to the following directory and compare the file *Engine\Source\Programs\AutomationTool\Gauntlet\Platform\Android\Gauntlet.TargetDeviceAndroid.cs* with the version in *EngineSourceChanges\AutomationTool*, starting from line 125.

```
// GauntletDemo: On Meta Quest devices the activity is pushed in background and then resumed when the test duration exceed a certain threshold (~10 seconds), this causes the test to exit prematurely and fail
// Therefore we change condition below to check if the ability is exited (which happens at the end of the test)

// Original variable 
// bool bActivityInForeground = ActivityQuery.Output.Contains("mResumedActivity");

bool bActivityExited = ActivityQuery.HasExited;

bool bHasExited = !bActivityPresent || !bActivityExited;

if (bHasExited)
	{
	    //Original code	
	}
```

You are now ready to execute Gauntlet tests on Meta Quest devices, ensuring their proper functionality. <br>
For added precaution, you could consider disabling certain device features during testing. <br>
Detailed instructions on how to do so can be found in the [following article](https://developer.oculus.com/documentation/unity/ts-scriptable-testing/?intern_source=devblog&intern_content=scale-e2e-on-device-testing-with-meta-quest-scriptable-testing) or by examining the **RunGauntletMetaDevices.bat** file located in the **Scripts** folder.