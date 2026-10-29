using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Items.Potions.Food
{
	public class Cometfruit : AmbrosiaFruit
	{
		protected override Color[] FoodParticleColors => new Color[3]
		{
			new Color(153, 100, 184),
			new Color(181, 141, 166),
			new Color(147, 200, 221)
		};
		protected override int FoodWidth => 32;
		protected override int FoodHeight => 32;
		protected override int FoodBuff => BuffID.WellFed2;
		protected override int FoodRarity => ItemRarityID.Green;
	}
}
