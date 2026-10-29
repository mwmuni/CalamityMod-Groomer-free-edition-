using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class HyperiusBullet : ModItem, ILocalizedModType
    {
        public static float overflowEfficency = 0.8f; // Multiplier applied to hyperius stacks taken when enemies are damaged by other sources
        public static float overflowAppliedMult = 0.9f; // The amount of damage from a hit taken into account when dealing overflow damage
        public static readonly SoundStyle hit = new("CalamityMod/Sounds/Item/HyperiusOverflow");
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }
        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 24, 30, 12, 1.5f,
                Item.sellPrice(copper: 16), ItemRarityID.Yellow,
                ModContent.ProjectileType<HyperiusBulletProj>(), 5f, AmmoID.Bullet);

        public override void AddRecipes()
        {
            CreateRecipe(333).
                AddIngredient(ItemID.MusketBall, 333).
                AddIngredient<LifeAlloy>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
