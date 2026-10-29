using CalamityMod.ExtraTextures.GreyscaleGradients;
using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class AgedCinderlight : DustCountingAgedPulsingLightTile<CagedLablightItem>
    {
        protected override int ItemDropType => ModContent.ItemType<AgedCinderlightItem>();
        protected override int FixtureDustType => DustID.Torch;
        protected override float Pulse => GreyscaleGradient.CinderplatePulse.GetRepeat((int)Main.GameUpdateCount);
        protected override Vector3 LightColor => new Vector3(252f / 255f, 184f / 255f, 29f / 255f);
        protected override Vector2 LightPosition(int i, int j) => new Vector2(i, j);
    }
}
