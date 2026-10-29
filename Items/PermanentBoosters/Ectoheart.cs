using CalamityMod.Balancing;
using CalamityMod.CalPlayer;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.PermanentBoosters
{
    public class Ectoheart : PermanentBooster
    {
        public override LocalizedText Tooltip => CalamityUtils.GetText($"{LocalizationCategory}.AdrenalineBoosterTooltip").WithFormatArgs(BalancingConstants.AdrenalineDamagePerBooster.ToPercent(), BalancingConstants.AdrenalineDRPerBooster.ToPercent());

        public int frameCounter = 0;
        public int frame = 0;
        protected override int BoosterWidth => 42;
        protected override int BoosterHeight => 44;
        protected override int BoosterRarity => ModContent.RarityType<PureGreen>();

        public static bool HasConsumedBefore(Player player) => player.Calamity().adrenalineBoostThree;
        protected override bool IsConsumed(Player player) => HasConsumedBefore(player);
        protected override void MarkConsumed(Player player) => player.Calamity().adrenalineBoostThree = true;

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/PermanentBoosters/Ectoheart_Animated").Value;
            spriteBatch.Draw(texture, position, Item.GetCurrentFrame(ref frame, ref frameCounter, 8, 3), Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/PermanentBoosters/Ectoheart_Animated").Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 8, 3), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/PermanentBoosters/EctoheartGlow").Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 8, 3, false), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }
}
