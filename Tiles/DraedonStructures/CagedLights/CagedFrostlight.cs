using CalamityMod.Items.Placeables.DraedonStructures.CagedLights;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures.CagedLights
{
    public class CagedFrostlight : CagedLightTile<CagedLablightItem>
    {
        protected override int ItemDropType => ModContent.ItemType<CagedFrostlightItem>();
        protected override int FixtureDustType => DustID.PlatinumCoin;
        protected override Vector3 LightColor => new Vector3(9f / 255f, 158f / 255f, 238f / 255f);
    }
}
