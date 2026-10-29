using CalamityMod.Items.Placeables.FurnitureExo;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureExo
{
    public class ExoPlatformTile : BasePlatformTile
    {
        public override void SetStaticDefaults() => this.SetUpPlatform(ModContent.ItemType<ExoPlatform>(), true);

        public override bool CanExplode(int i, int j) => false;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.TerraBlade, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

    }
}
