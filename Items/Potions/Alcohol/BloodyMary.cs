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
    public class BloodyMary : BaseAlcoholItem
    {
        protected override int FoodWidth => 34;
        protected override int FoodHeight => 56;
        protected override int FoodBuffType => ModContent.BuffType<BloodyMaryBuff>();
        protected override int ItemRarity => ItemRarityID.Blue;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(204, 90, 73),
            new Color(168, 37, 37),
            new Color(120, 28, 28)
        };
        protected override int UseStyleYOffset => 11;
        protected override int UseStyleXOffset => 9;
        public static float PierceDamageMultiplier = 0.66f;
        public static float SpawnRateGateMultiplier = 0.142f;
        public static float IVDripAdditionalSpawnRateGateMultiplier = 0.7f;
        public static float SpawnLimitMultiplier = 5f;
        public static float IVDripAdditionalSpawnLimitMultiplier = 1.5f;        
        public override LocalizedText Tooltip => base.Tooltip;
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.BloodyMary.DripEffect");
        public override AlcoholType AlcoholVariant => AlcoholType.BloodyMary;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyBloodyMaryEffect;

        private static void ApplyBloodyMaryEffect(Player player, float intensity)
        {
            // See CalamityPlayerOnHit
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.BloodMoonStarter).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
