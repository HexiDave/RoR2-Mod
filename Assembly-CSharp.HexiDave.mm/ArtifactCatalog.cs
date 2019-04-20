using MonoMod;

namespace RoR2
{
	public static class patch_ArtifactCatalog
	{
		[MonoModConstructor]
		static patch_ArtifactCatalog()
		{
			ArtifactCatalog.artifactDefs = new ArtifactDef[5];
			ArtifactCatalog.RegisterArtifact(ArtifactIndex.Command, new ArtifactDef
			{
				nameToken = "Command",
				smallIconSelectedPath = "Textures/ArtifactIcons/texCommandSmallSelected",
				smallIconDeselectedPath = "Textures/ArtifactIcons/texCommandSmallDeselected",
				unlockableName = "",
				descriptionToken = "[NON-FUNCTIONING] Items are no longer random. You can choose which items you want to purchase."
			});
			ArtifactCatalog.RegisterArtifact(ArtifactIndex.Bomb, new ArtifactDef
			{
				nameToken = "Bomb",
				smallIconSelectedPath = "Textures/ArtifactIcons/texSpiteSmallSelected",
				smallIconDeselectedPath = "Textures/ArtifactIcons/texSpiteSmallDeselected",
				unlockableName = "",
				descriptionToken = "Enemies explode on death."
			});
			ArtifactCatalog.RegisterArtifact(ArtifactIndex.Sacrifice, new ArtifactDef
			{
				nameToken = "Sacrifice",
				smallIconSelectedPath = "Textures/ArtifactIcons/texSacrificeSmallSelected",
				smallIconDeselectedPath = "Textures/ArtifactIcons/texSacrificeSmallDeselected",
				unlockableName = "",
				descriptionToken = "[NON-FUNCTIONING] Chests no longer spawn; monsters now drop items on death."
			});
			ArtifactCatalog.RegisterArtifact(ArtifactIndex.Enigma, new ArtifactDef
			{
				nameToken = "Enigma",
				smallIconSelectedPath = "Textures/ArtifactIcons/texEnigmaSmallSelected",
				smallIconDeselectedPath = "Textures/ArtifactIcons/texEnigmaSmallDeselected",
				unlockableName = "",
				descriptionToken = "Use items have a random effect."
			});
			ArtifactCatalog.RegisterArtifact(ArtifactIndex.Spirit, new ArtifactDef
			{
				nameToken = "Spirit",
				smallIconSelectedPath = "Textures/ArtifactIcons/texSpiritSmallSelected",
				smallIconDeselectedPath = "Textures/ArtifactIcons/texSpiritSmallDeselected",
				unlockableName = "",
				descriptionToken = "Characters and enemies move faster at low health"
			});
		}
	}
}