using System.Collections.Generic;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class HallowPointRound : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";

        public static int BaseDamage = 12;
        public static int BonusDamageOnHit = 6; // 12 + 6 = 18

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 8, 18, BaseDamage, 3f,
                Item.sellPrice(copper: 12), ItemRarityID.Pink,
                ModContent.ProjectileType<HallowPointRoundProj>(), 6f, AmmoID.Bullet);

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            // Add the proper damage value
            int nonConvertDamage = (int)Main.LocalPlayer.GetTotalDamage<RangedDamageClass>().ApplyTo(BaseDamage + BonusDamageOnHit);
            list.FindAndReplace("[DAMAGE]", nonConvertDamage.ToString());
        }

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient(ItemID.EmptyBullet, 100).
                AddIngredient(ItemID.HallowedBar).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
