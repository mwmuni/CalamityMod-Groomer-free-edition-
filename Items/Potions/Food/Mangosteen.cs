using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Items.Potions.Food
{
	public class Mangosteen : AmbrosiaFruit
	{
		protected override Color[] FoodParticleColors => new Color[3]
		{
			new Color(218, 238, 253),
			new Color(178, 199, 214),
			new Color(123, 99, 130)
		};
		protected override int FoodWidth => 30;
		protected override int FoodHeight => 32;
		protected override int FoodBuff => BuffID.WellFed2;
		protected override int FoodRarity => ItemRarityID.Green;
	}
}
