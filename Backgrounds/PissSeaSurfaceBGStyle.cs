using Terraria.ModLoader;

namespace CalamityMod.Backgrounds
{
    public class PissSeaSurfaceBGStyle : SurfaceBackgroundStyleBase
    {
        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            b -= 250f;
            return BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Backgrounds/PissSeaSurfaceClose");
        }

    }
}
