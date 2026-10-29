using System;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions.Food;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class RedWine : BaseAlcoholItem
    {
        protected override int FoodWidth => 20;
        protected override int FoodHeight => 61;
        protected override int FoodBuffType => ModContent.BuffType<RedWineBuff>();
        protected override int ItemRarity => ItemRarityID.Pink;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(54, 5, 21),
            new Color(82, 9, 36),
            new Color(105, 4, 29)
        };
        protected override int UseStyleYOffset => 5;
        protected override int UseStyleXOffset => 1;
        public static float VerticalSpeedBoost = 0.1f;
        public static float FlightTimeLoss = 0.25f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(VerticalSpeedBoost.ToPercent(), FlightTimeLoss.ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.RedWine.DripEffect").WithFormatArgs(VerticalSpeedBoost.ToPercent(), FlightTimeLoss.ToPercent());

        public override AlcoholType AlcoholVariant => AlcoholType.RedWine;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyRedWineEffect;

        private static void ApplyRedWineEffect(Player player, float intensity)
        {
            // See CalamityPlayer and CalamityPlayerMiscEffects
        }

        public override void AddRecipes()
        {
            CreateRecipe(20).
                AddIngredient(ItemID.Bottle, 20).
                AddIngredient(ItemID.FireFeather).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
