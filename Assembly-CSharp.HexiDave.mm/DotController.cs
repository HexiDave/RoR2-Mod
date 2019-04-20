using MonoMod;
using UnityEngine;

namespace RoR2
{
	public class patch_DotController : DotController
	{
		public static extern void orig_InflictDot(GameObject victimObject, GameObject attackerObject, DotIndex dotIndex,
			float duration = 8f, float damageMultiplier = 1f);

		public static void InflictDot(GameObject victimObject, GameObject attackerObject, DotIndex dotIndex,
			float duration = 8f, float damageMultiplier = 1f)
		{
			if ((victimObject.GetComponent<TeamComponent>()?.teamIndex ??
			     TeamIndex.None) == TeamIndex.Player)
			{
				switch (dotIndex)
				{
					case DotIndex.PercentBurn:
					case DotIndex.Burn:
						duration *= 0.8f;
						damageMultiplier *= 0.5f;
						break;
					case DotIndex.Helfire:
						duration *= 0.5f;
						damageMultiplier *= 0.3f;
						break; 
					default:
						break;
				}
			}

			orig_InflictDot(victimObject, attackerObject, dotIndex, duration, damageMultiplier);
		}
	}
}