using System;
using EntityStates;
using MonoMod;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace RoR2
{
	public class patch_HealthComponent : HealthComponent
	{
		[MonoModReplace]
		public void TakeDamage(DamageInfo damageInfo)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealthComponent::TakeDamage(RoR2.DamageInfo)' called on client");
				return;
			}
			if (!alive || godMode)
			{
				return;
			}
			CharacterBody characterBody = null;
			if (damageInfo.attacker)
			{
				characterBody = damageInfo.attacker.GetComponent<CharacterBody>();
			}
			BroadcastMessage("OnIncomingDamage", damageInfo, SendMessageOptions.DontRequireReceiver);
			var master = body.master;
			GetComponent<TeamComponent>();
			if (master && master.inventory)
			{
				var itemCount = master.inventory.GetItemCount((ItemIndex)1);
				if (itemCount > 0 && Util.CheckRoll((1f - 1f / (0.15f * itemCount + 1f)) * 100f, 0f, null))
				{
					var effectData = new EffectData
					{
						origin = damageInfo.position,
						rotation = Util.QuaternionSafeLookRotation((damageInfo.force != Vector3.zero) ? damageInfo.force : Random.onUnitSphere)
					};
					EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/BearProc"), effectData, true);
					damageInfo.rejected = true;
				}
			}
			if (body.HasBuff((BuffIndex)3))
			{
				damageInfo.rejected = true;
			}
			if (body.HasBuff((BuffIndex)23) && (!characterBody || !characterBody.HasBuff((BuffIndex)22)))
			{
				EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/DamageRejected"), new EffectData
				{
					origin = damageInfo.position
				}, true);
				damageInfo.rejected = true;
			}
			if (damageInfo.rejected)
			{
				return;
			}
			var num = damageInfo.damage;
			if (characterBody)
			{
				var master2 = characterBody.master;
				if (master2 && master2.inventory)
				{
					if (combinedHealth >= fullCombinedHealth * 0.9f)
					{
						var itemCount2 = master2.inventory.GetItemCount((ItemIndex)17);
						if (itemCount2 > 0)
						{
							Util.PlaySound("Play_item_proc_crowbar", gameObject);
							num *= 1.5f + 0.3f * (itemCount2 - 1);
							EffectManager.instance.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/ImpactCrowbar"), damageInfo.position, -damageInfo.force, true);
						}
					}
					if (body.isBoss)
					{
						var itemCount3 = master2.inventory.GetItemCount((ItemIndex)61);
						if (itemCount3 > 0)
						{
							num *= 1.2f + 0.1f * (itemCount3 - 1);
							damageInfo.damageColorIndex = (DamageColorIndex)5;
							EffectManager.instance.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/ImpactBossDamageBonus"), damageInfo.position, -damageInfo.force, true);
						}
					}
				}
			}
			if (damageInfo.crit)
			{
				num *= 2f;
			}
			if ((damageInfo.damageType & DamageType.WeakPointHit) != 0)
			{
				num *= 1.5f;
				damageInfo.damageColorIndex = (DamageColorIndex)5;
			}
			if ((damageInfo.damageType & DamageType.BypassArmor) == 0)
			{
				var armor = body.armor;
				var num2 = armor >= 0f ? 1f - armor / (armor + 100f) : 2f - 100f / (100f - armor);
				num = Mathf.Max(1f, num * num2);
			}
			if ((damageInfo.damageType & DamageType.BarrierBlocked) != 0)
			{
				damageInfo.force *= 0.5f;
				var component = GetComponent<IBarrier>();
				component?.BlockedDamage(damageInfo, num);
				damageInfo.procCoefficient = 0f;
				num = 0f;
			}
			if (hasOneshotProtection)
			{
				num = Mathf.Min(num, fullCombinedHealth * 0.40f); // Make this more aggressive; 90% -> 40%
			}
			if ((damageInfo.damageType & DamageType.SlowOnHit) != 0)
			{
				body.AddTimedBuff(0, 2f);
			}
			if ((damageInfo.damageType & DamageType.ClayGoo) != 0 && (body.bodyFlags & CharacterBody.BodyFlags.ImmuneToGoo) == 0)
			{
				body.AddTimedBuff((BuffIndex)21, 2f);
			}
			if (master && master.inventory)
			{
				var itemCount4 = master.inventory.GetItemCount((ItemIndex)44);
				if (itemCount4 > 0)
				{
					var num3 = num / fullCombinedHealth;
					var num4 = (uint)Mathf.Max(master.money * num3 * itemCount4, damageInfo.damage * itemCount4);
					master.money = (uint)Mathf.Max(0f, master.money - num4);
					EffectManager.instance.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CoinImpact"), damageInfo.position, Vector3.up, true);
				}
			}
			if ((damageInfo.damageType & DamageType.NonLethal) != 0)
			{
				num = Mathf.Max(Mathf.Min(num, health - 1f), 0f);
			}
			if (shield > 0f)
			{
				var num5 = Mathf.Min(num, shield);
				var num6 = num - num5;
				Networkshield = Mathf.Max(shield - num5, 0f);
				if (Mathf.Approximately(shield, 0f))
				{
					var scale = 1f;
					if (body)
					{
						scale = body.radius;
					}
					EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShieldBreakEffect"), new EffectData
					{
						origin = transform.position,
						scale = scale
					}, true);
				}
				if (num6 > 0f)
				{
					Networkhealth = health - num6;
				}
			}
			else
			{
				Networkhealth = health - num;
			}
			TakeDamageForce(damageInfo, false);
			SendMessage("OnTakeDamage", damageInfo, SendMessageOptions.DontRequireReceiver);
			var damageReport = new DamageReport
			{
				victim = this,
				damageInfo = damageInfo
			};
			damageReport.damageInfo.damage = num;
			if (num > 0f)
			{
				SendDamageDealt(damageReport);
			}
			UpdateLastHitTime(damageInfo.damage, damageInfo.position, (damageInfo.damageType & (DamageType)2048) > 0);
			if (damageInfo.attacker)
			{
				damageInfo.attacker.SendMessage("OnDamageDealt", damageReport, SendMessageOptions.DontRequireReceiver);
			}
			GlobalEventManager.ServerDamageDealt(damageReport);
			if (isInFrozenState && (body.bodyFlags & (CharacterBody.BodyFlags)16) == 0 && combinedHealthFraction < 0.3f)
			{
				Networkhealth = -1f;
				EffectManager.instance.SpawnEffect(FrozenState.executeEffectPrefab, new EffectData
				{
					origin = body.corePosition,
					scale = body ? body.radius : 1f
				}, true);
			}
			if (!alive)
			{
				BroadcastMessage("OnKilled", damageInfo, SendMessageOptions.DontRequireReceiver);
				if (damageInfo.attacker)
				{
					damageInfo.attacker.SendMessage("OnKilledOther", damageReport, SendMessageOptions.DontRequireReceiver);
				}
				GlobalEventManager.instance.OnCharacterDeath(damageReport);
				return;
			}
			if (master && master.inventory)
			{
				var itemCount5 = master.inventory.GetItemCount((ItemIndex)25);
				if (itemCount5 > 0 && Util.CheckRoll(damageInfo.damage / fullCombinedHealth * 100f, master))
				{
					body.AddTimedBuff((BuffIndex)7, 1.5f + itemCount5 * 1.5f);
					body.AddTimedBuff((BuffIndex)8, 1.5f + itemCount5 * 1.5f);
					EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ProcStealthkit"), new EffectData
					{
						origin = transform.position,
						rotation = Quaternion.identity
					}, true);
				}
			}
		}
	}
}