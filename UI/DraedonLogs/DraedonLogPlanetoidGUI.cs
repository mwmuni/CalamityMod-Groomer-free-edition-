using CalamityMod.Items.DraedonMisc;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.UI.DraedonLogs
{
    public class DraedonLogPlanetoidGUI : DraedonsLogGUI<DraedonsLogPlanetoid>
    {
        public override Texture2D GetTextureByPage() =>
            GetTextureByPage("DraedonsLogPlanetoid", null, "DraedonsLogDoGArmor");
    }
}
