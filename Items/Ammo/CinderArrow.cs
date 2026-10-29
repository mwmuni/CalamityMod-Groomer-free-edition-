using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    [LegacyName("NapalmArrow")]
    public class CinderArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BrimstoneFlames>()];
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 22, 36, 17, 1.5f,
                Item.sellPrice(copper: 12), ItemRarityID.Pink,
                ModContent.ProjectileType<CinderArrowProj>(), 13f, AmmoID.Arrow);

        public override void AddRecipes()
        {
            CreateRecipe(250).
                AddIngredient(ItemID.HellfireArrow, 250).
                AddIngredient<UnholyCore>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
