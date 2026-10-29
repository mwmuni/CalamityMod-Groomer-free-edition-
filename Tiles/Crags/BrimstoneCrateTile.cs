using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Crags
{
    public class BrimstoneCrateTile : BaseFishingCrateTile<BrimstoneCrate>
    {
        protected override Color MapColor => new(128, 0, 0);
        protected override int CrateDustType => DustID.RedTorch;
    }
}
