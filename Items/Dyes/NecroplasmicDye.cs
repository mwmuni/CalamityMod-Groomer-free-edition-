using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Dyes
{
    [LegacyName("PhantoplasmDye", "PolterplasmDye")]
    public class NecroplasmicDye : BaseDye
    {
        public override ArmorShaderData ShaderDataToBind => new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Dyes/NecroplasmicDyeShader"), "DyePass").
            UseColor(new Color(245, 143, 182)).UseSecondaryColor(new Color(119, 238, 255)).UseImage("Images/Misc/Perlin");

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(silver: 75);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.BottledWater, 2).
                AddIngredient<Necroplasm>(3).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
