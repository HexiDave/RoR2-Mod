using EntityStates.Huntress;
using MonoMod;
using RoR2;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	public class patch_PrepWall : BaseState
	{
		public void OnEnter()
		{
			base.OnEnter();
			stopwatch2 = 0f;
			endDuration = IceNova.baseEndDuration / attackSpeedStat;
			startDuration = IceNova.baseStartDuration / attackSpeedStat;
			Util.PlaySound(BlinkState.beginSoundString, gameObject);
			modelTransform = GetModelTransform();
			if (modelTransform)
			{
				characterModel = modelTransform.GetComponent<CharacterModel>();
				hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
			}
			if (characterModel)
			{
				characterModel.invisibilityCount++;
			}
			if (hurtboxGroup)
			{
				HurtBoxGroup hurtBoxGroup = hurtboxGroup;
				int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
				hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
			}
			blinkVector = inputBank.aimDirection;
			CreateBlinkEffect(Util.GetCorePosition(gameObject));
		}

		public void FixedUpdate()
		{
			base.FixedUpdate();
			stopwatch += Time.fixedDeltaTime;
			if (characterMotor && characterDirection)
			{
				characterMotor.velocity = Vector3.zero;
				characterMotor.rootMotion += blinkVector * (moveSpeedStat * BlinkState.speedCoefficient * Time.fixedDeltaTime);
			}
			if (stopwatch >= BlinkState.duration && isAuthority)
			{
				outer.SetNextStateToMain();
			}
			stopwatch2 += Time.fixedDeltaTime;
			if (stopwatch2 >= startDuration && !hasCastNova)
			{
				hasCastNova = true;
				EffectManager.instance.SpawnEffect(IceNova.novaEffectPrefab, new EffectData
				{
					origin = transform.position,
					scale = 12f
				}, true);
				BlastAttack blastAttack = new BlastAttack();
				blastAttack.radius = 12f;
				blastAttack.procCoefficient = IceNova.procCoefficient;
				blastAttack.position = transform.position;
				blastAttack.attacker = gameObject;
				blastAttack.crit = Util.CheckRoll(characterBody.crit, characterBody.master);
				blastAttack.baseDamage = characterBody.damage * IceNova.damageCoefficient;
				blastAttack.falloffModel = 0;
				blastAttack.damageType = DamageType.Freeze2s;
				blastAttack.baseForce = IceNova.force;
				blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
				blastAttack.Fire();
			}
			if (stopwatch2 >= startDuration + endDuration && isAuthority)
			{
				outer.SetNextStateToMain();
			}
		}

		[MonoModRemove]
		public void Update()
		{
			
		}

		public void OnExit()
		{
			if (!outer.destroying)
			{
				Util.PlaySound(BlinkState.endSoundString, gameObject);
				CreateBlinkEffect(Util.GetCorePosition(gameObject));
				modelTransform = GetModelTransform();
				if (modelTransform)
				{
					TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
					temporaryOverlay.duration = 0.6f;
					temporaryOverlay.animateShaderAlpha = true;
					temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
					temporaryOverlay.destroyComponentOnEnd = true;
					temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashBright");
					temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
					TemporaryOverlay temporaryOverlay2 = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
					temporaryOverlay2.duration = 0.7f;
					temporaryOverlay2.animateShaderAlpha = true;
					temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
					temporaryOverlay2.destroyComponentOnEnd = true;
					temporaryOverlay2.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashExpanded");
					temporaryOverlay2.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
				}
			}
			if (characterModel)
			{
				characterModel.invisibilityCount--;
			}
			if (hurtboxGroup)
			{
				HurtBoxGroup hurtBoxGroup = hurtboxGroup;
				int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
				hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
			}
			base.OnExit();
		}

		private void CreateBlinkEffect(Vector3 origin)
		{
			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(blinkVector);
			effectData.origin = origin;
			EffectManager.instance.SpawnEffect(BlinkState.blinkPrefab, effectData, false);
		}

		[MonoModConstructor]
		static patch_PrepWall()
		{
			damageCoefficient = 1.2f;
		}

		public static float damageCoefficient;

		public static float duration = 0.3f;

		private float stopwatch;

		private Transform modelTransform;

		public static GameObject blinkPrefab;

		private Vector3 blinkVector = Vector3.zero;

		public static float speedCoefficient = 25f;

		public static string beginSoundString;

		public static string endSoundString;

		private CharacterModel characterModel;

		private HurtBoxGroup hurtboxGroup;

		private float startDuration;

		private float endDuration;

		public static float baseEndDuration = 2f;

		private float stopwatch2;

		private bool hasCastNova;
	}
}
