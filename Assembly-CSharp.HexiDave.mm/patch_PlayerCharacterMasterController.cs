using MonoMod;
using UnityEngine;

namespace RoR2
{
	[MonoModIgnore]
	public class patch_PlayerCharacterMasterController : PlayerCharacterMasterController
	{
		private void FixedUpdate()
		{
			if (!bodyInputs) return;
			
			var localNetworkUser = networkUser;
			
			if (localNetworkUser && localNetworkUser.localUser != null && !localNetworkUser.localUser.isUIFocused)
			{
				var inputPlayer = localNetworkUser.localUser.inputPlayer;
				var isSprinting = false;
				
				if (body)
				{
					isSprinting = body.isSprinting;
					if (sprintInputPressReceived)
					{
						sprintInputPressReceived = false;
						isSprinting = !isSprinting;
					}
					if (isSprinting)
					{
						var aimDirection = bodyInputs.aimDirection;
						aimDirection.y = 0f;
						aimDirection.Normalize();
						var moveVector = bodyInputs.moveVector;
						moveVector.y = 0f;
						moveVector.Normalize();
						if (
							(body.bodyFlags & CharacterBody.BodyFlags.SprintAnyDirection) == 0 &&
							Vector3.Dot(aimDirection, moveVector) < sprintMinAimMoveDot &&
							body.baseNameToken != "HUNTRESS_BODY_NAME")
						{
							isSprinting = false;
						}
					}
				}
				
				bodyInputs.skill1.PushState(inputPlayer.GetButton("PrimarySkill"));
				bodyInputs.skill2.PushState(inputPlayer.GetButton("SecondarySkill"));
				bodyInputs.skill3.PushState(inputPlayer.GetButton("UtilitySkill"));
				bodyInputs.skill4.PushState(inputPlayer.GetButton("SpecialSkill"));
				bodyInputs.interact.PushState(inputPlayer.GetButton("Interact"));
				bodyInputs.jump.PushState(inputPlayer.GetButton("Jump"));
				bodyInputs.sprint.PushState(isSprinting);
				bodyInputs.activateEquipment.PushState(inputPlayer.GetButton("Equipment"));
				bodyInputs.ping.PushState(inputPlayer.GetButton("Ping"));
			}
			else
			{
				bodyInputs.skill1.PushState(false);
				bodyInputs.skill2.PushState(false);
				bodyInputs.skill3.PushState(false);
				bodyInputs.skill4.PushState(false);
				bodyInputs.interact.PushState(false);
				bodyInputs.jump.PushState(false);
				bodyInputs.sprint.PushState(false);
				bodyInputs.activateEquipment.PushState(false);
				bodyInputs.ping.PushState(false);
			}
			CheckPinging();
		}
	}
}