using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    [LegacyName("ArcticArrow", "VeriumBullet")]
    public class VeriumBolt : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 8, 8, 15, 1.25f,
                Item.sellPrice(copper: 12), ItemRarityID.Pink,
                ModContent.ProjectileType<VeriumBoltProj>(), 16f, AmmoID.Arrow);

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient(ItemID.WoodenArrow, 100).
                AddIngredient<CryonicBar>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
