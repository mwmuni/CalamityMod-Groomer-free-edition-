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
    [LegacyName("TarragonVisage")]
    public class TarragonHeadRanged : BaseSubtleOutlineArmor, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";

        public static float RangedDamageBoost = 0.1f;
        public static int RangedCritBoost = 7;
        public static float AmmoReduction = 0.75f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RangedDamageBoost.ToPercent(), RangedCritBoost, (1f - AmmoReduction).ToPercent());

        // Set Bonus
        public static int OnHitEffectCooldown = CalamityUtils.SecondsToFrames(1);
        public static float LeafDamageRatio = 0.25f;
        public static int LeafDamageSoftcap = 150;
        public static float EnergyDamageRatio = 0.33f;
        public static int EnergyDamageSoftcap = 200;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.defense = 28; // 88
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<TarragonBreastplate>() && legs.type == ModContent.ItemType<TarragonLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraRanged = true;
            player.setBonus = this.GetLocalizedValue("SetBonus");
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.ammoCost *= AmmoReduction;
            player.GetDamage<RangedDamageClass>() += RangedDamageBoost;
            player.GetCritChance<RangedDamageClass>() += RangedCritBoost;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UelibloomBar>(12).
                AddIngredient<DivineGeode>(6).
                AddTile(TileID.MythrilAnvil).
                SortBeforeFirstRecipesOf(ModContent.ItemType<TarragonHeadMagic>()).
                Register();
        }
    }
}
