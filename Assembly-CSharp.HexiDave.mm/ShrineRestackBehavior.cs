namespace RoR2
{
	public class patch_ShrineRestackBehavior : ShrineRestackBehavior
	{
		private extern void orig_Start();
		
		private void Start()
		{
			orig_Start();

			maxPurchaseCount = 10;
			costMultiplierPerPurchase = 1f;
		}
	}
}