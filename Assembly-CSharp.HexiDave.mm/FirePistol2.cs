#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
#pragma warning disable 108,114
// ReSharper disable CheckNamespace

using MonoMod;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// ReSharper disable once InconsistentNaming
	public class patch_FirePistol2 : FirePistol2
	{
		public static GameObject muzzleEffectPrefab;
		public static GameObject boostedMuzzleEffectPrefab;
		public static GameObject hitEffectPrefab;
		public static GameObject tracerEffectPrefab;
		public static GameObject boostedTracerEffectPrefab;
		public static float damageCoefficient;
		public static float force;
		public static float baseDuration;
		public static string firePistolSoundString;
		public static string boostedFirePistolSoundString;
		public static float recoilAmplitude;
		public static float spreadBloomValue;
		public static float commandoBoostBuffCoefficient;
		public int remainingShots;
		public Ray aimRay;
		public float duration;
		public bool boosted;
		
		private static extern void orig_cctor();

		[MonoModConstructor]
		private static void cctor()
		{
			orig_cctor();
		}

		private extern void orig_FireBullet(string targetMuzzle);
		private void FireBullet(string targetMuzzle)
		{
			Util.PlaySound(boosted ? boostedFirePistolSoundString : firePistolSoundString, gameObject);
			if (muzzleEffectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(boosted ? boostedMuzzleEffectPrefab : muzzleEffectPrefab, gameObject, targetMuzzle, false);
			}
			AddRecoil(-0.4f * recoilAmplitude, -0.8f * recoilAmplitude, -0.3f * recoilAmplitude, 0.3f * recoilAmplitude);
			if (isAuthority)
			{
				new BulletAttack
				{
					owner = gameObject,
					weapon = gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = 0f,
					maxSpread = characterBody.spreadBloomAngle,
					damage = damageCoefficient * damageStat * 2f, // Double the base damage
					force = force,
					tracerEffectPrefab = boosted ? boostedTracerEffectPrefab : tracerEffectPrefab,
					muzzleName = targetMuzzle,
					hitEffectPrefab = hitEffectPrefab,
					isCrit = Util.CheckRoll(critStat, characterBody.master),
					radius = 0.1f,
					smartCollision = true
				}.Fire();
			}
			characterBody.AddSpreadBloom(spreadBloomValue);
		}
	}
}