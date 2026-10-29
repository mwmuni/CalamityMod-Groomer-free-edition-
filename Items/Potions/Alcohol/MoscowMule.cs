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
    public class MoscowMule : BaseAlcoholItem
    {
        protected override int FoodWidth => 46;
        protected override int FoodHeight => 46;
        protected override int FoodBuffType => ModContent.BuffType<MoscowMuleBuff>();
        protected override int ItemRarity => ItemRarityID.LightRed;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(194, 252, 192),
            new Color(226, 252, 192),
            new Color(250, 225, 222)
        };
        protected override int UseStyleYOffset => 12;
        protected override int UseStyleXOffset => 8;
        public static float KnockbackBoost = 0.5f;
        public static float PierceDamageMultiplier = 0.7f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(KnockbackBoost.ToPercent());
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.MoscowMule.DripEffect").WithFormatArgs(KnockbackBoost.ToPercent());
        public override AlcoholType AlcoholVariant => AlcoholType.MoscowMule;

        public override Action<Player, float> IVDripAlcoholEffect => ApplyMoscowMuleEffect;

        private static void ApplyMoscowMuleEffect(Player player, float intensity)
        {
            // See CalamityPlayer and CalamityPlayerOnHit
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient<TitanHeart>().
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
