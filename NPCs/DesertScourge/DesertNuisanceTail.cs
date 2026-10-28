using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    [LongDistanceNetSync(SyncWith = typeof(DesertNuisanceHead))]
    public class DesertNuisanceTail : DesertNuisanceTailBase
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.DesertNuisanceHead.DisplayName");

        protected override int SegmentWidth => 88;
        protected override int SegmentHeight => 88;
        protected override int SegmentDefense => 8;
        protected override int GoodWorldDefenseBonus => 34;
        protected override int NormalLifeMax => 1500;
        protected override int RevengeanceLifeMax => 1800;
        protected override int BossRushLifeMax => 40000;
        protected override int SegmentOffset => 62;
        protected override string DeathGoreName => "ScourgeNuisanceTail";

        protected override bool HasHead() => NPC.AnyNPCs(ModContent.NPCType<DesertNuisanceHead>());
    }
}
