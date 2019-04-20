using MonoMod;
using RoR2;
using UnityEngine;

namespace EntityStates.TitanMonster
{
	[MonoModIgnore]
	public class patch_FireMegaLaser : BaseState
	{
		public GameObject effectPrefab;

		public GameObject hitEffectPrefab;

		public GameObject laserPrefab;

		public static string playAttackSoundString;

		public static string playLoopSoundString;

		public static string stopLoopSoundString;

		public static float damageCoefficient;

		public static float force;

		public static float minSpread;

		public static float maxSpread;

		public static int bulletCount;

		public static float fireFrequency;

		public static float maxDistance;

		public static float minimumDuration;

		public static float maximumDuration;

		public static float lockOnAngle;

		private HurtBox lockedOnHurtBox;

		private float fireStopwatch;

		private float stopwatch;

		private Ray aimRay;

		private Transform modelTransform;

		private GameObject laserEffect;

		private ChildLocator laserChildLocator;

		private Transform laserEffectEnd;

		public int bulletCountCurrent;

		protected Transform muzzleTransform;

		private float lockOnTestFrequency;

		private float lockOnStopwatch;

		private BullseyeSearch enemyFinder;

		private bool foundAnyTarget;

		[MonoModReplace]
		private void FireBullet(Transform modelTransform, Ray aimRay, string targetMuzzle, float maxDistance)
		{
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
					minSpread = FireMegaLaser.minSpread,
					maxSpread = FireMegaLaser.maxSpread,
					bulletCount = 1u,
					damage = FireMegaLaser.damageCoefficient * damageStat / FireMegaLaser.fireFrequency,
					force = FireMegaLaser.force,
					muzzleName = targetMuzzle,
					hitEffectPrefab = hitEffectPrefab,
					isCrit = Util.CheckRoll(critStat, characterBody.master),
					HitEffectNormal = false,
					radius = 0f,
					maxDistance = maxDistance
				}.Fire();
			}
		}

		[MonoModReplace]
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			fireStopwatch += Time.fixedDeltaTime;
			stopwatch += Time.fixedDeltaTime;
			lockOnStopwatch += Time.fixedDeltaTime;
			aimRay = GetAimRay();
			if (isAuthority && !lockedOnHurtBox && foundAnyTarget)
			{
				outer.SetNextState(new FireMegaLaser
				{
					stopwatch = stopwatch
				});
				return;
			}

			var vector = aimRay.origin;
			if (muzzleTransform)
			{
				vector = muzzleTransform.position;
			}

			var vector2 = aimRay.direction;
			RaycastHit raycastHit;

			var usedLockOn = false;
			if (lockedOnHurtBox)
			{
				var lockOnPos = lockedOnHurtBox.transform.position;
				var lockOnDir = (lockOnPos - vector).normalized;

				if (Vector3.Angle(aimRay.direction, lockOnDir) < 60f)
				{
					usedLockOn = true;
					vector2 = lockOnPos;
				}
			}

			if (!usedLockOn)
			{
				if (Util.CharacterRaycast(gameObject, aimRay, out raycastHit, maxDistance,
					LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
				{
					vector2 = raycastHit.point;
				}
				else
				{
					vector2 = aimRay.GetPoint(maxDistance);
				}
			}

			var ray = new Ray(vector, vector2 - vector);
			var flag = false;
			if (laserEffect && laserChildLocator)
			{
				RaycastHit raycastHit2;
				if (Util.CharacterRaycast(gameObject, ray, out raycastHit2, (vector2 - vector).magnitude,
					LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
				{
					vector2 = raycastHit2.point;
					RaycastHit raycastHit3;
					if (Util.CharacterRaycast(gameObject, new Ray(vector2 - ray.direction * 0.1f, -ray.direction),
						out raycastHit3, raycastHit2.distance, LayerIndex.world.mask | LayerIndex.entityPrecise.mask,
						QueryTriggerInteraction.UseGlobal))
					{
						vector2 = ray.GetPoint(0.1f);
						flag = true;
					}
				}

				laserEffect.transform.rotation = Util.QuaternionSafeLookRotation(vector2 - vector);
				laserEffectEnd.transform.position = vector2;
			}

			if (fireStopwatch > 1f / fireFrequency)
			{
				const string targetMuzzle = "MuzzleLaser";
				if (!flag)
				{
					FireBullet(modelTransform, ray, targetMuzzle, (vector2 - ray.origin).magnitude + 0.1f);
				}

				fireStopwatch -= 1f / fireFrequency;
			}

			if (isAuthority && (((!inputBank || !inputBank.skill4.down) && stopwatch > minimumDuration) ||
			                    stopwatch > maximumDuration))
			{
				outer.SetNextStateToMain();
			}
		}
	}
}