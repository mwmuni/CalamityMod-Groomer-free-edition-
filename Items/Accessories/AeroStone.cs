using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AeroStone : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int FlightTimeBoostFlat = 50;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FlightTimeBoostFlat.FramesToSeconds());

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 8));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 50;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().aeroStone = true;
            player.wingTimeMax += FlightTimeBoostFlat;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            AccessoryInventoryDrawing.DrawCustomScale(Type, spriteBatch, position, frame, drawColor, itemColor, origin, scale, 0.85f);
            return false;
        }
    }
}
