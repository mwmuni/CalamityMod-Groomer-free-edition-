using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class IcicleArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Frostburn2];
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 18, 50, 6, 2.5f,
                Item.sellPrice(copper: 20), ItemRarityID.Yellow,
                ModContent.ProjectileType<IcicleArrowProj>(), 0.2f, AmmoID.Arrow);
        public override void AddRecipes()
        {
            CreateRecipe(250).
                AddIngredient(ItemID.WoodenArrow, 250).
                AddIngredient<EssenceofEleum>().
                AddIngredient(ItemID.Ectoplasm).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
