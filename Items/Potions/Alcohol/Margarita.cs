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
    public class Margarita : BaseAlcoholItem
    {
        protected override int FoodWidth => 42;
        protected override int FoodHeight => 52;
        protected override int FoodBuffType => ModContent.BuffType<MargaritaBuff>();
        protected override int ItemRarity => ItemRarityID.LightRed;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(219, 227, 191),
            new Color(186, 189, 147),
            new Color(142, 161, 125)
        };
        protected override int UseStyleYOffset => 14;
        protected override int UseStyleXOffset => 9;
        public static int BuffType = ModContent.BuffType<MargaritaBuff>();
        public static float DebuffLoss = 0.5f;
        public static int MinuteDuration = 3; 
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DebuffLoss.ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.Margarita.DripEffect").WithFormatArgs(DebuffLoss.ToPercent());
        public override AlcoholType AlcoholVariant => AlcoholType.Margarita;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyMargaritaEffect;

        private static void ApplyMargaritaEffect(Player player, float intensity)
        {
            var cplayer = player.Calamity();
            cplayer.HeatDebuffMultiplier -= Margarita.DebuffLoss;
            cplayer.SicknessDebuffMultiplier -= Margarita.DebuffLoss;
            cplayer.ColdDebuffMultiplier -= Margarita.DebuffLoss;
            cplayer.WaterDebuffMultiplier -= Margarita.DebuffLoss;
            cplayer.ElectricDebuffMultiplier -= Margarita.DebuffLoss;
            cplayer.TypelessDebuffMultiplier -= Margarita.DebuffLoss;
            // Also see CalamityPlayerMiscEffects
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.DarkShard).
                AddIngredient<StarblightSoot>(10).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
