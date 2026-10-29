using System;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class Whiskey : BaseAlcoholItem
    {
        protected override int FoodWidth => 18;
        protected override int FoodHeight => 32;
        protected override int FoodBuffType => ModContent.BuffType<WhiskeyBuff>();
        protected override int ItemRarity => ItemRarityID.Blue;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(227, 148, 11),
            new Color(235, 177, 5),
            new Color(250, 190, 12)
        };
        protected override int UseStyleYOffset => 7;
        protected override int UseStyleXOffset => 2;
        public static float StandardDamageCeiling = 0.15f;
        public static float StandardDamageFloor = -0.15f;
        public static float TimeToDischarge = 600;
        public static float TimeToRecharge = 300;
        public static float CombinedDamageCeiling = 0.3f;
        public static float CombinedDamageFloor = -0.3f;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(StandardDamageFloor.ToPercent(), StandardDamageCeiling.ToPercent(), (TimeToDischarge/60).ToString("0.##"), (TimeToRecharge/60).ToString("0.##"));
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.Whiskey.DripEffect").WithFormatArgs(StandardDamageFloor.ToPercent(), StandardDamageCeiling.ToPercent(), (TimeToDischarge / 60).ToString("0.##"), (TimeToRecharge / 60).ToString("0.##"), CombinedDamageFloor.ToPercent(), CombinedDamageCeiling.ToPercent());
        public override AlcoholType AlcoholVariant => AlcoholType.Whiskey;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyWhiskeyEffect;

        private static void ApplyWhiskeyEffect(Player player, float intensity)
        {
            // See CalamityPlayer
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.SpicyPepper).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
