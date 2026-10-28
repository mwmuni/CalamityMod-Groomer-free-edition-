using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    [LongDistanceNetSync(SyncWith = typeof(DesertNuisanceHead))]
    public class DesertNuisanceBody : DesertNuisanceBodyBase
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.DesertNuisanceHead.DisplayName");

        public static Asset<Texture2D> BodyTexture2;
        public static Asset<Texture2D> BodyTexture3;
        public static Asset<Texture2D> BodyTexture4;

        protected override int SegmentWidth => 88;
        protected override int SegmentHeight => 88;
        protected override int SegmentDefense => 5;
        protected override int GoodWorldDefenseBonus => 27;
        protected override int NormalLifeMax => 1500;
        protected override int RevengeanceLifeMax => 1800;
        protected override int BossRushLifeMax => 40000;
        protected override int SegmentOffset => 62;
        protected override int AlternateFinHitDistance => 40;
        protected override bool ScaleHitDistance => true;
        protected override string GoreNamePrefix => "ScourgeNuisance";

        protected override Asset<Texture2D> AlternateBodyTexture2 => BodyTexture2;
        protected override Asset<Texture2D> AlternateBodyTexture3 => BodyTexture3;
        protected override Asset<Texture2D> AlternateBodyTexture4 => BodyTexture4;

        protected override void RequestAlternateBodyTextures()
        {
            BodyTexture2 = ModContent.Request<Texture2D>(Texture + "2", AssetRequestMode.AsyncLoad);
            BodyTexture3 = ModContent.Request<Texture2D>(Texture + "3", AssetRequestMode.AsyncLoad);
            BodyTexture4 = ModContent.Request<Texture2D>(Texture + "4", AssetRequestMode.AsyncLoad);
        }

        protected override bool HasHead() => NPC.AnyNPCs(ModContent.NPCType<DesertNuisanceHead>());
    }
}
