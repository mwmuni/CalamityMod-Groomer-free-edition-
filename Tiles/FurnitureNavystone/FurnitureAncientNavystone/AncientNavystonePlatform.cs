using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureNavystone.FurnitureAncientNavystone
{
    [LegacyName("EutrophicPlatform")]
    public class AncientNavystonePlatform : BasePlatformTile
    {
        public override void SetStaticDefaults() => this.SetUpPlatform(ModContent.ItemType<Items.Placeables.FurnitureNavystone.FurnitureAncientNavystone.AncientNavystonePlatform>());

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.SnowBlock, 0f, 0f, 1, new Color(54, 69, 72), 1f);
            return false;
        }

    }
}
