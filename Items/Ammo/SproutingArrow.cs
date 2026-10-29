using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    [LegacyName("TerraArrow")]
    public class SproutingArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 22, 36, 14, 1.5f,
                Item.sellPrice(copper: 20), ItemRarityID.Lime,
                ModContent.ProjectileType<SproutingArrowMain>(), 15f, AmmoID.Arrow);

        public override void AddRecipes()
        {
            CreateRecipe(250).
                AddIngredient(ItemID.WoodenArrow, 250).
                AddIngredient<LivingShard>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
