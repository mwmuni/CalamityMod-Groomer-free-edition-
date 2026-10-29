using CalamityMod.Items.Placeables.FurnitureSacrilegious;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureSacrilegious
{
    // TODO -- why does this one piece of Otherworldly furniture still use the old naming scheme
    public class OccultPlatformTile : BasePlatformTile
    {
        public override void SetStaticDefaults() => this.SetUpPlatform(ModContent.ItemType<OccultPlatformItem>(), true);

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Iron, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

    }
}
