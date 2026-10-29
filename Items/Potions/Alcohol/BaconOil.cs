using System;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class BaconOil : BaseAlcoholItem
    {
        protected override int FoodWidth => 20;
        protected override int FoodHeight => 32;
        protected override int FoodBuffType => ModContent.BuffType<BaconOilBuff>();
        protected override int ItemRarity => ItemRarityID.Blue;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(204, 90, 73),
            new Color(168, 37, 37),
            new Color(120, 28, 28)
        };
        protected override int UseStyleYOffset => 5;
        protected override int UseStyleXOffset => 1;
        protected override int ItemValue => Item.buyPrice(silver: 30);
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.BaconOil.DripEffect");
        public override AlcoholType AlcoholVariant => AlcoholType.BaconOil;
        public override Action<Player, float> IVDripAlcoholEffect => ApplyBaconOilEffect;

        private static void ApplyBaconOilEffect(Player player, float intensity)
        {
            // See CalamityPlayer
        }
    }
}
