using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace CalamityMod.Items.Dyes
{
    public class ElementalDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/ElementalDyeShader"), "DyePass").UseImage("Images/Misc/Perlin");
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe(5).
                AddIngredient(ItemID.SolarDye).
                AddIngredient(ItemID.VortexDye).
                AddIngredient(ItemID.NebulaDye).
                AddIngredient(ItemID.StardustDye).
                AddIngredient<MeldDye>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
