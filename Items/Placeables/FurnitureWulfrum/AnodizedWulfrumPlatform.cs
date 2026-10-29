using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.FurnitureWulfrum
{
    public class AnodizedWulfrumPlatform : PlaceableTileItem<Tiles.FurnitureWulfrum.AnodizedWulfrumPlatform>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }


        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Placeables/FurnitureWulfrum/AnodizedWulfrumPlatform_Glow").Value);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<AnodizedWulfrumMetal>().
                Register();
        }
    }
}
