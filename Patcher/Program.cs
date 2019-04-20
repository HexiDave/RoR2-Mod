using System;
using System.IO;
using System.IO.Compression;
using MonoMod;
using Utilities;

namespace Patcher
{
	internal class Program
	{
		private const string DefaultModFileName = "Assembly-CSharp.HexiDave.mm.dll";
		
		/// <summary>
		/// Perform the actual patching of Assembly-CSharp.dll in the Risk of Rain 2 managed folder
		/// </summary>
		/// <param name="modPath">The path to the Assembly-CSharp.HexiDave.mm.dll file</param>
		private static void PatchWithMod(string modPath)
		{
			// Ensure that we have an Assembly-CSharp.dll.original file to work from
			FileUtilities.EnsureBackup();
			
			// Get a temporary file to write the new assembly to
			var outputPath = Path.GetTempFileName();

			// Setup MonoModder to patch the original Assembly-CSharp.dll
			using (var monoModder = new MonoModder
			{
				InputPath = FileUtilities.BackupAssemblyPath,
				OutputPath = outputPath
			})
			{
				// Read the assembly
				monoModder.Read();
				
				// Read the patch
				monoModder.ReadMod(modPath);
				
				// Ensure all the assembly references are still set
				monoModder.MapDependencies();
				
				// Re-write assembly with patch functions
				monoModder.AutoPatch();
				
				// Spit the file out
				monoModder.Write();
			}
			
			// Clear the assembly in RoR2's managed folder
			File.Delete(FileUtilities.AssemblyPath);
			
			// Move the patched assembly into place
			File.Move(outputPath, FileUtilities.AssemblyPath);

			// Make sure any dependencies are moved
			// TODO: Maybe remove this, but still tinkering
			var filesToInclude = new[]
			{
				"Mono.Cecil.dll"
			};
			
			foreach (var fileName in filesToInclude)
			{
				var moveToPath = $@"{FileUtilities.ManagedPath}\{fileName}";

				if (!File.Exists(moveToPath))
				{
					File.Move(fileName, moveToPath);
				}
			}
		}

		/// <summary>
		/// Start the patch process from a Zip file, presumably downloaded from GitHub's releases.
		/// Presently assumes there's just the Assembly-CSharp.HexiDave.mm.dll file inside
		/// </summary>
		/// <param name="zipPath">Path to the Zip file to start from</param>
		private static void PatchFromZip(string zipPath)
		{
			var outputPath = Path.GetTempFileName();
			
			using (var zipArchive = ZipFile.OpenRead(zipPath))
			{
				// TODO: Null check and abort
				zipArchive.GetEntry(DefaultModFileName)?.ExtractToFile(outputPath, true);	
			}
			
			PatchWithMod(outputPath);
		}

		private static void PatchFromGitHub()
		{
			// TODO: Fill this out post-GitHub commit/release
		}
		
		public static void Main(string[] args)
		{
			// Try to use the first item in the path
			if (args.Length > 0)
			{
				// TODO: Implement Zip file check
				PatchWithMod(args[0]);
				return;
			}
			
			// Else, try patching with the local patch file in the directory (good for development)
			// TODO: Implement local check, then go for GitHub release if missing
			PatchWithMod(DefaultModFileName);
		}
	}
}