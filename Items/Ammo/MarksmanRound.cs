using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class MarksmanRound : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 12, 26, 12, 2.25f,
                Item.sellPrice(copper: 10), ItemRarityID.LightRed,
                ModContent.ProjectileType<MarksmanShot>(), 1f, AmmoID.Bullet);

        public override void AddRecipes()
        {
            CreateRecipe(999).
                AddIngredient(ItemID.GoldenBullet, 999).
                AddIngredient(ItemID.GoldCoin).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
