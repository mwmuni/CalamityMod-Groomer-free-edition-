using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class PuffShroom : SummonStaff, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 14;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item42;
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<PuffWarriorBuff>();
            Item.shoot = ModContent.ProjectileType<PuffWarrior>();
            Item.DamageType = DamageClass.Summon;
        }

    }
}
