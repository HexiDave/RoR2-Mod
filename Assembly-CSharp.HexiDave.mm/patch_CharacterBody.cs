using MonoMod;
using UnityEngine;

namespace RoR2
{
	public class patch_CharacterBody : CharacterBody
	{
		private int tier1Items;

		private int tier2Items;

		private int tier3Items;

		private int arcaneAffinity;
		
		public extern void orig_RecalculateStats();
		
		public void RecalculateStats()
		{
			orig_RecalculateStats();

			if (name != "MageBody(Clone)") return;
			
			arcaneAffinity = tier1Items + tier2Items + tier3Items;
			damage = damage + damage * arcaneAffinity / 100f;
		}

		private extern void orig_Start();

		private void Start()
		{
			if (master)
			{
				// Is player controlled - add flags
				bodyFlags |= BodyFlags.IgnoreFallDamage | BodyFlags.SprintAnyDirection;
			}
			
			orig_Start();
		}
		
		[MonoModReplace]
		public bool HasBuff(BuffIndex buffType)
		{
			// Ignore Cripple buff for players
			if (buffType == BuffIndex.Cripple && master)
			{
				return false;
			}
			return buffMask.HasBuff(buffType);
		}
	}
}