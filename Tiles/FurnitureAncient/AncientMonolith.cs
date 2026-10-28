using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAncient
{
    public class AncientMonolith : BaseRedTorchStoneDustTile
    {
        int animationFrameWidth = 36;
        public override void SetStaticDefaults() => this.SetUpClock(ModContent.ItemType<Items.Placeables.FurnitureAncient.AncientMonolith>(), true);

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.9f;
            g = 0.3f;
            b = 0.3f;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int uniqueAnimationFrame = Main.tileFrame[Type] + i;
            uniqueAnimationFrame = (uniqueAnimationFrame % 35) + 1;

            frameXOffset = uniqueAnimationFrame * animationFrameWidth;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 6)
            {
                frameCounter = 0;
                frame++;
                if (frame > 35)
                {
                    frame = 1;
                }
            }
        }

        public override bool RightClick(int x, int y)
        {
            return FurnitureCommon.ClockRightClick();
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                Main.SceneMetrics.HasClock = true;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void MouseOver(int i, int j) => FurnitureCommon.MouseOver(i, j, ModContent.ItemType<Items.Placeables.FurnitureAncient.AncientMonolith>());
    }
}
