using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureExo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
namespace CalamityMod.Items.Placeables.FurnitureExo
{
    public class ExoPlating : PlaceableTileItem<ExoPlatingTile>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Placeables/FurnitureExo/ExoPlating_Glow").Value);
        }

        public override void AddRecipes()
        {
            CreateRecipe(400).
                AddRecipeGroup("AnyStoneBlock", 400).
                AddIngredient<ExoPrism>().
                AddTile<DraedonsForge>().
                Register();
            CreateRecipe().
                AddIngredient<ExoPlatform>(2).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<ExoPlatingWallItem>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
