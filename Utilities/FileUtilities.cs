using System;
using System.IO;
using Microsoft.Win32;

namespace Utilities
{
	public static class FileUtilities
	{
		public static string GamePath
		{
			get
			{
				using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
				using (var reg = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 632360",
					false))
				{
					return reg?.GetValue("InstallLocation") as string;
				}
			}
		}

		public static string ManagedPath => $@"{GamePath}\Risk of Rain 2_Data\Managed";
		
		public const string AssemblyFileName = "Assembly-CSharp.dll";

		public static string AssemblyPath => $@"{ManagedPath}\{AssemblyFileName}";

		public static string BackupAssemblyPath => $@"{AssemblyPath}.original";

		public static void EnsureBackup()
		{
			// No backup?
			if (!File.Exists(BackupAssemblyPath))
			{
				// Make one!
				File.Copy(AssemblyPath, BackupAssemblyPath, true);
			}
		}
	}
}