using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public abstract class LoreItem : ModItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Lore";

        // When holding SHIFT on a lore item, the default tooltip is removed entirely.
        public bool HidesNormalTooltip => true;

        // Lore items have a flavorful extension indicator tooltip.
        public string ExtensionIndicatorKey => $"{LocalizationCategory}.ShortTooltip";

        // Each line of the extension indicator tooltip is manually colored, so don't provide an override color.
        public Color? ExtensionIndicatorColor => null;

        // The localization key for all lore items' full lore content is just "Lore".
        public string TooltipExtensionKey => "Lore";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player) => false;

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.LoreItems;
        }
    }
}
