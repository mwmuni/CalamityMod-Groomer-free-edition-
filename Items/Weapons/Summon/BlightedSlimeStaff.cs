using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    [LegacyName("CorroslimeStaff", "CrimslimeStaff")]
    public class BlightedSlimeStaff : SummonStaff, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 54;
            Item.damage = 18;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<BlightedSlime>();
            Item.shoot = ModContent.ProjectileType<BlightedSlimeMinion>();
            Item.DamageType = DamageClass.Summon;
        }

    }
}
