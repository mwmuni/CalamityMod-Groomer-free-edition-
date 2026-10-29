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
    public class CaribbeanRum : BaseAlcoholItem
    {
        protected override int FoodWidth => 28;
        protected override int FoodHeight => 52;
        protected override int FoodBuffType => ModContent.BuffType<CaribbeanRumBuff>();
        protected override int ItemRarity => ItemRarityID.Pink;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(105, 29, 14),
            new Color(128, 39, 22),
            new Color(138, 28, 7)
        };
        protected override int UseStyleYOffset => 9;
        protected override int UseStyleXOffset => 5;
        public static float MoveSpeedBoost = 0.25f;
        public static float GravityMultiplier = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeedBoost.ToPercent(), (1-GravityMultiplier).ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.CaribbeanRum.DripEffect").WithFormatArgs(MoveSpeedBoost.ToPercent(), (1 - GravityMultiplier).ToPercent());

        public override AlcoholType AlcoholVariant => AlcoholType.CaribbeanRum;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyCaribbeanRumEffect;

        private static void ApplyCaribbeanRumEffect(Player player, float intensity)
        {
            // See CalamityPlayerMiscEffects
        }

        public override void AddRecipes()
        {
            CreateRecipe(20).
                AddIngredient(ItemID.Bottle, 20).
                AddIngredient(ItemID.IceFeather).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
