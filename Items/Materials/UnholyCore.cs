using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class UnholyCore : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 26;
        protected override int MaterialHeight => 46;
        protected override int SellValue => Item.sellPrice(silver: 80);
        protected override int MaterialRarity => ItemRarityID.Pink;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.SortingPriorityMaterials[Type] = 90; // Chlorophyte Ore
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<InfernalSuevite>(3).
                AddIngredient(ItemID.Hellstone, 2).
                AddTile(TileID.Hellforge).
                Register();
        }
    }
}
