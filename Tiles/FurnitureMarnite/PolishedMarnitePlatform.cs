using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureMarnite
{
    public class PolishedMarnitePlatform : BasePlatformTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpPlatform(ModContent.ItemType<Items.Placeables.FurnitureMarnite.PolishedMarnitePlatform>(), true);
            HitSound = SoundID.Tink;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Granite, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

    }
}
