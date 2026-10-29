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
    public class EvergreenGin : BaseAlcoholItem
    {
        protected override int FoodWidth => 34;
        protected override int FoodHeight => 56;
        protected override int FoodBuffType => ModContent.BuffType<EvergreenGinBuff>();
        protected override int ItemRarity => ItemRarityID.LightRed;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(201, 248, 248),
            new Color(201, 248, 221),
            new Color(177, 240, 184)
        };
        protected override int UseStyleYOffset => 9;
        protected override int UseStyleXOffset => 5;
        public static float DebuffBoost = 0.5f;
        public static float DebuffLoss = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.EvergreenGin.DripEffect").WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public override AlcoholType AlcoholVariant => AlcoholType.EvergreenGin;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyEvergreenGinEffect;
        private static void ApplyEvergreenGinEffect(Player player, float intensity)
        {
            var cplayer = player.Calamity();
            cplayer.SicknessDebuffMultiplier += EvergreenGin.DebuffBoost;
            cplayer.WaterDebuffMultiplier += EvergreenGin.DebuffBoost;
            cplayer.ElectricDebuffMultiplier -= EvergreenGin.DebuffLoss;
            cplayer.HeatDebuffMultiplier -= EvergreenGin.DebuffLoss;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.Vine, 3).
                AddIngredient<StarblightSoot>(10).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
