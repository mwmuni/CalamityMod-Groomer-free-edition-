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
    [LegacyName("TarragonHelmet")]
    public class TarragonHeadRogue : BaseSubtleOutlineArmor, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";

        public static float RogueDamageBoost = 0.1f;
        public static int RogueCritBoost = 7;
        public static float MoveSpeedBoost = 0.05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RogueDamageBoost.ToPercent(), RogueCritBoost, MoveSpeedBoost.ToPercent());

        // Set Bonus
        public static float SetBonusRogueStealth = 1.15f;
        public static int CritsToActivateImmunity = 50;
        public static int ImmunityDuration = CalamityUtils.SecondsToFrames(2.5f);
        public static int ImmunityCooldown = CalamityUtils.SecondsToFrames(25);
        public static float RogueDamageBoostWhileDebuffed = 0.1f;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.defense = 24; // 84
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<TarragonBreastplate>() && legs.type == ModContent.ItemType<TarragonLeggings>();

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraThrowing = true;
            modPlayer.rogueStealthMax += SetBonusRogueStealth;
            modPlayer.wearingRogueArmor = true;
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusRogueStealth.ToStealth(), CritsToActivateImmunity, ImmunityDuration.FramesToSeconds(), ImmunityCooldown.FramesToSeconds());
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ThrowingDamageClass>() += RogueDamageBoost;
            player.GetCritChance<ThrowingDamageClass>() += RogueCritBoost;
            player.moveSpeed += MoveSpeedBoost;
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
