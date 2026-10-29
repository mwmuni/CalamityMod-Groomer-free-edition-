using System.Collections.Generic;
using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class PrimroseKeepsake : VanityPetItem<PrimroseKeepsakeDisplay, FurtasticDuoBuff>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        protected override int PetBuffDuration => 15;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.UseSound = SoundID.Item44;
            Item.value = Item.sellPrice(platinum: 1);
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().devItem = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            List<int> pets = new List<int> { ModContent.ProjectileType<Bear>(), ModContent.ProjectileType<KendraPet>() };
            foreach (int petProjID in pets)
                Projectile.NewProjectile(source, position, velocity, petProjID, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BearsEye>().
                AddIngredient<RomajedaOrchid>().
                AddIngredient(ItemID.LovePotion).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
