using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("VictideBar")]
    public class SeaRemains : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 30;
        protected override int MaterialHeight => 24;
        protected override int SellValue => Item.sellPrice(silver: 36);
        protected override int MaterialRarity => ItemRarityID.Green;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.SortingPriorityMaterials[Type] = 60; // Meteorite
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PearlShard>(2).
                AddIngredient(ItemID.Coral, 2).
                AddIngredient(ItemID.Starfish, 2).
                AddIngredient(ItemID.Seashell, 2).
                AddTile(TileID.Furnaces).
                Register();
        }
    }
}
