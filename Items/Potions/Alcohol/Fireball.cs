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
    public class Fireball : BaseAlcoholItem
    {
        protected override int FoodWidth => 20;
        protected override int FoodHeight => 52;
        protected override int FoodBuffType => ModContent.BuffType<FireballBuff>();
        protected override int ItemRarity => ItemRarityID.LightRed;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(245, 171, 22),
            new Color(227, 128, 41),
            new Color(237, 82, 31)
        };
        protected override int UseStyleYOffset => 8;
        public static float DebuffBoost = 0.5f;
        public static float DebuffLoss = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.Fireball.DripEffect").WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());

        public override AlcoholType AlcoholVariant => AlcoholType.Fireball;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyFireballEffect;

        private static void ApplyFireballEffect(Player player, float intensity)
        {
            var cplayer = player.Calamity();
            cplayer.HeatDebuffMultiplier += Fireball.DebuffBoost;
            cplayer.SicknessDebuffMultiplier -= Fireball.DebuffLoss;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.LivingFireBlock, 50).
                AddIngredient<StarblightSoot>(10).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
