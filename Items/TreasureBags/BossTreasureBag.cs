using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public abstract class BossTreasureBag : ModItem
    {
        protected virtual int BagWidth => 24;
        protected virtual int BagHeight => 24;
        protected virtual int BagRarity => ItemRarityID.Cyan;
        protected virtual bool IsPreHardmodeLike => false;
        protected virtual bool ForceIntoWorld => false;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            ItemID.Sets.BossBag[Type] = true;
            if (IsPreHardmodeLike)
                ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = BagWidth;
            Item.height = BagHeight;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = BagRarity;
            Item.expert = true;
        }

        public override bool CanRightClick() => true;

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void PostUpdate()
        {
            if (ForceIntoWorld)
                CalamityUtils.ForceItemIntoWorld(Item);

            Item.TreasureBagLightAndDust();
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return CalamityUtils.DrawTreasureBagInWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        }
    }
}
