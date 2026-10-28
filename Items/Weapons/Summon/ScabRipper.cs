using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class ScabRipper : SummonStaff, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public override void SetStaticDefaults() => Item.staff[Type] = true;

        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 70;
            Item.damage = 10;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item83;
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<BabyBloodCrawlerBuff>();
            Item.shoot = ModContent.ProjectileType<BabyBloodCrawler>();
            Item.DamageType = DamageClass.Summon;

            // This doesn't do anything, it's just so the item is held like a staff.
            Item.shootSpeed = 1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrimtaneBar, 5).
                AddIngredient(ItemID.TissueSample, 9).
                AddIngredient(ItemID.Shadewood, 20).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
