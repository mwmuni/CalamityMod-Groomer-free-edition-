using CalamityMod.Items.DraedonMisc;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.UI.DraedonLogs
{
    public class DraedonLogSunkenSeaGUI : DraedonsLogGUI<DraedonsLogSunkenSea>
    {
        public override Texture2D GetTextureByPage() =>
            GetTextureByPage("DraedonsLogSunkenSeaBiome", "DraedonsLogEutrophicRayGhostBell", "DraedonsLogGiantClam");
    }
}
