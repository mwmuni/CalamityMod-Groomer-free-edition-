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
    [LegacyName("TarragonHornedHelm")]
    public class TarragonHeadSummon : BaseSubtleOutlineArmor, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";
        internal static string LifeAuraEntitySourceContext => "SetBonus_Calamity_Tarragon";

        public static int MinionSlotBoost = 1;
        public static float SummonDamageBoost = 0.25f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MinionSlotBoost, SummonDamageBoost.ToPercent());

        // Set Bonus
        public static int SetBonusMinionSlotBoost = 2;
        public static float SetBonusSummonDamageBoost = 0.3f;
        public static int AuraDamage = 120;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.defense = 10; // 70
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<TarragonBreastplate>() && legs.type == ModContent.ItemType<TarragonLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraSummon = true;
            modPlayer.WearingPostMLSummonerSet = true;
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusMinionSlotBoost, SetBonusSummonDamageBoost.ToPercent());
            player.maxMinions += SetBonusMinionSlotBoost;
            player.GetDamage<SummonDamageClass>() += SetBonusSummonDamageBoost;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += MinionSlotBoost;
            player.GetDamage<SummonDamageClass>() += SummonDamageBoost;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UelibloomBar>(12).
                AddIngredient<DivineGeode>(6).
                AddTile(TileID.MythrilAnvil).
                SortBeforeFirstRecipesOf(ModContent.ItemType<TarragonHeadRogue>()).
                Register();
        }
    }
}
