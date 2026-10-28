using System.IO;
using Terraria;

namespace CalamityMod.TileEntities
{
    public abstract class TEIndexedTargetingTurret : TEBaseTurret
    {
        private const int BytesUsed = 2;
        private static readonly byte[] JunkData = new byte[NumExtraBytes - BytesUsed];
        private int _targetIndex = -1;

        protected int TargetIndex
        {
            get => _targetIndex;
            set
            {
                bool sync = _targetIndex != value;
                _targetIndex = value;
                if (sync)
                    SendSyncPacket();
            }
        }

        public override void WriteExtraTurretData(BinaryWriter writer)
        {
            writer.Write((short)_targetIndex);
            writer.Write(JunkData);
        }

        public override void ReadExtraTurretData(BinaryReader reader)
        {
            _targetIndex = reader.ReadInt16();
            _ = reader.ReadBytes(NumExtraBytes - BytesUsed);
        }
    }
}
