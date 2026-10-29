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
    public class StarBeamRye : BaseAlcoholItem
    {
        protected override int FoodWidth => 20;
        protected override int FoodHeight => 54;
        protected override int FoodBuffType => ModContent.BuffType<StarBeamRyeBuff>();
        protected override int ItemRarity => ItemRarityID.LightRed;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(89, 18, 10),
            new Color(102, 39, 14),
            new Color(128, 31, 9)
        };
        protected override int UseStyleYOffset => 7;
        protected override int UseStyleXOffset => 4;
        public static float MagicDmgMult = 0.9f;
        public static int ManaRegenBoost = 30;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MagicDmgMult);
        public override AlcoholType AlcoholVariant => AlcoholType.StarBeamRye;
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.StarBeamRye.DripEffect").WithFormatArgs(MagicDmgMult);

        public override Action<Player, float> IVDripAlcoholEffect => ApplyStarBeamRyeEffect;

        private static void ApplyStarBeamRyeEffect(Player player, float intensity)
        {
            // See CalamityPlayerMiscEffects
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.Starfruit).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
