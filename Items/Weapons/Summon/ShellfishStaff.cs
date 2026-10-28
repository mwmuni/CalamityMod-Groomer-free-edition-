using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class ShellfishStaff : SummonStaff, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public override void SetStaticDefaults() => ItemID.Sets.StaffMinionSlotsRequired[Type] = 2f;

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.damage = 70;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<ShellfishBuff>();
            Item.shoot = ModContent.ProjectileType<Shellfish>();
            Item.DamageType = DamageClass.Summon;
        }

    }
}
