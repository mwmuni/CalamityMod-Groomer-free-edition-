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
    public class TequilaSunrise : BaseAlcoholItem
    {
        protected override int FoodWidth => 22;
        protected override int FoodHeight => 28;
        protected override int FoodBuffType => ModContent.BuffType<TequilaSunriseBuff>();
        protected override int ItemRarity => ItemRarityID.LightRed;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(220, 75, 59),
            new Color(229, 147, 47),
            new Color(255, 190, 76)
        };
        protected override int UseStyleYOffset => 10;
        protected override int UseStyleXOffset => 9;
        public static float DoTMultiplier = 1.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DoTMultiplier-1).ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.TequilaSunrise.DripEffect").WithFormatArgs((DoTMultiplier-1).ToPercent());
        public override AlcoholType AlcoholVariant => AlcoholType.TequilaSunrise;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyTequilaSunriseEffect;

        private static void ApplyTequilaSunriseEffect(Player player, float intensity)
        {
            // See CalamityPlayerLifeRegen
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.LightShard).
                AddIngredient<StarblightSoot>(10).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
