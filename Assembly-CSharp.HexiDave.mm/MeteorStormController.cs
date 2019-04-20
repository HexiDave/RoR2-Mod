using MonoMod;
using UnityEngine;

namespace RoR2
{
	public class patch_MeteorStormController : MeteorStormController
	{
		[MonoModReplace]
		private void DetonateMeteor(Meteor meteor)
		{
			var effectData = new EffectData
			{
				origin = meteor.impactPosition
			};
			
			EffectManager.instance.SpawnEffect(impactEffectPrefab, effectData, true);

			var localGameObject = gameObject;
			
			new BlastAttack
			{
				inflictor = localGameObject,
				baseDamage = blastDamageCoefficient * ownerDamage,
				baseForce = blastForce,
				canHurtAttacker = true,
				crit = isCrit,
				falloffModel = BlastAttack.FalloffModel.Linear,
				attacker = owner,
				bonusForce = Vector3.zero,
				damageColorIndex = DamageColorIndex.Item,
				position = meteor.impactPosition,
				procChainMask = default(ProcChainMask),
				procCoefficient = 1f,
				teamIndex = TeamComponent.GetObjectTeam(localGameObject), // Make sure meteor storm doesn't hurt friendlies
				radius = blastRadius
			}.Fire();
		}
	}
}