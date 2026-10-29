using CalamityMod.Items.Materials;
using CalamityMod.Tiles.LivingFire;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;

namespace CalamityMod.Items.Placeables.LivingFire
{
    public class LivingGodSlayerFireBlock : PlaceableTileItem<LivingGodSlayerFireBlockTile>
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }


        public override void PostUpdate()
        {
            Lighting.AddLight((int)((Item.position.X + Item.width / 2) / 16f), (int)((Item.position.Y + Item.height / 2) / 16f), 1f, 0f, 1f);
        }

        public override void AddRecipes()
        {
            CreateRecipe(20).
                AddIngredient(ItemID.LivingFireBlock, 20).
                AddIngredient<CosmiliteBar>().
                AddTile(TileID.CrystalBall).
                Register();
        }
    }
}
