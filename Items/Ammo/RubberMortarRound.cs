using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class RubberMortarRound : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 20, 14, 8, 7f,
                Item.sellPrice(copper: 20), ItemRarityID.Yellow,
                ModContent.ProjectileType<RubberMortarRoundProj>(), 0f, AmmoID.Bullet);

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient<MortarRound>(100).
                AddIngredient(ItemID.PinkGel, 5).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
