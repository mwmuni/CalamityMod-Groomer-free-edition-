using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Sounds;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    [LegacyName("AngryChickenStaff")]
    public class YharonsKindleStaff : SummonStaff, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public const float ReboundRamDamageFactor = 2f;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 5f;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Dragonfire>()];
        }

        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 74;
            Item.damage = 325;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 24;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.UseSound = CommonCalamitySounds.FlareSound;
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<FieryDraconidBuff>();
            Item.shoot = ModContent.ProjectileType<FieryDraconid>();
        }

    }
}
