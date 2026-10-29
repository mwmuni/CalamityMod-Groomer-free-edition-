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
    public class Moonshine : BaseAlcoholItem
    {
        protected override int FoodWidth => 30;
        protected override int FoodHeight => 48;
        protected override int FoodBuffType => ModContent.BuffType<MoonshineBuff>();
        protected override int ItemRarity => ItemRarityID.Lime;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(237, 237, 218, 128),
            new Color(227, 219, 191, 128),
            new Color(209, 204, 194, 128)
        };
        protected override int UseStyleYOffset => 8;
        protected override int UseStyleXOffset => 4;
        public static float MaxLifePercentBoost = 0.25f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxLifePercentBoost.ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.Moonshine.DripEffect").WithFormatArgs(MaxLifePercentBoost.ToPercent());
        public override AlcoholType AlcoholVariant => AlcoholType.Moonshine;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyMoonshineEffect;

        private static void ApplyMoonshineEffect(Player player, float intensity)
        {
            // See CalamityPlayer and CalamityPlayerHitHurt
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.LifeFruit).
                AddIngredient<AncientBoneDust>(5).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
