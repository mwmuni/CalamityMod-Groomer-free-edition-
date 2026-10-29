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
    public class Everclear : BaseAlcoholItem
    {
        protected override int FoodWidth => 16;
        protected override int FoodHeight => 52;
        protected override int FoodBuffType => ModContent.BuffType<EverclearBuff>();
        protected override int ItemRarity => ItemRarityID.LightRed;
        protected override Color[] DrinkParticleColors => new Color[2] {
            new Color(153, 168, 162, 180),
            new Color(198, 205, 207, 180)
        };
        protected override int UseStyleYOffset => 7;
        protected override int UseStyleXOffset => 3;
        public static float DamageBoost = 0.25f;
        public static int RegenLoss = 10;
        public static float DefenseLossPercent = 0.30f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(), RegenLoss.ToRegenPerSecond(), DefenseLossPercent.ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.Everclear.DripEffect").WithFormatArgs(DamageBoost.ToPercent(), RegenLoss.ToRegenPerSecond(), DefenseLossPercent.ToPercent());
        public override AlcoholType AlcoholVariant => AlcoholType.Everclear;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyEverclearEffect;

        private static void ApplyEverclearEffect(Player player, float intensity)
        {
            // See CalamityPlayerLifeRegen and CalamityPlayerMiscEffects
        }

        public override void AddRecipes()
        {
            CreateRecipe(20).
                AddIngredient(ItemID.Bottle, 20).
                AddIngredient(ItemID.BlackLens).
                AddIngredient(ItemID.SoulofNight, 10).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
