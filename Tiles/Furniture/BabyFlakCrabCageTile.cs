using CalamityMod.Items.Placeables.Furniture;

namespace CalamityMod.Tiles.Furniture
{
    public class BabyFlakCrabCageTile : BaseCageTile<BabyFlakCrabCage>
    {
        protected override string TopTexturePath => "CalamityMod/Tiles/Furniture/TransparentCageTile_Top";
        protected override int AnimationFrameCount => 34;
        protected override int GetAnimationFrameTime(int frame) =>
            frame == 0 || frame == 16 ? 90 : frame == 28 ? 60 : 6;
    }
}
