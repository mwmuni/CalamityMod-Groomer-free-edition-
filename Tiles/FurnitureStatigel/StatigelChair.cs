using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureStatigel
{
    public class StatigelChair : BaseChairTile<Items.Placeables.FurnitureStatigel.StatigelChair>
    {
        protected override bool LavaImmune => false;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.PinkSlime, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }
    }
}
