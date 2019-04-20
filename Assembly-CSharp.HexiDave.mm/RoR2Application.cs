#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
#pragma warning disable 108,114
// ReSharper disable CheckNamespace

using MonoMod;

namespace RoR2
{
	// ReSharper disable once InconsistentNaming
	class patch_RoR2Application : RoR2Application
	{
		public const int PatchedMaxPlayers = 16;

		// Redefine the fields so that we can access them without any restrictions.
		public static int hardMaxPlayers;
		public static int maxPlayers;
		public static int maxLocalPlayers;
		public static bool isModded = true;
		public static string steamBuildId;
		public static string messageForDevelopers;

		// Set our custom values after the game has set its original values.
		private static extern void orig_cctor();

		[MonoModConstructor]
		private static void cctor()
		{
			orig_cctor();

			isModded = true;
		}

		public static string GetBuildId()
		{
			return $"{steamBuildId}-HexiMod";
		}

		private void Awake()
		{
			stopwatch.Start();
			
			DontDestroyOnLoad(gameObject);
			
			if (instance)
			{
				Destroy(gameObject);
				return;
			}

			instance = this;
			
			if (loaded) 
				return;
			
			hardMaxPlayers = PatchedMaxPlayers;
			maxPlayers = PatchedMaxPlayers;
			maxLocalPlayers = PatchedMaxPlayers;
			
			OnLoad();
			
			loaded = true;
		}
	}
}