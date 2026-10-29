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
    [LegacyName("FabsolsVodka")]
    public class PurpleHaze : BaseAlcoholItem
    {
        protected override int FoodWidth => 24;
        protected override int FoodHeight => 48;
        protected override int FoodBuffType => ModContent.BuffType<PurpleHazeBuff>();
        protected override int ItemRarity => ItemRarityID.Blue;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(239, 123, 202),
            new Color(187, 56, 158),
            new Color(165, 47, 255)
        };
        protected override int UseStyleYOffset => 8;
        protected override int UseStyleXOffset => 4;
        public static float DamageBoost = 0.25f;
        public static float StealthDamageLoss = 0.25f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(),StealthDamageLoss.ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.PurpleHaze.DripEffect").WithFormatArgs(DamageBoost.ToPercent(), StealthDamageLoss.ToPercent());

        public override AlcoholType AlcoholVariant => AlcoholType.PurpleHaze;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyPurpleHazeEffect;

        private static void ApplyPurpleHazeEffect(Player player, float intensity)
        {
            // See CalamityPlayer and CalamityPlayerMiscEffects
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.Plum).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
