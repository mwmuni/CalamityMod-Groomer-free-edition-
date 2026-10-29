using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class YharonMask : BaseBossMask, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Vanity";
        public override void SetStaticDefaults()
        {

            if (!Main.dedServ)
                ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
        }
    }
}
