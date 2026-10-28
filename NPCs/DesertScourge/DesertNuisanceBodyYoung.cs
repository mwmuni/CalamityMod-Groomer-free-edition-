using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    [LongDistanceNetSync(SyncWith = typeof(DesertNuisanceHeadYoung))]
    public class DesertNuisanceBodyYoung : DesertNuisanceBodyBase
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.DesertNuisanceHeadYoung.DisplayName");

        public static Asset<Texture2D> BodyTexture2;
        public static Asset<Texture2D> BodyTexture3;
        public static Asset<Texture2D> BodyTexture4;

        protected override int SegmentWidth => 78;
        protected override int SegmentHeight => 78;
        protected override int SegmentDefense => 4;
        protected override int GoodWorldDefenseBonus => 26;
        protected override int NormalLifeMax => 1300;
        protected override int RevengeanceLifeMax => 1560;
        protected override int BossRushLifeMax => 35000;
        protected override int SegmentOffset => 50;
        protected override int AlternateFinHitDistance => 35;
        protected override bool ScaleHitDistance => false;
        protected override string GoreNamePrefix => "ScourgeNuisance2";

        protected override Asset<Texture2D> AlternateBodyTexture2 => BodyTexture2;
        protected override Asset<Texture2D> AlternateBodyTexture3 => BodyTexture3;
        protected override Asset<Texture2D> AlternateBodyTexture4 => BodyTexture4;

        protected override void RequestAlternateBodyTextures()
        {
            BodyTexture2 = ModContent.Request<Texture2D>(Texture + "2", AssetRequestMode.AsyncLoad);
            BodyTexture3 = ModContent.Request<Texture2D>(Texture + "3", AssetRequestMode.AsyncLoad);
            BodyTexture4 = ModContent.Request<Texture2D>(Texture + "4", AssetRequestMode.AsyncLoad);
        }

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
