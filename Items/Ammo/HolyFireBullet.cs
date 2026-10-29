using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class HolyFireBullet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public static readonly SoundStyle Explosion = new("CalamityMod/Sounds/Item/HolyFireBulletExplosion") { PitchVariance = 0.2f, Volume = 0.6f };
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HolyFlames>()];
            Item.ResearchUnlockCount = 99;
        }
        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 22, 22, 19, 2f,
                Item.sellPrice(copper: 24), ItemRarityID.Purple,
                ModContent.ProjectileType<HolyFireBulletProj>(), 1f, AmmoID.Bullet);

        public override void AddRecipes()
        {
            CreateRecipe(333).
                AddIngredient(ItemID.ExplodingBullet, 333).
                AddIngredient<UnholyEssence>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
