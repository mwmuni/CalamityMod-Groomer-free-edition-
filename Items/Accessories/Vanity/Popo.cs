using CalamityMod.Items.BaseItems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Vanity
{
    public class Popo : TransformationAccessory, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override (EquipType, string, string)[] EquipSlots =>
        [
            (EquipType.Head, "Popo", null),
            (EquipType.Head, "PopoNoseless", "PopoNoseless"),
            (EquipType.Body, "Popo", null),
            (EquipType.Legs, "Popo", null),
            (EquipType.Face, null, null), //results in setting this equip slot to -1
        ];

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 44;
            Item.accessory = true;
            Item.value = Item.buyPrice(platinum: 5); // Sold by Archmage
            Item.rare = ItemRarityID.Pink;
            Item.Calamity().devItem = true;
        }

        public override bool CustomSetEquipType(Player player, EquipType type, Mod mod, string name)
        {
            if (type == EquipType.Head)
            {
                player.head = CalamityUtils.GetCachedEquipSlot(Mod, player.Calamity().snowmanNoseless ? "PopoNoseless" : "Popo", EquipType.Head);
                return true;
            }
            return false;
        }
    }
}
