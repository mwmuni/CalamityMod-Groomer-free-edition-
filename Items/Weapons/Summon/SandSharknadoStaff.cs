using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class SandSharknadoStaff : SummonStaff, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public const float ProjSpeed = 30f;
        public const float FireSpeed = 50f; // In frames. 60 frames = 1 second.

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 56;
            Item.damage = 60;
            Item.knockBack = 2f;
            Item.mana = 10;

            Item.buffType = ModContent.BuffType<Sandnado>();
            Item.shoot = ModContent.ProjectileType<SandnadoMinion>();
            Item.useAnimation = Item.useTime = 36;
            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item44;
            Item.rare = ItemRarityID.Lime;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ForgottenApexWand>().
                AddIngredient(ItemID.AncientCloth, 2).
                AddIngredient<GrandScale>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
