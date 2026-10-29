using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Food
{
    public abstract class AmbrosiaFruit : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Potions";

        protected abstract Color[] FoodParticleColors { get; }
        protected abstract int FoodWidth { get; }
        protected abstract int FoodHeight { get; }
        protected abstract int FoodBuff { get; }
        protected abstract int FoodRarity { get; }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Type] = FoodParticleColors;
            ItemID.Sets.IsFood[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Ambrosia;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(FoodWidth, FoodHeight, FoodBuff, CalamityUtils.MinutesToFrames(5));
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = FoodRarity;
        }
    }
}
