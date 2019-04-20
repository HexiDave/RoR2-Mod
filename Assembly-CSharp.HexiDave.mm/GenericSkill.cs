using UnityEngine;

namespace RoR2
{
	public class patch_GenericSkill : GenericSkill
	{
		private extern void orig_Awake();

		private void Awake()
		{
			orig_Awake();

			switch (skillNameToken)
			{
				case "COMMANDO_UTILITY_NAME":
					noSprint = false;
					baseRechargeInterval = 1;
					break;
				case "MERC_UTILITY_NAME":
					noSprint = false;
					baseRechargeInterval = 3;
					break;
				case "MAGE_PRIMARY_NAME":
					baseRechargeInterval = 0;
					baseMaxStock = 1;
					stockToConsume = 0;
					break;
				case "MAGE_UTILITY_ICE_NAME":
					noSprint = true;
					baseRechargeInterval = 3;
					break;
			}
			
			RecalculateFinalRechargeInterval();
			RecalculateMaxStock();
		}
	}
}