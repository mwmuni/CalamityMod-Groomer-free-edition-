using System;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class Screwdriver : BaseAlcoholItem
    {
        protected override int FoodWidth => 32;
        protected override int FoodHeight => 46;
        protected override int FoodBuffType => ModContent.BuffType<ScrewdriverBuff>();
        protected override int ItemRarity => ItemRarityID.Blue;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(225, 84, 33),
            new Color(244, 176, 77),
            new Color(255, 218, 102)
        };
        protected override int UseStyleYOffset => 10;
        protected override int UseStyleXOffset => 5;
        public static float DebuffBoost = 0.5f;
        public static float DebuffLoss = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.Screwdriver.DripEffect").WithFormatArgs(DebuffBoost.ToString("N2"), DebuffLoss.ToString("N2"));
        public override AlcoholType AlcoholVariant => AlcoholType.Screwdriver;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyScrewdriverEffect;

        private static void ApplyScrewdriverEffect(Player player, float intensity)
        {
            player.blockRange += 5;
            player.tileSpeed += 1;
            player.wallSpeed += 1;
            player.GetDamage(DamageClass.Generic) *= 0.75f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.Diamond).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
