using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class BloodfireArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 14, 36, 19, 3.5f,
                Item.sellPrice(copper: 24), ModContent.RarityType<Turquoise>(),
                ModContent.ProjectileType<BloodfireArrowProj>(), 10f, AmmoID.Arrow);

        public override void AddRecipes()
        {
            CreateRecipe(333).
                AddIngredient<Bloodstone>().
                AddIngredient<BloodOrb>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
