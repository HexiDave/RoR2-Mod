using System;
using System.IO;
using MonoMod;
using Utilities;

namespace AssemblySetup
{
	internal class Program
	{
		private static void ReplaceGamePathTarget()
		{
			const string replaceTarget = "REPLACE_ME";
			
			var gamePathTargetsPath = $@"{EnvHelper.SolutionDir}\GamePath.targets";
			var gamePathTargetsTemplatePath = $@"{gamePathTargetsPath}.template";

			var gamePathTargetsContent = File.ReadAllText(gamePathTargetsTemplatePath);

			gamePathTargetsContent = gamePathTargetsContent.Replace(replaceTarget, FileUtilities.GamePath);
			
			File.WriteAllText(gamePathTargetsPath, gamePathTargetsContent);
		}
		
		public static void Main(string[] args)
		{
			// Backup the assembly before setup
			FileUtilities.EnsureBackup();
			
			// Rebuild the GamePath.targets file with correct game path
			ReplaceGamePathTarget();

			// Directory for storing 'public'-ified assembly 
			var libsDir = $@"{EnvHelper.SolutionDir}\libs";

			// Ensure the path exists for output
			Directory.CreateDirectory(libsDir);
			
			// Output path for the assembly
			var publicAssemblyPath = $@"{libsDir}\{FileUtilities.AssemblyFileName}";

			// Remove the old one, if it exists
			if (File.Exists(publicAssemblyPath))
			{
				File.Delete(publicAssemblyPath);
			}
			
			// Create the all-public assembly for import
			using (var monoMod = new MonoModder
			{
				InputPath = FileUtilities.BackupAssemblyPath,
				OutputPath = publicAssemblyPath
			})
			{
				monoMod.Read();
				monoMod.PublicEverything = true;
				monoMod.MapDependencies();
				monoMod.AutoPatch();
				monoMod.Write();
			}
		}
	}
}