
# Gauntlet Demo - Unreal Engine 5.2

## Software Requirements
- Visual Studio 2019 or 2022 and [following components](https://dev.epicgames.com/documentation/en-us/unreal-engine/setting-up-visual-studio-development-environment-for-cplusplus-projects-in-unreal-engine?application_version=5.0) - 2022 is recommended
- [Android SDK and Java](https://dev.epicgames.com/documentation/en-us/unreal-engine/setting-up-unreal-engine-projects-for-android-development?application_version=5.0) 
- [Unreal Engine 5.2 Source Code](https://github.com/EpicGames/Signup)

</br>

## Additional Notes
- The <ins>Unreal Project Setup</ins> section has been included to provide a complete explanation, but can be skipped, as it is already integrated into the project repository.
- The folder **EngineSource** contains the added/modified engine files required for the Gauntlet setup
- The folder **Scripts** contains multiple batch scripts to run Gauntlet on target builds and generate the performance graph data

</br>

## Automation Project Setup

To gather the performance data and generate graph, we will use the built-in **Performance Report Tool** in Unreal Engine. <br>
[Building the engine](https://github.com/EpicGames/UnrealEngine/tree/5.2) will automatically generate the needed binaries to allow us to use it. <br> 
You can verify their presence by navigating to *Engine\Binaries\DotNET\CsvTools* and checking for the **PerfreportTool.exe** file.
If the binaries are not found, you need to manually rebuild the project solution located at *Engine\Source\Programs\CSVTools*.

Next, you'll need to add a new C# project to the source engine. <br>
Refer to the [following documentation](https://dev.epicgames.com/documentation/en-us/unreal-engine/create-an-automation-project-in-unreal-engine?application_version=5.2) for instructions on how to do this. <br>
Follow the steps outlined under the <ins>Create an Automation Project</ins> and <ins>Configure your Automation Project Adjust Build Settings</ins> sections.<br>

Proceed by right-clicking on your recently created project in the Solution Explorer and choose Properties. <br> Navigate to the **Build** tab, under the **Ouput** section, and input the following in the **Base output path** field: <i>../../../Binaries/DotNET/AutomationScripts/</i>

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
This code is required to prevent Visual Studio from encountering configuration errors and defaulting the automation project configuration to Debug instead of the desired Develop.

Once this is done, close Visual Studio, navigate to your engine's root directory, and execute the  *GenerateProjectFiles.bat*, then, reopen the **UE5.sln**. <br>
Your project should now be visible under *UE5/Programs/Automation* in the Solution Explorer.

Next, right-click on your project and navigate to *Add->Reference*. <br> 
Switch to the Projects tab on the left and select the following three projects:

- AutomationUtils.Automation
- Gauntlet.Automation
- UnrealBuildTool

By following these steps, you should have a file identical to the one found in the folder *EngineSourceChanges/CustomAutomationProject/GauntletDemo.Automation.csproj*

Now, open the **.cs** project file that was renamed in step 6 of the <ins>Create an Automation Project</ins> section. <br> 
Compare it with the file located at *EngineSource/AutomationProject/PerformanceGraphData.cs* and update the code to match with your class name.

For further guidance, refer to the examples provided located at *Gauntlet.Automation/Unreal/Game/Samples* in the Solution Explorer, with particular attention to the **ElementalDemoTest.cs** one.

</br>

## Unreal Project Setup

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
You can place it anywhere within your game's Source folder, just ensure that its name matches the one specified into the **.cs** file for the **GauntletController** variable. 

With these changes, Gauntlet is now ready to run within your game project.

</br>

## Additional Engine Changes

If you intend to run Gauntlet on Windows builds without further modifications, everything is set up and you can skip the explanation below. <br>
Utilize the **.bat** script located in the **Scripts** folder to make and execute Gauntlet on target builds. <br>
Just be sure to adjust the paths according to your project's location and name and be sure the builds made are in **Development** configuration. <br>
Each time a test is executed, the performance data will be stored in a folder named **PerfTests** within the project directory.


However by default, the PerfTest tool does only generate graphs for 30 and 60 fps as target.
You'll need to edit the 2 files to add new framerate if needed. It is possible to play around with them to add additional data and compress more data together.
Please refers to the files.

However, to allow to recognize your specific target framerate you'll have to do additional changes, please refer to the file to check how I managed to have the tool targeting quest.

Finally, specifically for Quest 2 there's another trick to 
