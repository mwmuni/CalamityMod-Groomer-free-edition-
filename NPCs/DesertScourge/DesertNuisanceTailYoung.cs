using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    [LongDistanceNetSync(SyncWith = typeof(DesertNuisanceHeadYoung))]
    public class DesertNuisanceTailYoung : DesertNuisanceTailBase
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.DesertNuisanceHeadYoung.DisplayName");

        protected override int SegmentWidth => 78;
        protected override int SegmentHeight => 78;
        protected override int SegmentDefense => 7;
        protected override int GoodWorldDefenseBonus => 33;
        protected override int NormalLifeMax => 1300;
        protected override int RevengeanceLifeMax => 1560;
        protected override int BossRushLifeMax => 35000;
        protected override int SegmentOffset => 50;
        protected override string DeathGoreName => "ScourgeNuisance2Tail";

        protected override bool HasHead()
        {
            bool shouldDespawn = true;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<DesertNuisanceHeadYoung>())
                {
                    shouldDespawn = false;
                    break;
                }
            }
            return !shouldDespawn;
        }
    }
}
