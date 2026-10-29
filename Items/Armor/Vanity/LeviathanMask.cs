using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class LeviathanMask : BaseBossMask, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Vanity";
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }
    }
}
