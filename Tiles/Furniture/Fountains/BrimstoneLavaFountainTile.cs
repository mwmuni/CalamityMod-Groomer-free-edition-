using CalamityMod.Items.Placeables.Furniture.Fountains;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture.Fountains
{
    public class BrimstoneLavaFountainTile : BaseFountainTile<BrimstoneLavaFountainItem>
    {
        protected override Color MapColor => new Color(251, 136, 130);
        protected override bool IsLava => true;
        protected override int SecondDustType => DustID.Lava;
        protected override int AnimationFrameCount => 6;

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            if (player is null || Main.dedServ)
                return;

            if (Main.tile[i, j].TileFrameX >= 36)
                player.Calamity().BrimstoneLavaFountainCounter = 5;
        }
    }
}
