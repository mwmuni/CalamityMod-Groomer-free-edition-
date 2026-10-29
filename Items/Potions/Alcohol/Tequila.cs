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
    public class Tequila : BaseAlcoholItem
    {
        protected override int FoodWidth => 32;
        protected override int FoodHeight => 44;
        protected override int FoodBuffType => ModContent.BuffType<TequilaBuff>();
        protected override int ItemRarity => ItemRarityID.LightRed;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(163, 110, 10),
            new Color(176, 135, 0),
            new Color(194, 132, 25)
        };
        protected override int UseStyleYOffset => 9;
        protected override int UseStyleXOffset => 4;
        public static float DebuffBoost = 0.5f;
        public static float DebuffLoss = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.Tequila.DripEffect").WithFormatArgs((DebuffBoost).ToPercent(), DebuffLoss.ToPercent());
        public override AlcoholType AlcoholVariant => AlcoholType.Tequila;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyTequilaEffect;

        private static void ApplyTequilaEffect(Player player, float intensity)
        {
            var cplayer = player.Calamity();
            cplayer.ElectricDebuffMultiplier += Tequila.DebuffBoost;
            cplayer.ColdDebuffMultiplier -= Tequila.DebuffLoss;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient<StormlionMandible>(3).
                AddIngredient<StarblightSoot>(10).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
