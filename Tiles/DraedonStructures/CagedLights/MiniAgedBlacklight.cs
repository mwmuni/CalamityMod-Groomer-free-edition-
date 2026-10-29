using CalamityMod.ExtraTextures.GreyscaleGradients;
using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class MiniAgedBlacklight : MiniAgedPulsingLightTile<MiniAgedBlacklightItem>
    {
        protected override int FixtureDustType => DustID.ShadowbeamStaff;
        protected override float Pulse => GreyscaleGradient.OnyxplatePulse.GetRepeat((int)Main.GameUpdateCount);
        protected override Vector3 LightColor => new Vector3(182f / 255f, 28f / 255f, 232f / 255f);
    }
}
