using System.Collections.Generic;
using System.Linq;
using MonoMod;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	public static class patch_RuleCatalog
	{
		private static readonly List<RuleDef> allRuleDefs;

		private static readonly List<RuleChoiceDef> allChoicesDefs;

		public static readonly List<RuleCategoryDef> allCategoryDefs;

		private static readonly Dictionary<string, RuleDef> ruleDefsByGlobalName;

		private static readonly Dictionary<string, RuleChoiceDef> ruleChoiceDefsByGlobalName;

		public static ResourceAvailability availability;

		private static readonly BoolConVar ruleShowItems;

		[MonoModConstructor]
		static patch_RuleCatalog()
		{
			allRuleDefs = new List<RuleDef>();
			allChoicesDefs = new List<RuleChoiceDef>();
			allCategoryDefs = new List<RuleCategoryDef>();
			ruleDefsByGlobalName = new Dictionary<string, RuleDef>();
			ruleChoiceDefsByGlobalName = new Dictionary<string, RuleChoiceDef>();
			ruleShowItems = new BoolConVar("rule_show_items", ConVarFlags.None, "1",
				"Whether or not to allow voting on items in the pregame rules.");
			RuleCatalog.AddCategory("RULE_HEADER_DIFFICULTY", new Color32(28, 99, 150, byte.MaxValue));
			RuleCatalog.AddRule(RuleDef.FromDifficulty());
			RuleCatalog.AddCategory("RULE_HEADER_ARTIFACTS", new Color32(74, 50, 149, byte.MaxValue), null,
				RuleCatalog.HiddenTestFalse);
			for (ArtifactIndex artifactIndex = 0; artifactIndex < ArtifactIndex.Count; artifactIndex++)
			{
				RuleCatalog.AddRule(RuleDef.FromArtifact(artifactIndex));
			}

			RuleCatalog.AddCategory("RULE_HEADER_ITEMS", new Color32(147, 225, 128, byte.MaxValue), null,
				RuleCatalog.HiddenTestItemsConvar);
			var list = new List<ItemIndex>();
			for (ItemIndex itemIndex = 0; itemIndex < ItemIndex.Count; itemIndex++)
			{
				list.Add(itemIndex);
			}

			foreach (var itemIndex2 in from i in list
				where ItemCatalog.GetItemDef(i).inDroppableTier
				orderby ItemCatalog.GetItemDef(i).tier
				select i)
			{
				RuleCatalog.AddRule(RuleDef.FromItem(itemIndex2));
			}

			RuleCatalog.AddCategory("RULE_HEADER_EQUIPMENT", new Color32(byte.MaxValue, 128, 0, byte.MaxValue), null,
				RuleCatalog.HiddenTestItemsConvar);
			var list2 = new List<EquipmentIndex>();
			for (EquipmentIndex equipmentIndex = 0; equipmentIndex < EquipmentIndex.Count; equipmentIndex++)
			{
				list2.Add(equipmentIndex);
			}

			foreach (var equipmentIndex2 in from i in list2
				where EquipmentCatalog.GetEquipmentDef(i).canDrop
				select i)
			{
				RuleCatalog.AddRule(RuleDef.FromEquipment(equipmentIndex2));
			}

			RuleCatalog.AddCategory("RULE_HEADER_MISC", new Color32(192, 192, 192, byte.MaxValue), null,
				RuleCatalog.HiddenTestFalse);
			var ruleDef = new RuleDef("Misc.StartingMoney", "RULE_MISC_STARTING_MONEY");
			var ruleChoiceDef = ruleDef.AddChoice("0", 0, true);
			ruleChoiceDef.tooltipNameToken = "RULE_STARTINGMONEY_CHOICE_0_NAME";
			ruleChoiceDef.tooltipBodyToken = "RULE_STARTINGMONEY_CHOICE_0_DESC";
			ruleChoiceDef.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			var ruleChoiceDef2 = ruleDef.AddChoice("15", 15, true);
			ruleChoiceDef2.tooltipNameToken = "RULE_STARTINGMONEY_CHOICE_15_NAME";
			ruleChoiceDef2.tooltipBodyToken = "RULE_STARTINGMONEY_CHOICE_15_DESC";
			ruleChoiceDef2.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			ruleDef.MakeNewestChoiceDefault();
			var ruleChoiceDef3 = ruleDef.AddChoice("50", 50, true);
			ruleChoiceDef3.tooltipNameToken = "RULE_STARTINGMONEY_CHOICE_50_NAME";
			ruleChoiceDef3.tooltipBodyToken = "RULE_STARTINGMONEY_CHOICE_50_DESC";
			ruleChoiceDef3.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
			ruleChoiceDef3.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			RuleCatalog.AddRule(ruleDef);
			var ruleDef2 = new RuleDef("Misc.StageOrder", "RULE_MISC_STAGE_ORDER");
			var ruleChoiceDef4 = ruleDef2.AddChoice("Normal", 0, true);
			ruleChoiceDef4.tooltipNameToken = "RULE_STAGEORDER_CHOICE_NORMAL_NAME";
			ruleChoiceDef4.tooltipBodyToken = "RULE_STAGEORDER_CHOICE_NORMAL_DESC";
			ruleChoiceDef4.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			ruleDef2.MakeNewestChoiceDefault();
			var ruleChoiceDef5 = ruleDef2.AddChoice("Random", 1, true);
			ruleChoiceDef5.tooltipNameToken = "RULE_STAGEORDER_CHOICE_RANDOM_NAME";
			ruleChoiceDef5.tooltipBodyToken = "RULE_STAGEORDER_CHOICE_RANDOM_DESC";
			ruleChoiceDef5.spritePath = "Textures/MiscIcons/texRuleMapIsRandom";
			ruleChoiceDef5.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			RuleCatalog.AddRule(ruleDef2);
			var ruleDef3 = new RuleDef("Misc.KeepMoneyBetweenStages", "RULE_MISC_KEEP_MONEY_BETWEEN_STAGES");
			ruleDef3.AddChoice("On", 1, true).tooltipBodyToken = "RULE_KEEPMONEYBETWEENSTAGES_CHOICE_ON_DESC";
			ruleDef3.AddChoice("Off", 0, true).tooltipBodyToken = "RULE_KEEPMONEYBETWEENSTAGES_CHOICE_OFF_DESC";
			ruleDef3.MakeNewestChoiceDefault();
			RuleCatalog.AddRule(ruleDef3);
			for (var k = 0; k < RuleCatalog.allRuleDefs.Count; k++)
			{
				var ruleDef4 = RuleCatalog.allRuleDefs[k];
				ruleDef4.globalIndex = k;
				for (var j = 0; j < ruleDef4.choices.Count; j++)
				{
					var ruleChoiceDef6 = ruleDef4.choices[j];
					ruleChoiceDef6.localIndex = j;
					ruleChoiceDef6.globalIndex = RuleCatalog.allChoicesDefs.Count;
					RuleCatalog.allChoicesDefs.Add(ruleChoiceDef6);
				}
			}

			RuleCatalog.availability.MakeAvailable();
		}
	}
}