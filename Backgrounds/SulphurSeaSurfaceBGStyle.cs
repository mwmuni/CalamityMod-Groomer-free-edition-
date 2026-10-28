using Terraria.ModLoader;

namespace CalamityMod.Backgrounds
{
    public class SulphurSeaSurfaceBGStyle : SurfaceBackgroundStyleBase
    {
        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            b -= 100f;
            return BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Backgrounds/BlankPixel");
        }

    }
}
