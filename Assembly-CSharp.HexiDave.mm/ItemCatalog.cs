using MonoMod;

namespace RoR2
{
	public static class patch_ItemCatalog
	{
		private static extern void orig_DefineItems();

		private static void DefineItems()
		{
			orig_DefineItems();

			foreach (var itemDef in ItemCatalog.itemDefs)
			{
				itemDef.unlockableName = "";
			}
		}
	}
}