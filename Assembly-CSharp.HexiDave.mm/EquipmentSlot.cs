using MonoMod;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	public class patch_EquipmentSlot : EquipmentSlot
	{
		[MonoModReplace]
		private bool PerformEquipmentAction(EquipmentIndex equipmentIndex)
		{
					switch (equipmentIndex)
			{
			case EquipmentIndex.CommandMissile:
				remainingMissiles += 12;
				return true;
			case EquipmentIndex.Saw:
			{
				Vector3 position = transform.position;
				Ray ray = new Ray
				{
					direction = inputBank.aimDirection,
					origin = inputBank.aimOrigin
				};
				bool crit = Util.CheckRoll(characterBody.crit, characterBody.master);
				ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("Prefabs/Projectiles/Sawmerang"), ray.origin, Util.QuaternionSafeLookRotation(ray.direction), gameObject, characterBody.damage, 0f, crit, DamageColorIndex.Default, null, -1f);
				return true;
			}
			case EquipmentIndex.Fruit:
				if (healthComponent)
				{
					Util.PlaySound("Play_item_use_fruit", gameObject);
					EffectData effectData = new EffectData();
					effectData.origin = transform.position;
					effectData.SetNetworkedObjectReference(gameObject);
					EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/FruitHealEffect"), effectData, true);
					healthComponent.HealFraction(0.5f, default(ProcChainMask));
					return true;
				}
				return true;
			case EquipmentIndex.Meteor:
			{
				MeteorStormController component = Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/MeteorStorm"), characterBody.corePosition, Quaternion.identity).GetComponent<MeteorStormController>();
				component.owner = gameObject;
				component.ownerDamage = characterBody.damage;
				component.isCrit = Util.CheckRoll(characterBody.crit, characterBody.master);
				NetworkServer.Spawn(component.gameObject);
				return true;
			}
			case EquipmentIndex.SoulJar:
				return true;
			case EquipmentIndex.Blackhole:
			{
				Vector3 position2 = transform.position;
				Ray ray2 = new Ray
				{
					direction = inputBank.aimDirection,
					origin = inputBank.aimOrigin
				};
				ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("Prefabs/Projectiles/GravSphere"), position2, Util.QuaternionSafeLookRotation(ray2.direction), gameObject, 0f, 0f, false, DamageColorIndex.Default, null, -1f);
				return true;
			}
			case EquipmentIndex.GhostGun:
			{
				GameObject gameObject = Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GhostGun"), transform.position, Quaternion.identity);
				gameObject.GetComponent<GhostGunController>().owner = this.gameObject;
				NetworkServer.Spawn(gameObject);
				return true;
			}
			case EquipmentIndex.CritOnUse:
				characterBody.AddTimedBuff(BuffIndex.FullCrit, 8f);
				return true;
			case EquipmentIndex.DroneBackup:
			{
				Util.PlaySound("Play_item_use_radio", gameObject);
				int num = 4;
				float num2 = 25f;
				if (NetworkServer.active)
				{
					for (int i = 0; i < num; i++)
					{
						Vector2 vector = Random.insideUnitCircle.normalized * 3f;
						Vector3 position3 = transform.position + new Vector3(vector.x, 0f, vector.y);
						SummonMaster(Resources.Load<GameObject>("Prefabs/CharacterMasters/DroneBackupMaster"), position3).gameObject.AddComponent<MasterSuicideOnTimer>().lifeTimer = num2 + Random.Range(0f, 3f);
					}
					return true;
				}
				return true;
			}
			case EquipmentIndex.OrbitalLaser:
			{
				Vector3 position4 = transform.position;
				RaycastHit raycastHit;
				if (Physics.Raycast(new Ray
				{
					direction = inputBank.aimDirection,
					origin = inputBank.aimOrigin
				}, out raycastHit, 900f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask))
				{
					position4 = raycastHit.point;
				}
				GameObject gameObject2 = (GameObject)Instantiate(Resources.Load("Prefabs/NetworkedObjects/OrbitalLaser"), position4, Quaternion.identity);
				gameObject2.GetComponent<OrbitalLaserController>().ownerBody = characterBody;
				NetworkServer.Spawn(gameObject2);
				return true;
			}
			case EquipmentIndex.BFG:
				bfgChargeTimer = 2f;
				subcooldownTimer = 2.2f;
				return true;
			case EquipmentIndex.Enigma:
			{
				EquipmentIndex equipmentIndex2 = EquipmentCatalog.enigmaEquipmentList[Random.Range(0, EquipmentCatalog.enigmaEquipmentList.Count - 1)];
				PerformEquipmentAction(equipmentIndex2);
				return true;
			}
			case EquipmentIndex.Jetpack:
			{
				JetpackController jetpackController = JetpackController.FindJetpackController(gameObject);
				if (!jetpackController)
				{
					GameObject gameObject3 = Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/JetpackController"));
					jetpackController = gameObject3.GetComponent<JetpackController>();
					jetpackController.NetworktargetObject = gameObject;
					NetworkServer.Spawn(gameObject3);
					return true;
				}
				jetpackController.ResetTimer();
				return true;
			}
			case EquipmentIndex.Lightning:
			{
				HurtBox hurtBox = currentTargetHurtBox;
				if (hurtBox)
				{
					subcooldownTimer = 0.2f;
					OrbManager.instance.AddOrb(new LightningStrikeOrb
					{
						attacker = gameObject,
						damageColorIndex = DamageColorIndex.Item,
						damageValue = characterBody.damage * 30f,
						isCrit = Util.CheckRoll(characterBody.crit, characterBody.master),
						procChainMask = default(ProcChainMask),
						procCoefficient = 1f,
						target = hurtBox
					});
					return true;
				}
				return false;
			}
			case EquipmentIndex.PassiveHealing:
				if (passiveHealingFollower)
				{
					passiveHealingFollower.AssignNewTarget(currentTargetBodyObject);
					return true;
				}
				return true;
			case EquipmentIndex.BurnNearby:
				if (characterBody)
				{
					characterBody.AddHelfireDuration(8f);
					return true;
				}
				return true;
			case EquipmentIndex.SoulCorruptor:
			{
				HurtBox hurtBox2 = currentTargetHurtBox;
				if (!hurtBox2)
				{
					return false;
				}
				if (!hurtBox2.healthComponent || hurtBox2.healthComponent.combinedHealthFraction > 0.25f)
				{
					return false;
				}
				Util.TryToCreateGhost(hurtBox2.healthComponent.body, characterBody, 30);
				hurtBox2.healthComponent.Suicide(gameObject);
				return true;
			}
			case EquipmentIndex.Scanner:
				NetworkServer.Spawn(Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/ChestScanner"), characterBody.corePosition, Quaternion.identity));
				return true;
			case EquipmentIndex.CrippleWard:
				NetworkServer.Spawn(Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/CrippleWard"), characterBody.corePosition, Quaternion.identity));
				// Don't drop it for now - can't pick back up (bug)
				// inventory.SetEquipmentIndex(EquipmentIndex.None);
				return true;
			}
			return false;
		}
	}
}