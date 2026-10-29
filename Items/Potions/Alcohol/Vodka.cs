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
    public class Vodka : BaseAlcoholItem
    {
        protected override int FoodWidth => 14;
        protected override int FoodHeight => 36;
        protected override int FoodBuffType => ModContent.BuffType<VodkaBuff>();
        protected override int ItemRarity => ItemRarityID.LightRed;
        protected override Color[] DrinkParticleColors => new Color[2] {
            new Color(219, 219, 208, 180),
            new Color(181, 181, 176, 180)
        };
        protected override int UseStyleYOffset => 8;
        protected override int UseStyleXOffset => 2;
        public static float DebuffBoost = 0.25f;
        public static float DebuffLoss = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.Vodka.DripEffect").WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public override AlcoholType AlcoholVariant => AlcoholType.Vodka;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyVodkaEffect;

        private static void ApplyVodkaEffect(Player player, float intensity)
        {
            var cplayer = player.Calamity();
            cplayer.TypelessDebuffMultiplier += Vodka.DebuffBoost;
            cplayer.HeatDebuffMultiplier -= Vodka.DebuffLoss;
            cplayer.ColdDebuffMultiplier -= Vodka.DebuffLoss;
            cplayer.SicknessDebuffMultiplier -= Vodka.DebuffLoss;
            cplayer.WaterDebuffMultiplier -= Vodka.DebuffLoss;
            cplayer.ElectricDebuffMultiplier -= Vodka.DebuffLoss;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient<PurifiedGel>(10).
                AddIngredient<StarblightSoot>(10).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
