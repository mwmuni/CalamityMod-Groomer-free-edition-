using System;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class WhiteWine : BaseAlcoholItem
    {
        protected override int FoodWidth => 14;
        protected override int FoodHeight => 44;
        protected override int FoodBuffType => ModContent.BuffType<WhiteWineBuff>();
        protected override int ItemRarity => ItemRarityID.Pink;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(242, 252, 177, 180),
            new Color(250, 252, 215, 180),
            new Color(228, 245, 181, 180)
        };
        protected override int UseStyleYOffset => 5;
        protected override int UseStyleXOffset => 4;
        public static float FlightTimeRecoveryAmount = 0.66f;
        public static float FlightTimeLoss = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FlightTimeLoss.ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.WhiteWine.DripEffect").WithFormatArgs(FlightTimeLoss.ToPercent());
        public override AlcoholType AlcoholVariant => AlcoholType.WhiteWine;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyWhiteWineEffect;

        private static void ApplyWhiteWineEffect(Player player, float intensity)
        {
            // See CalamityPlayerMiscEffects
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(Item.buffType, Item.buffTime);
        }
        public override void AddRecipes()
        {
            CreateRecipe(20).
                AddIngredient(ItemID.Bottle, 20).
                AddIngredient(ItemID.GiantHarpyFeather).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
