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
    public class DeepseaStaff : SummonStaff, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetDefaults()
        {
            Item.width = Item.height = 44;
            Item.damage = 32;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<AquaticStar>();
            Item.shoot = ModContent.ProjectileType<AquaticStarMinion>();
            Item.DamageType = DamageClass.Summon;
        }

    }
}
