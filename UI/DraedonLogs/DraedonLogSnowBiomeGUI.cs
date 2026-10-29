using CalamityMod.Items.DraedonMisc;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.UI.DraedonLogs
{
    public class DraedonLogSnowBiomeGUI : DraedonsLogGUI<DraedonsLogSnowBiome>
    {
        public override Texture2D GetTextureByPage() =>
            GetTextureByPage("DraedonsLogIceBiome", null, "DraedonsLogPermafrost");
    }
}
