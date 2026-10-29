using CalamityMod.Items.DraedonMisc;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.UI.DraedonLogs
{
    public class DraedonLogHellGUI : DraedonsLogGUI<DraedonsLogHell>
    {
        public override Texture2D GetTextureByPage() =>
            GetTextureByPage("DraedonsLogCragsBiome", null, "DraedonsLogWallOfFlesh");
    }
}
