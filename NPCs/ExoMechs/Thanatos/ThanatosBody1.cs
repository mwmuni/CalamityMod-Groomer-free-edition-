using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ExoMechs.Thanatos
{
    [HasPierceResist]
    [LongDistanceNetSync(SyncWith = typeof(ThanatosHead))]
    public class ThanatosBody1 : ThanatosSegmentBase
    {
        public static int normalIconIndex;
        public static int vulnerableIconIndex;
        public static Asset<Texture2D> GlowTexture;

        protected override string NormalIconPath => "CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosNormalBody";
        protected override string VulnerableIconPath => "CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosVulnerableBody";
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
        protected override int SegmentDamage => 150;
        protected override int SegmentWidth => 88;
        protected override int SegmentHeight => 88;
        protected override float ContactDamageDistance => 50f;
        protected override int DeathGoreCount => 3;
        protected override string GetDeathGoreName(int index) => index == 0 ? "ThanatosBody1" : $"ThanatosBody1_{index + 1}";
    }
}
