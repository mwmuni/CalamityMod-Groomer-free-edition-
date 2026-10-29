using CalamityMod.Items.Materials;
using CalamityMod.Items.Armor;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Tarragon
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("TarragonMask")]
    public class TarragonHeadMagic : BaseSubtleOutlineArmor, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";

        public static int MaxManaBoost = 100;
        public static float ManaCostReduction = 0.15f;
        public static float MagicDamageBoost = 0.15f;
        public static int MagicCritBoost = 10;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxManaBoost, ManaCostReduction.ToPercent(), MagicDamageBoost.ToPercent(), MagicCritBoost);

        // Set Bonus
        public static int CritsToSpawnLeaves = 5;
        public static float LeafDamageRatio = 0.2f;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.defense = 16; // 76
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<TarragonBreastplate>() && legs.type == ModContent.ItemType<TarragonLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraMage = true;
            player.setBonus = this.GetLocalizedValue("SetBonus");
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += MaxManaBoost;
            player.manaCost -= ManaCostReduction;
            player.GetDamage<MagicDamageClass>() += MagicDamageBoost;
            player.GetCritChance<MagicDamageClass>() += MagicCritBoost;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UelibloomBar>(12).
                AddIngredient<DivineGeode>(6).
                AddTile(TileID.MythrilAnvil).
                SortBeforeFirstRecipesOf(ModContent.ItemType<TarragonBreastplate>()).
                Register();
        }
    }
}
