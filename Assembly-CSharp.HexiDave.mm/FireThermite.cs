using MonoMod;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	[MonoModIgnore]
	public class NOPE_FireThermite : BaseState
	{
		private void FireGrenade(string targetMuzzle)
		{
			Util.PlaySound(attackSoundString, gameObject);
			aimRay = GetAimRay();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(targetMuzzle);
					if (transform)
					{
						aimRay.origin = transform.position;
					}
				}
			}
			AddRecoil(-1f * recoilAmplitude, -2f * recoilAmplitude, -1f * recoilAmplitude, 1f * recoilAmplitude);
			if (effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(effectPrefab, gameObject, targetMuzzle, false);
			}
			if (isAuthority)
			{
				new BulletAttack
				{
					owner = gameObject,
					weapon = gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = minSpread,
					maxSpread = maxSpread,
					damage = damageCoefficient * damageStat,
					force = force,
					tracerEffectPrefab = tracerEffectPrefab,
					muzzleName = targetMuzzle,
					hitEffectPrefab = hitEffectPrefab,
					isCrit = Util.CheckRoll(critStat, characterBody.master),
					maxDistance = maxDistance,
					radius = radius,
					stopperMask = 0
				}.Fire();
			}
		}

		public override void OnEnter()
		{
			base.OnEnter();
			duration = baseDuration / attackSpeedStat;
			modelTransform = GetModelTransform();
			aimRay = GetAimRay();
			StartAimMode(aimRay, 2f, false);
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (isAuthority)
			{
				fireTimer -= Time.fixedDeltaTime;
				float num = fireDuration / attackSpeedStat / grenadeCountMax;
				if (fireTimer <= 0f && grenadeCount < grenadeCountMax)
				{
					fireTimer += num;
					if (grenadeCount % 2 == 0)
					{
						FireGrenade("MuzzleLeft");
						PlayCrossfade("Gesture, Left Cannon", "FireGrenadeLeft", 0.1f);
					}
					else
					{
						FireGrenade("MuzzleRight");
						PlayCrossfade("Gesture, Right Cannon", "FireGrenadeRight", 0.1f);
					}
					grenadeCount++;
				}
				if (fixedAge >= duration)
				{
					outer.SetNextStateToMain();
				}
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		[MonoModConstructor]
		static NOPE_FireThermite()
		{
			// Note: this type is marked as 'beforefieldinit'.
			grenadeCountMax = 3;
			fireDuration = 1f;
			baseDuration = 2f;
			minSpread = 0f;
			maxSpread = 5f;
			recoilAmplitude = 1f;
		}

		public static GameObject effectPrefab;

		public static GameObject hitEffectPrefab;

		public static int grenadeCountMax;

		public static float damageCoefficient;

		public static float fireDuration;

		public static float baseDuration;

		public static float minSpread;

		public static float maxSpread;

		public static float recoilAmplitude;

		public static string attackSoundString;

		public static float force;

		public static float maxDistance;

		public static float radius;

		public static GameObject tracerEffectPrefab;

		private Ray aimRay;

		private Transform modelTransform;

		private float duration;

		private float fireTimer;

		private int grenadeCount;
	}
}