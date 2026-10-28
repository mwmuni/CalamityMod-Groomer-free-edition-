using CalamityMod.ExtraTextures.GreyscaleGradients;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Plates
{
    [LegacyName("Chaosplate")]
    public class Havocplate : BaseGlowMaskRedTorchStoneDustTile
    {
        public override void SetupStatic()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);

            HitSound = CommonCalamitySounds.PlatingMine;
            MineResist = 1f;
            AddMapEntry(new Color(235, 108, 108));
        }

        public override void RandomUpdate(int i, int j)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.RedTorch, 0f, 0f, 1, new Color(255, 255, 255), 1f);
        }

        public override Color GetGlowMaskColor(int i, int j, TileDrawInfo drawData)
        {
            float brightness = GreyscaleGradient.HavocplatePulse.GetRepeat((int)Main.GameUpdateCount);
            brightness = 0.04f + (brightness * 0.156f);
            return Color.White * brightness;
        }
    }
}
