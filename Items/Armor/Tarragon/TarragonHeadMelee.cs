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
    [LegacyName("TarragonHelm")]
    public class TarragonHeadMelee : BaseSubtleOutlineArmor, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";

        public static float MeleeDamageBoost = 0.1f;
        public static int MeleeCritBoost = 5;
        public static float MeleeSpeedBoost = 0.15f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeDamageBoost.ToPercent(), MeleeCritBoost, MeleeSpeedBoost.ToPercent());

        // Set Bonus
        public static int SetBonusAggroBoost = 800;
        public static int TarraLifeDuration = CalamityUtils.SecondsToFrames(5);
        public static int TarraLifeRegenBoost = 3;
        public static float CloakContactDamageReduction = 0.5f;
        public static int CloakDuration = CalamityUtils.SecondsToFrames(10);
        public static int CloakCooldown = CalamityUtils.SecondsToFrames(30);

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.defense = 40; // 100
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<TarragonBreastplate>() && legs.type == ModContent.ItemType<TarragonLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraMelee = true;
            player.aggro += SetBonusAggroBoost;
            player.setBonus = this.GetLocalization("SetBonus").Format(TarraLifeRegenBoost.ToRegenPerSecond(), CalamityUtils.GetArmorSetBonusKey(), CloakDuration.FramesToSeconds(), CloakCooldown.FramesToSeconds());
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MeleeDamageClass>() += MeleeDamageBoost;
            player.GetCritChance<MeleeDamageClass>() += MeleeCritBoost;
            player.GetAttackSpeed<MeleeDamageClass>() += MeleeSpeedBoost;
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
