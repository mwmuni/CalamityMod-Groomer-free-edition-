using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Items.BaseItems
{
    public abstract class BaseToggleableItem : ModItem
    {
        protected virtual bool IsToggleable => true;

        protected virtual bool toggleEnabled { get; set; } = true;

        public override bool CanRightClick() => IsToggleable && Main.keyState.PressingShift();

        public override void RightClick(Player player)
        {
            if (!IsToggleable)
                return;

            toggleEnabled = !toggleEnabled;
            Item.NetStateChanged();
        }

        public override bool ConsumeItem(Player player) => !IsToggleable;

        public override void SaveData(TagCompound tag)
        {
            if (IsToggleable)
                tag.Add("toggleEffect", toggleEnabled);
        }

        public override void LoadData(TagCompound tag)
        {
            if (IsToggleable)
                toggleEnabled = tag.GetBool("toggleEffect");
        }

        public override void NetSend(BinaryWriter writer)
        {
            if (IsToggleable)
                writer.Write(toggleEnabled);
        }

        public override void NetReceive(BinaryReader reader)
        {
            if (IsToggleable)
                toggleEnabled = reader.ReadBoolean();
        }
    }
}
