using CalamityMod.Items.Placeables.FurnitureProfaned;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Dyes
{
    public class ProfanedRockDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/ProfanedRockDyeShader"), "DyePass").
            UseColor(new Color(79, 16, 16)).UseSecondaryColor(new Color(173, 116, 0)).UseImage(Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Veins"));
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 1, silver: 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.BottledWater, 3).
                AddIngredient<ProfanedRock>(15).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
