using CalamityMod.Balancing;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace CalamityMod.Items.PermanentBoosters
{
    public class StarlightFuelCell : PermanentBooster
    {
        public override LocalizedText Tooltip => CalamityUtils.GetText($"{LocalizationCategory}.AdrenalineBoosterTooltip").WithFormatArgs(BalancingConstants.AdrenalineDamagePerBooster.ToPercent(), BalancingConstants.AdrenalineDRPerBooster.ToPercent());
        protected override int BoosterWidth => 20;
        protected override int BoosterHeight => 20;
        protected override int BoosterRarity => ItemRarityID.Lime;

        public static bool HasConsumedBefore(Player player) => player.Calamity().adrenalineBoostTwo;
        protected override bool IsConsumed(Player player) => HasConsumedBefore(player);
        protected override void MarkConsumed(Player player) => player.Calamity().adrenalineBoostTwo = true;
    }
}
