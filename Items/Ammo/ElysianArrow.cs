using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class ElysianArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HolyFlames>()];
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 22, 36, 20, 3f,
                Item.sellPrice(copper: 24), ItemRarityID.Purple,
                ModContent.ProjectileType<ElysianArrowProj>(), 10f, AmmoID.Arrow);

        public override void AddRecipes()
        {
            CreateRecipe(150).
                AddIngredient(ItemID.HolyArrow, 150).
                AddIngredient<UnholyEssence>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
