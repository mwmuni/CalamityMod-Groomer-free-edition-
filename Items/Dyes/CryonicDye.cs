using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace CalamityMod.Items.Dyes
{
    // Despite what it may seem with its name, this dye is intended to be based on Cryogen, not the Cryonic set
    public class CryonicDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/CryonicDyeShader"), "DyePass").
            UseColor(new Color(138, 225, 255)).UseSecondaryColor(new Color(90, 90, 204)).UseImage("Images/Misc/Perlin");
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 75);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<CryonicOre>(4).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
