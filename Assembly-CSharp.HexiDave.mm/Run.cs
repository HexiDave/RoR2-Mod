using MonoMod;

namespace RoR2
{
	public class patch_Run : Run
	{
		[MonoModReplace]
		public virtual void OnServerBossKilled(bool bossGroupDefeated)
		{
			if (!bossGroupDefeated) return;

			var teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);

			foreach (var teamMember in teamMembers)
			{
				var body = teamMember.GetComponent<CharacterBody>();
				
				if (!body) continue;

				var networkUser = Util.LookUpBodyNetworkUser(body);
				
				if (!networkUser) continue;
				
				networkUser.AwardLunarCoins(1);
			}
		}
	}
}