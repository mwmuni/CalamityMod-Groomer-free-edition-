using CalamityMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public abstract class BaseAlcoholItem : ModItem, ILocalizedModType, IAlcoholItem
    {
        public new string LocalizationCategory => "Items.Potions";

        public abstract AlcoholType AlcoholVariant { get; }
        public abstract LocalizedText DripEffectText { get; }
        public abstract System.Action<Player, float> IVDripAlcoholEffect { get; }

        protected abstract int FoodWidth { get; }
        protected abstract int FoodHeight { get; }
        protected abstract int FoodBuffType { get; }
        protected abstract int ItemRarity { get; }
        protected abstract Color[] DrinkParticleColors { get; }
        protected abstract int UseStyleYOffset { get; }
        protected virtual int UseStyleXOffset => 0;
        protected virtual int ItemValue => Item.sellPrice(silver: 2);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Type] = DrinkParticleColors;
            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(FoodWidth, FoodHeight, FoodBuffType, CalamityUtils.MinutesToFrames(6), true);
            Item.value = ItemValue;
            Item.rare = ItemRarity;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += UseStyleXOffset * player.direction;
            player.itemLocation.Y -= UseStyleYOffset;
        }
    }
}
