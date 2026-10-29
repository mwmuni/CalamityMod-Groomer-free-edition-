using CalamityMod.ExtraTextures.GreyscaleGradients;
using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class MiniAgedFrostlight : MiniAgedPulsingLightTile<MiniAgedFrostlightItem>
    {
        protected override int FixtureDustType => DustID.PlatinumCoin;
        protected override float Pulse => GreyscaleGradient.ElumplatePulse.GetRepeat((int)Main.GameUpdateCount);
        protected override Vector3 LightColor => new Vector3(9f / 255f, 158f / 255f, 238f / 255f);
        protected override float AdjustBrightness(float brightness) => 0.04f + (brightness * 0.156f);
        protected override Vector2 LightPosition(int i, int j) => new Vector2(i, j);
    }
}
