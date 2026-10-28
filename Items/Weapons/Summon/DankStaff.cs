using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DankStaff : SummonStaff, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BrainRot>()];
        }
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 58;
            Item.damage = 10;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<DankCreeperBuff>();
            Item.shoot = ModContent.ProjectileType<DankCreeperMinion>();
            Item.DamageType = DamageClass.Summon;
        }

    }
}
