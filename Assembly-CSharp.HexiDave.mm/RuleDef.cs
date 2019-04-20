using MonoMod;
using UnityEngine;

namespace RoR2
{
	public class patch_RuleDef
	{
		[MonoModReplace]
		public static RuleDef FromArtifact(ArtifactIndex artifactIndex)
		{
			var artifactDef = ArtifactCatalog.GetArtifactDef(artifactIndex);
			var ruleDef = new RuleDef($"Artifacts.{artifactIndex}", artifactDef.nameToken);
			
			var ruleChoiceDef = ruleDef.AddChoice("On");
			ruleChoiceDef.spritePath = artifactDef.smallIconSelectedPath;
			ruleChoiceDef.tooltipBodyToken = artifactDef.descriptionToken;
			ruleChoiceDef.unlockableName = artifactDef.unlockableName;
			ruleChoiceDef.artifactIndex = artifactIndex;
			ruleChoiceDef.tooltipNameColor = new Color32(74, 50, 149, byte.MaxValue);
			ruleChoiceDef.tooltipNameToken = $"{artifactDef.nameToken} On";
			
			var ruleChoiceDef2 = ruleDef.AddChoice("Off");
			ruleChoiceDef2.spritePath = artifactDef.smallIconDeselectedPath;
			ruleChoiceDef2.materialPath = "Materials/UI/matRuleChoiceOff";
			ruleChoiceDef.tooltipBodyToken = artifactDef.descriptionToken;
			ruleChoiceDef.tooltipNameColor = new Color32(74, 50, 149, byte.MaxValue);
			ruleChoiceDef.tooltipNameToken = $"{artifactDef.nameToken} Off";

			ruleDef.MakeNewestChoiceDefault();
			
			return ruleDef;
		}
	}
}