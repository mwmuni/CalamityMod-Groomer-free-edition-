using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace CalamityMod.Items.Dyes
{
    public class DefiledFlameDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/DefiledFlameDyeShader"), "DyePass").
            UseColor(new Color(106, 190, 48)).UseSecondaryColor(new Color(204, 248, 48)).UseImage("Images/Misc/Perlin");
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 7, silver: 50); // Sold by Dye Trader
        }
    }
}
