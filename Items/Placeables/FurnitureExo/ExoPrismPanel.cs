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
    public class ExoPrismPanel : PlaceableTileItem<ExoPrismPanelTile>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Placeables/FurnitureExo/ExoPrismPanel_Glow").Value);
        }

        public override void AddRecipes()
        {
            CreateRecipe(400).
                AddIngredient(ItemID.Glass, 400).
                AddIngredient<ExoPrism>().
                AddTile<DraedonsForge>().
                Register();
            CreateRecipe().
                AddIngredient<ExoPrismPlatform>(2).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient<ExoPrismPanelWallItem>(4).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
