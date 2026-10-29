using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ExoMechs.Thanatos
{
    [HasPierceResist]
    [LongDistanceNetSync(SyncWith = typeof(ThanatosHead))]
    public class ThanatosTail : ThanatosSegmentBase
    {
        public static int normalIconIndex;
        public static int vulnerableIconIndex;
        public static Asset<Texture2D> GlowTexture;

        protected override string NormalIconPath => "CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosNormalTail";
        protected override string VulnerableIconPath => "CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosVulnerableTail";
        protected override int NormalIconIndex
        {
            get => normalIconIndex;
            set => normalIconIndex = value;
        }
        protected override int VulnerableIconIndex
        {
            get => vulnerableIconIndex;
            set => vulnerableIconIndex = value;
        }
        protected override Asset<Texture2D> GlowAsset
        {
            get => GlowTexture;
            set => GlowTexture = value;
        }
        protected override int SegmentDamage => 120;
        protected override int SegmentWidth => 76;
        protected override int SegmentHeight => 110;
        protected override float ContactDamageDistance => 45f;
        protected override int DeathGoreCount => 4;
        protected override string GetDeathGoreName(int index) => index == 0 ? "ThanatosTail" : $"ThanatosTail{index + 1}";
    }
}
