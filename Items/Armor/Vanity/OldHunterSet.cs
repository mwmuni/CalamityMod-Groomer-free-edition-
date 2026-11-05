using CalamityMod.Items.Armor.DesertProwler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class OldHunterHat : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Vanity";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DesertProwlerHat>().
                AddIngredient(ItemID.BlackInk).
                AddTile(TileID.DyeVat).
                Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class OldHunterShirt : ModItem, IBulkyArmor, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Vanity";
        public string BulkTexture => "CalamityMod/Items/Armor/Vanity/OldHunterShirt_Bulk";

        public override void Load()
        {
            if (Main.dedServ)
                return;

            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/Vanity/OldHunterShirt_Back", EquipType.Back, this);
        }

        public override void SetStaticDefaults()
        {

            if (Main.dedServ)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

            ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }


        public override void EquipFrameEffects(Player player, EquipType type)
        {
            player.back = (sbyte)CalamityUtils.GetCachedEquipSlot(Mod, Name, EquipType.Back);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DesertProwlerShirt>().
                AddIngredient(ItemID.BlackInk).
                AddTile(TileID.DyeVat).
                Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class OldHunterPants : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Vanity";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DesertProwlerPants>().
                AddIngredient(ItemID.BlackInk).
                AddTile(TileID.DyeVat).
                Register();
        }
    }
}
