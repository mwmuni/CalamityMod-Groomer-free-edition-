using CalamityMod.Items.Placeables.Furniture;

namespace CalamityMod.Tiles.Furniture
{
    public class ShroombleCageTile : BaseCageTile<ShroombleCage>
    {
        protected override string TopTexturePath => "CalamityMod/Tiles/Furniture/TransparentCageTile_Top";
        protected override int AnimationFrameCount => 39;
    }
}
