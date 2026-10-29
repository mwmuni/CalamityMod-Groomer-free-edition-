using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    [LegacyName("IcyBullet")]
    public class HailstormBullet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Frostburn2, BuffID.Frozen];
            Item.ResearchUnlockCount = 99;
        }
        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 14, 20, 9, 2f,
                Item.sellPrice(copper: 16), ItemRarityID.Yellow,
                ModContent.ProjectileType<HailstormBulletProj>(), 0.3f, AmmoID.Bullet);
        public override void AddRecipes()
        {
            CreateRecipe(333).
                AddIngredient(ItemID.MusketBall, 333).
                AddIngredient<EssenceofEleum>().
                AddIngredient(ItemID.Ectoplasm).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
