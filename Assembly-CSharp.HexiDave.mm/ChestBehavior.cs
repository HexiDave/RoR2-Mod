using MonoMod;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	public class patch_ChestBehavior : ChestBehavior
	{
		[MonoModReplace]
		public void ItemDrop()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ChestBehavior::ItemDrop()' called on client");
				return;
			}

			if (dropPickup == PickupIndex.none)
			{
				return;
			}

			var localTransform = transform;
			PickupDropletController.CreatePickupDroplet(dropPickup, localTransform.position + Vector3.up * 1.5f,
				Vector3.up * 20f + localTransform.forward * 2f);

			// Lucky!
			if (Util.CheckRoll(5f))
			{
				PickupDropletController.CreatePickupDroplet(dropPickup, localTransform.position + Vector3.up * 1.5f,
					Vector3.up * 22f + localTransform.forward * 4f);
			}
			
			dropPickup = PickupIndex.none;
		}
	}
}