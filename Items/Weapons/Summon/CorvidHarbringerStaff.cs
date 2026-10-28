using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CorvidHarbringerStaff : SummonStaff, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 52;
            Item.damage = 99;
            Item.DamageType = DamageClass.Summon;
            Item.useAnimation = Item.useTime = 24;
            Item.mana = 10;
            Item.knockBack = 2f;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.DD2_BetsyFlyingCircleAttack;
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<CorvidHarbringerBuff>();
            Item.shoot = ModContent.ProjectileType<PowerfulRaven>();

            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<CosmicPurple>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RavenStaff).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<NightmareFuel>(20).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
