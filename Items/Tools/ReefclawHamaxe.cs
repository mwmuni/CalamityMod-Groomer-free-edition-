using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class ReefclawHamaxe : BaseMeleeTool
    {
        public override void SetDefaults()
        {
            SetMeleeToolDefaults(
                44, 44, 15, 6f, 11, 29, Item.sellPrice(silver: 30), ItemRarityID.Green,
                axePower: 55 / 5, hammerPower: 60);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaRemains>(2).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
