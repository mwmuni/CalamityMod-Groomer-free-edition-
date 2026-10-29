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
    public class CinnamonRoll : BaseAlcoholItem
    {
        protected override int FoodWidth => 28;
        protected override int FoodHeight => 48;
        protected override int FoodBuffType => ModContent.BuffType<CinnamonRollBuff>();
        protected override int ItemRarity => ItemRarityID.Lime;
        protected override Color[] DrinkParticleColors => new Color[3] {
            new Color(245, 223, 181),
            new Color(222, 186, 147),
            new Color(176, 129, 106)
        };
        protected override int UseStyleYOffset => 9;
        protected override int UseStyleXOffset => 5;
        public override AlcoholType AlcoholVariant => AlcoholType.CinnamonRoll;

        public override LocalizedText Tooltip => base.Tooltip;
        public override LocalizedText DripEffectText => Language.GetText("Mods.CalamityMod.Items.Potions.CinnamonRoll.DripEffect");
        public override Action<Player, float> IVDripAlcoholEffect => ApplyCinnamonRollEffect;

        private static void ApplyCinnamonRollEffect(Player player, float intensity)
        {
            // See CalamityPlayerMiscEffects and CalamityPlayerOnHit
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.TurtleShell).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
