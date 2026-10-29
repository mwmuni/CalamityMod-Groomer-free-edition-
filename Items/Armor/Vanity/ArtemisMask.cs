using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class ArtemisMask : BaseBossMask, IExtendedHat, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Vanity";
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
                ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            }
        }

        public string ExtensionTexture => "CalamityMod/Items/Armor/Vanity/ArtemisMask_Extra";
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => new Vector2(drawInfo.drawPlayer.direction == 1f ? -6f : 0, 0f);
    }
}
