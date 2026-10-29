using CalamityMod.Items.Placeables.Furniture;

namespace CalamityMod.Tiles.Furniture
{
    public class PiggyCageGoldTile : BaseCageTile<PiggyCageGold>
    {
        protected override string TopTexturePath => "CalamityMod/Tiles/Furniture/TransparentGoldCageTile_Top";
        protected override int AnimationFrameCount => 18;
        protected override int GetAnimationFrameTime(int frame) => frame == 0 || frame == 9 ? 90 : 6;
    }
}
