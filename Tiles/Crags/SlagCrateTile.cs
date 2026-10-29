using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Crags
{
    public class SlagCrateTile : BaseFishingCrateTile<SlagCrate>
    {
        protected override Color MapColor => new(189, 152, 43);
        protected override int CrateDustType => DustID.RedTorch;
    }
}
