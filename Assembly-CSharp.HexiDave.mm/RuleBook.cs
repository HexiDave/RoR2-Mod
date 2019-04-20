#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
#pragma warning disable 108,114
// ReSharper disable CheckNamespace

namespace RoR2
{
	public class patch_RuleBook : RuleBook
	{
		public uint startingMoney => 100;

		public bool keepMoneyBetweenStages => true;
	}
}