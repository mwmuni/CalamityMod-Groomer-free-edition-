using CalamityMod.Items.DraedonMisc;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.UI.DraedonLogs
{
    public class DraedonLogJungleGUI : DraedonsLogGUI<DraedonsLogJungle>
    {
        public override Texture2D GetTextureByPage() =>
            GetTextureByPage("DraedonsLogJungleBiome", "DraedonsLogPlagueCell", "DraedonsLogPlaguebringerGoliath");
    }
}
