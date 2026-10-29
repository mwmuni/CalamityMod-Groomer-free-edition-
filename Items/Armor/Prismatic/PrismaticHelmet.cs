using System;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Prismatic
{
    [AutoloadEquip(EquipType.Head)]
    public class PrismaticHelmet : BaseSubtleOutlineArmor, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";
        internal static string LaserEntitySourceContext => "SetBonus_Calamity_Prismatic";

        public static int MaxManaBoost = 80;
        public static float ManaCostReduction = 0.15f;
        public static float MagicDamageBoost = 0.15f;
        public static int MagicCritBoost = 12;
        public static float NonMagicDamageDecrease = 0.2f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxManaBoost, ManaCostReduction.ToPercent(), MagicDamageBoost.ToPercent(), MagicCritBoost, NonMagicDamageDecrease.ToPercent());

        // Set Bonus
        public static int ManaRegenBonus = 8;
        public static int LaserDamage = 30;
        public static int LaserDuration = CalamityUtils.SecondsToFrames(5);
        public static int LaserCooldown = CalamityUtils.SecondsToFrames(30);

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 18; // 72

            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.Calamity().donorItem = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<PrismaticRegalia>() && legs.type == ModContent.ItemType<PrismaticGreaves>();

        public override void UpdateArmorSet(Player player)
        {
            player.Calamity().prismaticSet = true;
            player.manaRegenBonus += ManaRegenBonus;
            Color AbilityBriefColor = Color.Lerp(new Color(255, 106, 246), new Color(148, 145, 243), 0.5f + 0.5f * MathF.Sin(Main.GlobalTimeWrappedHourly * 3f));
            player.setBonus = this.GetLocalization("SetBonus").Format(AbilityBriefColor.Hex3(), CalamityUtils.GetArmorSetBonusKey(), LaserDuration.FramesToSeconds(), LaserCooldown.FramesToSeconds());
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().prismaticHelmet = true;
            player.statManaMax2 += MaxManaBoost;
            player.manaCost -= ManaCostReduction;
            player.GetDamage<MagicDamageClass>() += MagicDamageBoost;
            player.GetCritChance<MagicDamageClass>() += MagicCritBoost;
            player.GetDamage<GenericDamageClass>() -= NonMagicDamageDecrease;
            player.GetDamage<MagicDamageClass>() += NonMagicDamageDecrease;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArmoredShell>(3).
                AddIngredient<ExodiumCluster>(5).
                AddIngredient<DivineGeode>(4).
                AddIngredient(ItemID.Nanites, 300).
                AddTile(TileID.MythrilAnvil).
                SortBeforeFirstRecipesOf(ModContent.ItemType<PrismaticGreaves>()).
                Register();
        }
    }
}
