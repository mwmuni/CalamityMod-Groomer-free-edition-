using CalamityMod.Balancing;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace CalamityMod.Items.PermanentBoosters
{
    public class InfernalBlood : PermanentBooster
    {
        public override LocalizedText Tooltip => CalamityUtils.GetText($"{LocalizationCategory}.RageBoosterTooltip").WithFormatArgs(BalancingConstants.RageDurationPerBooster.FramesToSeconds());
        protected override int BoosterWidth => 20;
        protected override int BoosterHeight => 20;
        protected override int BoosterRarity => ItemRarityID.Yellow;

        public static bool HasConsumedBefore(Player player) => player.Calamity().rageBoostTwo;
        protected override bool IsConsumed(Player player) => HasConsumedBefore(player);
        protected override void MarkConsumed(Player player) => player.Calamity().rageBoostTwo = true;
    }
}
