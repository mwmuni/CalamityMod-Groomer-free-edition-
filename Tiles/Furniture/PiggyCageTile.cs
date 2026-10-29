using CalamityMod.Items.Placeables.Furniture;

namespace CalamityMod.Tiles.Furniture
{
    public class PiggyCageTile : BaseCageTile<PiggyCage>
    {
        protected override string TopTexturePath => "CalamityMod/Tiles/Furniture/TransparentCageTile_Top";
        protected override int AnimationFrameCount => 18;
        protected override int GetAnimationFrameTime(int frame) => frame == 0 || frame == 9 ? 90 : 6;
    }
}
