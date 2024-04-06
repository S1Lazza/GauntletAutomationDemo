
cd /d "PathToEngineDirectory\Engine\Build\BatchFiles"
RunUAT RunUnreal -project=PathToProjectDirectory\ProjectName.uproject -platform=Android -cookflavor=ASTC -configuration=Development -test=PerformanceGraphData -build=PathToBuildDirectory -uploaddir=PathToGraphOutputDirectory