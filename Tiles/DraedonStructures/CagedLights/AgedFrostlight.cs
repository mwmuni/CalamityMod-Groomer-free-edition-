using CalamityMod.ExtraTextures.GreyscaleGradients;
using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class AgedFrostlight : DustCountingAgedPulsingLightTile<CagedLablightItem>
    {
        protected override int ItemDropType => ModContent.ItemType<AgedFrostlightItem>();
        protected override int FixtureDustType => DustID.PlatinumCoin;
        protected override float Pulse => GreyscaleGradient.ElumplatePulse.GetRepeat((int)Main.GameUpdateCount);
        protected override Vector3 LightColor => new Vector3(9f / 255f, 158f / 255f, 238f / 255f);
    }
}
