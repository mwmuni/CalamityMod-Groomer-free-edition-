using Microsoft.Xna.Framework;
using Terraria.ID;

namespace CalamityMod.Items.Potions.Food
{
	public class Lotus : AmbrosiaFruit
	{
		protected override Color[] FoodParticleColors => new Color[3]
		{
			new Color(198, 83, 236),
			new Color(207, 81, 108),
			new Color(233, 184, 87)
		};
		protected override int FoodWidth => 26;
		protected override int FoodHeight => 32;
		protected override int FoodBuff => BuffID.WellFed2;
		protected override int FoodRarity => ItemRarityID.Green;
	}
}
