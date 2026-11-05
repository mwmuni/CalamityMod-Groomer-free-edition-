using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Body)]
    public class TheGrandGarment : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Vanity";
        public override void Load()
        {
            if (!Main.dedServ)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Vanity/TheGrandGarment_Waist", EquipType.Waist, this);
            }
        }


        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
            Item.Calamity().donorItem = true;
        }

        public override void EquipFrameEffects(Player player, EquipType type)
        {
            player.waist = (sbyte)CalamityUtils.GetCachedEquipSlot(Mod, Name, EquipType.Waist);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 5).
                AddIngredient(ItemID.Leather, 2).
                AddIngredient(ItemID.BlueDye, 1).
                AddTile(TileID.Loom).
                Register();
        }
    }
}
