using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    public class EndothermicDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/EndothermicDyeShader"), "DyePass").
            UseColor(new Color(123, 205, 237)).UseSecondaryColor(new Color(85, 85, 171));
        public override void SafeSetDefaults()
        {
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 1, silver: 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.BottledWater, 3).
                AddIngredient<EndothermicEnergy>(5).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
