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
    public class Manhattan : BaseAlcoholItem
    {
        protected override int FoodWidth => 19;
        protected override int FoodHeight => 58;
        protected override int FoodBuffType => ModContent.BuffType<ManhattanBuff>();
        protected override int ItemRarity => ItemRarityID.Pink;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(225, 84, 33),
            new Color(244, 176, 77),
            new Color(255, 218, 102)
        };
        protected override int UseStyleYOffset => 14;
        protected override int UseStyleXOffset => 8;
        public static float DebuffBoost = 0.5f;
        public static float DebuffLoss = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.Manhattan.DripEffect").WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public override AlcoholType AlcoholVariant => AlcoholType.Manhattan;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyManhattanEffect;

        private static void ApplyManhattanEffect(Player player, float intensity)
        {
            var cplayer = player.Calamity();
            cplayer.ColdDebuffMultiplier += Manhattan.DebuffBoost;
            cplayer.WaterDebuffMultiplier -= Manhattan.DebuffLoss;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.FrostCore).
                AddIngredient<StarblightSoot>(10).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
