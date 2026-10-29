using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("CalamitasMask")]
    public class CalamitasCloneMask : BaseBossMask, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Vanity";
        public override void SetStaticDefaults()
        {

            if (!Main.dedServ)
                ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
        }
    }
}
