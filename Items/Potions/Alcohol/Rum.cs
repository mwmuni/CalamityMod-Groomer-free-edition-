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
    public class Rum : BaseAlcoholItem
    {
        protected override int FoodWidth => 30;
        protected override int FoodHeight => 44;
        protected override int FoodBuffType => ModContent.BuffType<RumBuff>();
        protected override int ItemRarity => ItemRarityID.LightRed;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(237, 165, 9),
            new Color(247, 219, 54),
            new Color(255, 195, 31)
        };
        protected override int UseStyleYOffset => 9;
        protected override int UseStyleXOffset => 3;
        public static float NonMinionBoost = 1.15f;
        public static float MinionBoost = 0.85f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((NonMinionBoost-1).ToPercent(), (1- MinionBoost).ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.Rum.DripEffect").WithFormatArgs((NonMinionBoost - 1).ToPercent(), (1 - MinionBoost).ToPercent());
        public override AlcoholType AlcoholVariant => AlcoholType.Rum;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyRumEffect;

        private static void ApplyRumEffect(Player player, float intensity)
        {
            // See CalamityGlobalProjectile
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.SpiderFang, 3).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
