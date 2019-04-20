#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
#pragma warning disable 108,114
// ReSharper disable CheckNamespace

using System;
using System.Linq;
using MonoMod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RoR2
{
	class patch_SurvivorCatalog
	{
		public static int survivorMaxCount;
		private static extern void orig_cctor();

		[MonoModConstructor]
		private static void cctor()
		{
			orig_cctor();
		}


		[MonoModReplace]
		public static SurvivorDef GetSurvivorDef(SurvivorIndex survivorIndex)
		{
			if ((int) survivorIndex < 0 || (int) survivorIndex > SurvivorCatalog.survivorDefs.Length)
			{
				return null;
			}

			return SurvivorCatalog.survivorDefs[(int) survivorIndex];
		}

		private static void RegisterBandit()
		{
			var characterObject = Object.Instantiate(BodyCatalog.FindBodyPrefab("BanditBody"));
			Object.DontDestroyOnLoad(characterObject);
			characterObject.SetActive(false);
			var skillLocator = characterObject.GetComponent<SkillLocator>();

			skillLocator.primary.skillNameToken = "Blast";
			skillLocator.primary.skillDescriptionToken =
				"Fire a powerful slug for <style=cIsDamage>150% damage</style>.";

			skillLocator.secondary.skillNameToken = "Lights Out";
			skillLocator.secondary.skillDescriptionToken =
				"Take aim for a headshot, dealing <style=cIsDamage>600% damage</style>. If the ability <style=cIsDamage>kills an enemy</style>, the Bandit's <style=cIsUtility>Cooldowns are all reset to 0</style>.";

			skillLocator.utility.skillNameToken = "Smokebomb";
			skillLocator.utility.skillDescriptionToken =
				"Turn invisible for <style=cIsDamage>3 seconds</style>, gaining <style=cIsUtility>increased movement speed</style>.";

			skillLocator.special.skillNameToken = "Thermite Toss";
			skillLocator.special.skillDescriptionToken =
				"Fire off a burning Thermite grenade, dealing <style=cIsDamage>damage over time</style>.";
				
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Bandit, new SurvivorDef
			{
				bodyPrefab = characterObject,
				displayPrefab = Resources.Load<GameObject>("Prefabs/Characters/BanditDisplay"),
				descriptionToken = "BANDIT_DESCRIPTION",
				primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f)
			});
		}

		private static void RegisterMage()
		{
			var bodyPrefab = BodyCatalog.FindBodyPrefab("MageBody");
			var skillLocator = bodyPrefab.GetComponent<SkillLocator>();

			var passiveSkill = skillLocator.passiveSkill;
			passiveSkill.skillNameToken = "Arcane Affinity";
			passiveSkill.skillDescriptionToken = "Absorb energy from all current items. Gaining <style=cIsUtility>1-3% bonus damage</style> per item, depending on its tier.";
			passiveSkill.enabled = true;
			passiveSkill.icon = passiveSkill.icon;
			skillLocator.passiveSkill = passiveSkill;
			
			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Mage, new SurvivorDef
			{
				bodyPrefab = bodyPrefab,
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/MageDisplay"),
				descriptionToken = "MAGE_DESCRIPTION",
				primaryColor = new Color(0.968627453f, 0.75686276f, 0.992156863f),
				unlockableName = "Characters.Mage"
			});
		}

		[SystemInitializer]
		[MonoModReplace]
		private static void Init()
		{
			SurvivorCatalog.idealSurvivorOrder = new[]
			{
				SurvivorIndex.Commando,
				SurvivorIndex.Toolbot,
				SurvivorIndex.Huntress,
				SurvivorIndex.Engineer,
				SurvivorIndex.Mage,
				SurvivorIndex.Merc,
				SurvivorIndex.Bandit
			};

			SurvivorCatalog.survivorMaxCount = SurvivorCatalog.idealSurvivorOrder.Length;

			SurvivorCatalog.survivorDefs = new SurvivorDef[SurvivorCatalog.survivorMaxCount];

			Debug.LogFormat("[Debug] Defined Survivor Array with {0} survivor slots and max survivor count of {1}",
				SurvivorCatalog.survivorDefs.Length, SurvivorCatalog.survivorMaxCount);

			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Commando, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("CommandoBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/CommandoDisplay"),
				descriptionToken = "COMMANDO_DESCRIPTION",
				primaryColor = new Color(0.929411769f, 0.5882353f, 0.07058824f)
			});

			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Huntress, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("HuntressBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/HuntressDisplay"),
				primaryColor = new Color(0.8352941f, 0.235294119f, 0.235294119f),
				descriptionToken = "HUNTRESS_DESCRIPTION",
				unlockableName = "Characters.Huntress"
			});

			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Toolbot, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("ToolbotBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/ToolbotDisplay"),
				descriptionToken = "TOOLBOT_DESCRIPTION",
				primaryColor = new Color(0.827451f, 0.768627465f, 0.3137255f),
				unlockableName = "Characters.Toolbot"
			});

			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Engineer, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("EngiBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/EngiDisplay"),
				descriptionToken = "ENGI_DESCRIPTION",
				primaryColor = new Color(0.372549027f, 0.8862745f, 0.5254902f),
				unlockableName = "Characters.Engineer"
			});

			RegisterMage();

			SurvivorCatalog.RegisterSurvivor(SurvivorIndex.Merc, new SurvivorDef
			{
				bodyPrefab = BodyCatalog.FindBodyPrefab("MercBody"),
				displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/MercDisplay"),
				descriptionToken = "MERC_DESCRIPTION",
				primaryColor = new Color(0.423529416f, 0.819607854f, 0.917647064f),
				unlockableName = "Characters.Mercenary"
			});

			RegisterBandit();

			for (var survivorIndex = 0; survivorIndex < SurvivorCatalog.survivorDefs.Length; survivorIndex++)
			{
				Debug.LogFormat("[Debug] Survivor index {0}: {1}", survivorIndex,
					SurvivorCatalog.survivorDefs.ElementAt(survivorIndex).bodyPrefab);
				if (SurvivorCatalog.survivorDefs[survivorIndex] == null)
				{
					Debug.LogWarningFormat("Unregistered survivor {0}!", survivorIndex);
				}
			}

			SurvivorCatalog._allSurvivorDefs = (from v in SurvivorCatalog.survivorDefs
				where v != null
				select v).ToArray();

			var node = new ViewablesCatalog.Node("Survivors", true);

			using (var enumerator = SurvivorCatalog.allSurvivorDefs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					var survivor = enumerator.Current;

					if (survivor == null)
						continue;

					Debug.LogFormat("[DEBUG] Creating node for {0}.", survivor.displayNameToken);

					var survivorEntryNode =
						new ViewablesCatalog.Node(survivor.displayNameToken, false, node);

					survivorEntryNode.shouldShowUnviewed = userProfile =>
						!userProfile.HasViewedViewable(survivorEntryNode.fullName) &&
						userProfile.HasSurvivorUnlocked(survivor.survivorIndex) &&
						!string.IsNullOrEmpty(survivor.unlockableName);

					Debug.LogFormat("[DEBUG] Created node {0}", survivorEntryNode.name);
				}
			}

			ViewablesCatalog.AddNodeToRoot(node);
		}
	}
}