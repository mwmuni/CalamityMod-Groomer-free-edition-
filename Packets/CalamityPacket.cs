using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Packets
{
    internal abstract class CalamityPacket : ILoadable
    {
        public abstract void HandlePacket(BinaryReader packet, int sender);

        private ushort _NetID;
        private PropertyInfo _Prop_Static_Instance;

        public void Load(Mod mod)
        {
            _NetID = CalamityNetcode.RegisterHandler(this);

            var type = GetType();
            var instanceProperty = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);

            if (instanceProperty == null)
                return;

            if (!instanceProperty.PropertyType.IsAssignableFrom(type))
                CalamityMod.Log.Error($"Packet instance's 'Instance' property is not asssignable with given type! [Failed On: '{type.FullName}']");

            instanceProperty.SetValue(null, this);
            _Prop_Static_Instance = instanceProperty; // We saving this for Unload Steps
        }

        public virtual void Unload()
        {
            _Prop_Static_Instance?.SetValue(null, null);
            _Prop_Static_Instance = null;
        }

        public void CloneAndBroadcast(BinaryReader packet, long startIndex, int length, int ignoreClient = -1)
        {
            if (Main.netMode != NetmodeID.Server)
                return;

            var stream = packet.BaseStream;
            if (startIndex < 0 || startIndex > stream.Length || length < 0 || length > stream.Length - startIndex)
                throw new InvalidDataException("Invalid Calamity packet relay range.");

            long originalPosition = stream.Position;
            try
            {
                stream.Position = startIndex;
                Span<byte> buffer = length <= 256 ? stackalloc byte[length] : new byte[length];
                stream.ReadExactly(buffer);

                var newPacket = CreateBasePacket();
                newPacket.Write(buffer);
                newPacket.Send(ignoreClient: ignoreClient);
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        public ModPacket CreateBasePacket()
        {
            var packet = CalamityMod.Instance.GetPacket();
            CalamityNetcode.WriteHandlerNetID(packet, _NetID);
            return packet;
        }
    }
}
