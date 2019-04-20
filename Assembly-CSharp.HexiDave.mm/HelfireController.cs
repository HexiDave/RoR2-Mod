using System;
using MonoMod;
using UnityEngine;

namespace RoR2
{
	public class patch_HelfireController : HelfireController
	{
		[MonoModReplace]
		private void ServerFixedUpdate()
		{
			timer -= Time.fixedDeltaTime;
			if (!(timer <= 0f)) return;

			var damageMultiplier = 1f + (stack - 1) * 0.5f;
			timer = interval;
			var array = Physics.OverlapSphere(transform.position, radius, LayerIndex.entityPrecise.mask,
				QueryTriggerInteraction.Collide);
			var array2 = new GameObject[array.Length];
			var count = 0;
			
			var teamIndex = networkedBodyAttachment.attachedBodyObject.GetComponent<TeamComponent>()?.teamIndex ?? TeamIndex.None;
			
			foreach (var collider in array)
			{
				var otherGameObject = GetGameObjectFromCollider(collider);
				
				if (!otherGameObject || Array.IndexOf(array2, otherGameObject, 0, count) != -1) continue;

				var otherTeamIndex = otherGameObject.GetComponent<TeamComponent>()?.teamIndex ?? TeamIndex.None;
				
				// Don't affect friendlies
				if (teamIndex == TeamIndex.Player && otherTeamIndex == TeamIndex.Player) continue;

				DotController.InflictDot(otherGameObject, networkedBodyAttachment.attachedBodyObject,
					DotController.DotIndex.Helfire, dotDuration, damageMultiplier);
				array2[count++] = otherGameObject;
			}
		}

		public static GameObject GetGameObjectFromCollider(Collider collider)
		{
			var hurtBox = collider.GetComponent<HurtBox>();
			if (hurtBox && hurtBox.healthComponent)
			{
				return hurtBox.healthComponent.gameObject;
			}

			return null;
		}
	}
}