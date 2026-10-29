using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Items.Potions.Food
{
	public class Jackfruit : AmbrosiaFruit
	{
		protected override Color[] FoodParticleColors => new Color[3]
		{
			new Color(250, 216, 80),
			new Color(201, 207, 79),
			new Color(147, 176, 54)
		};
		protected override int FoodWidth => 26;
		protected override int FoodHeight => 36;
		protected override int FoodBuff => BuffID.WellFed;
		protected override int FoodRarity => ItemRarityID.Blue;
	}
}
