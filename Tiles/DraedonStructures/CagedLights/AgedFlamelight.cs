using CalamityMod.ExtraTextures.GreyscaleGradients;
using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class AgedFlamelight : DustCountingAgedPulsingLightTile<CagedLablightItem>
    {
        protected override int ItemDropType => ModContent.ItemType<AgedFlamelightItem>();
        protected override int FixtureDustType => DustID.RedTorch;
        protected override float Pulse => GreyscaleGradient.HavocplatePulse.GetRepeat((int)Main.GameUpdateCount);
        protected override Vector3 LightColor => new Vector3(255f / 255f, 104f / 255f, 104f / 255f);
    }
}
