using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Items.Potions.Food
{
	public class Barberry : AmbrosiaFruit
	{
		protected override Color[] FoodParticleColors => new Color[3]
		{
			new Color(112, 1, 25),
			new Color(136, 69, 137),
			new Color(160, 32, 95)
		};
		protected override int FoodWidth => 26;
		protected override int FoodHeight => 32;
		protected override int FoodBuff => BuffID.WellFed2;
		protected override int FoodRarity => ItemRarityID.Green;
	}
}
