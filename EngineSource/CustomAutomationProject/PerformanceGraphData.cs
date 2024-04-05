using System.Collections.Generic;
using System.Linq;
using System.IO;
using Gauntlet;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UnrealGame;
using UnrealBuildTool;

namespace GauntletDemo.Automation
{
	public class PerformanceGraphData : DefaultTest
	{
		public PerformanceGraphData(UnrealTestContext inContext)
			: base(inContext)
		{
		}

		private string PerfReportToolPath = "Engine/Binaries/DotNET/CsvTools/PerfReportTool.exe";
		private string GauntletController = "CustomGauntletController";

		public override UnrealTestConfig GetConfiguration()
		{
			UnrealTestConfig Config = base.GetConfiguration();
			UnrealTestRole ClientRole = Config.RequireRole(UnrealTargetRole.Client);
			ClientRole.Controllers.Add(GauntletController);
			Config.MaxDuration = 10 * 60; // 10 minutes: this is a time limit, not the time the tests will take
			Config.MaxDurationReachedResult = EMaxDurationReachedResult.Failure;
			return Config;
		}

		public override void TickTest()
		{
			base.TickTest();
		}

		public override ITestReport CreateReport(TestResult Result, UnrealTestContext Contex, UnrealBuildSource Build, IEnumerable<UnrealRoleResult> RoleResults, string ArtifactPath)
		{
			UnrealRoleArtifacts ClientArtifacts = RoleResults.Where(R => R.Artifacts.SessionRole.RoleType == UnrealTargetRole.Client).Select(R => R.Artifacts).FirstOrDefault();

			var SnapshotSummary = new UnrealSnapshotSummary<UnrealHealthSnapshot>(ClientArtifacts.AppInstance.StdOut);

			Log.Info("Performance Report");
			Log.Info(SnapshotSummary.ToString());

			return base.CreateReport(Result, Contex, Build, RoleResults, ArtifactPath);
		}

		public override void SaveArtifacts_DEPRECATED(string OutputPath)
		{
			string uploadDir = Globals.Params.ParseValue("uploaddir", "");

			Log.Info("====== Upload directory: " + uploadDir);

			if (uploadDir.Count() > 0 && Directory.CreateDirectory(uploadDir).Exists)
			{
				string artifactDir = TestInstance.ClientApps[0].ArtifactPath;
				string profilingDir = Path.Combine(artifactDir, "Profiling");
				string csvDir = Path.Combine(profilingDir, "CSV");
				string targetDir = Path.Combine(uploadDir, "CSV");

				if (!Directory.Exists(targetDir))
					Directory.CreateDirectory(targetDir);

				if (!Directory.Exists(csvDir))
					Directory.CreateDirectory(csvDir);

				string[] csvFiles = Directory.GetFiles(csvDir);
				foreach (string csvFile in csvFiles)
				{
					string targetCSVFile = Path.Combine(targetDir, Path.GetFileName(csvFile));
					File.Copy(csvFile, targetCSVFile);

					if (!File.Exists(PerfReportToolPath))
					{
						Log.Error("Can't find PerfReportTool.exe at " + PerfReportToolPath, ", aborting!");
						break;
					}

					ProcessStartInfo startInfo = new ProcessStartInfo();
					startInfo.FileName = PerfReportToolPath;
					startInfo.Arguments = "-csv ";
					startInfo.Arguments += targetCSVFile;
					startInfo.Arguments += " -o ";
					startInfo.Arguments += uploadDir;

					try
					{
						using (Process exeProcess = Process.Start(startInfo))
						{
							exeProcess.WaitForExit();
						}
					}
					catch
					{
						Log.Error("Error running PerfReportTool.exe, aborting!");
					}
				}
			}
			else
				Log.Error("No UploadDir specified, not copying performance report! Set one with -uploaddir=c:/path/to/dir");
		}
	}
}

