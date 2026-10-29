using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Items.Potions.Food
{
	public class Salak : AmbrosiaFruit
	{
		protected override Color[] FoodParticleColors => new Color[3]
		{
			new Color(203, 134, 81),
			new Color(162, 109, 68),
			new Color(253, 242, 233)
		};
		protected override int FoodWidth => 28;
		protected override int FoodHeight => 26;
		protected override int FoodBuff => BuffID.WellFed;
		protected override int FoodRarity => ItemRarityID.Blue;
	}
}
