using CalamityMod.Items.Placeables.FurnitureAcidwood;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAcidwood
{
    public class AcidwoodPlatformTile : BasePlatformTile
    {
        public override void SetStaticDefaults() => this.SetUpPlatform(ModContent.ItemType<AcidwoodPlatform>());

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.WoodFurniture, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

    }
}
