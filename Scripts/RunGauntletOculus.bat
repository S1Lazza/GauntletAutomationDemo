:: Disable three system features that would otherwise interfere with an E2E test.
:: Note that a developer (or test user) must be logged on and you must specify
:: the Oculus PIN associated with that logged in account.
adb shell content call --uri content://com.oculus.rc --method SET_PROPERTY --extra 'disable_guardian:b:true' --extra 'disable_dialogs:b:true'  --extra 'disable_autosleep:b:true' --extra 'PIN:s:PersonalAccountPin' 
::Result: Bundle[{Success=true}]

:: Query the currently set property values.
adb shell content call --uri content://com.oculus.rc --method GET_PROPERTY 
::Result: Bundle[{disable_guardian=true, disable_dialogs=true, disable_autosleep=true}]

:: Simulate putting on the headset by triggering the proximity sensor.
adb shell am broadcast -a com.oculus.vrpowermanager.prox_close

:: Execute your test here
cd /d "PathToEngineDirectory\Engine\Build\BatchFiles"
RunUAT RunUnreal -project=PathToProjectDirectory\ProjectName.uproject -platform=Android -cookflavor=ASTC -configuration=Development -test=PerformanceGraphData -build=PathToBuildDirectory -uploaddir=PathToGraphOutputDirectory

:: Re-enable the system features.
adb shell content call --uri content://com.oculus.rc --method SET_PROPERTY --extra 'disable_guardian:b:false' --extra 'disable_dialogs:b:false' --extra 'disable_autosleep:b:false' --extra 'PIN:s:PersonalAccountPin'

:: Simulate taking the headset off by triggering the proximity sensor.
adb shell am broadcast -a com.oculus.vrpowermanager.prox_far