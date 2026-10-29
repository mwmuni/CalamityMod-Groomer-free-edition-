using System;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class GrapeBeer : BaseAlcoholItem
    {
        protected override int FoodWidth => 20;
        protected override int FoodHeight => 48;
        protected override int FoodBuffType => ModContent.BuffType<GrapeBeerBuff>();
        protected override int ItemRarity => ItemRarityID.LightRed;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(36, 2, 41),
            new Color(56, 0, 64),
            new Color(82, 10, 92)
        };
        protected override int UseStyleYOffset => 8;
        protected override int UseStyleXOffset => 4;
        public static float CloseRangeDistance = 80;
        public static float LongRangeDistance = 1200;
        public static float CloseRangeDamage = 0.75f;
        public static float LongRangeDamage = 0.25f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LongRangeDamage, LongRangeDistance / 16f); 
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.GrapeBeer.DripEffect").WithFormatArgs(LongRangeDamage, LongRangeDistance / 16f);
        public override AlcoholType AlcoholVariant => AlcoholType.GrapeBeer;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyGrapeBeerEffect;

        private static void ApplyGrapeBeerEffect(Player player, float intensity)
        {
            // See CalamityGlobalProjectile
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.Grapes).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
