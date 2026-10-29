using CalamityMod.ExtraTextures.GreyscaleGradients;
using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class AgedFloodlight : DustCountingAgedPulsingLightTile<CagedLablightItem>
    {
        protected override int ItemDropType => ModContent.ItemType<AgedFloodlightItem>();
        protected override int FixtureDustType => DustID.BlueFlare;
        protected override float Pulse => GreyscaleGradient.NavyplatePulse.GetRepeat((int)Main.GameUpdateCount);
        protected override Vector3 LightColor => new Vector3(105f / 255f, 195f / 255f, 193f / 255f);
    }
}
