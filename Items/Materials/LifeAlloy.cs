using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("BarofLife")]
    public class LifeAlloy : BaseMaterial
    {
        protected override int ResearchCount => 25;
        protected override int MaterialWidth => 30;
        protected override int MaterialHeight => 24;
        protected override int SellValue => Item.sellPrice(gold: 3);
        protected override int MaterialRarity => ItemRarityID.Yellow;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.SortingPriorityMaterials[Type] = 95; // Stardust Fragment
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryonicBar>().
                AddIngredient<PerennialBar>().
                AddIngredient<ScoriaBar>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
