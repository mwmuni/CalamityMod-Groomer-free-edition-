using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ExoMechs.Thanatos
{
    [HasPierceResist]
    [LongDistanceNetSync(SyncWith = typeof(ThanatosHead))]
    public class ThanatosBody2 : ThanatosSegmentBase
    {
        public static int normalIconIndex;
        public static int vulnerableIconIndex;
        public static Asset<Texture2D> GlowTexture;

        protected override string NormalIconPath => "CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosNormalBody2";
        protected override string VulnerableIconPath => "CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosVulnerableBody2";
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
        protected override int SegmentWidth => 90;
        protected override int SegmentHeight => 90;
        protected override float ContactDamageDistance => 50f;
        protected override int DeathGoreCount => 3;
        protected override string GetDeathGoreName(int index) => index == 0 ? "ThanatosBody2" : $"ThanatosBody2_{index + 1}";
    }
}
