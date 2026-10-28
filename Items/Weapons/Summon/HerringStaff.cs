using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class HerringStaff : SummonStaff, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetDefaults()
        {
            Item.width = Item.height = 48;
            Item.damage = 15;
            Item.knockBack = 1.25f;
            Item.mana = 10;

            Item.buffType = ModContent.BuffType<Herring>();
            Item.shoot = ModContent.ProjectileType<HerringAI>();
            Item.useAnimation = Item.useTime = 36;
            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item21;
            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

    }
}
