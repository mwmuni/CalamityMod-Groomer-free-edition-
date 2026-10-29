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
    public class OldFashioned : BaseAlcoholItem
    {
        protected override int FoodWidth => 38;
        protected override int FoodHeight => 44;
        protected override int FoodBuffType => ModContent.BuffType<OldFashionedBuff>();
        protected override int ItemRarity => ItemRarityID.Blue;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(255, 118, 3),
            new Color(255, 200, 82),
            new Color(255, 228, 122)
        };
        protected override int UseStyleYOffset => 12;
        protected override int UseStyleXOffset => 3;
        public static readonly float DamageBoostMultiplier = 1.25f;
        public static readonly float DamageReductionMultiplier = 0.75f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DamageBoostMultiplier).ToString("N2"), (DamageReductionMultiplier).ToString("N2"));
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.OldFashioned.DripEffect").WithFormatArgs(DamageBoostMultiplier.ToString("N2"), DamageReductionMultiplier.ToString("N2"));
        public override AlcoholType AlcoholVariant => AlcoholType.OldFashioned;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyOldFashionedEffect;

        private static void ApplyOldFashionedEffect(Player player, float intensity)
        {
            // See CalamityGlobalProjectile
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.Grapefruit).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
