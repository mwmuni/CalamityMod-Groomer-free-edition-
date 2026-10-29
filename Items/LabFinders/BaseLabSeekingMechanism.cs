using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    public abstract class BaseLabSeekingMechanism : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.DraedonItems";

        protected abstract Vector2 LabCenter { get; }

        protected abstract int SeekerProjectileType { get; }

        protected abstract void AddRecipeIngredient(Recipe recipe);

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 36;
            Item.shoot = SeekerProjectileType;
            Item.Calamity().MaxCharge = 100;
            Item.Calamity().ChargePerUse = 10;
            Item.Calamity().UsesCharge = true;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ModContent.RarityType<DarkOrange>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && LabCenter != Vector2.Zero;

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<LabSeekingMechanism>();
            AddRecipeIngredient(recipe);
            recipe.AddTile(TileID.Anvils).Register();
        }
    }
}
