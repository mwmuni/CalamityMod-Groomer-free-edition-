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
    public class PinkCosmicFlameDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/CosmicFlameShader"), "DyePass").
            UseColor(new Color(255, 115, 221)).UseSecondaryColor(new Color(255, 115, 221)).UseImage("Images/Misc/noise").UseSaturation(0f);
        public override void SafeSetDefaults()
        {
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = Item.sellPrice(gold: 1, silver: 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.BottledWater, 3).
                AddIngredient<CosmiliteBar>().
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
