using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DarkSunRing : ModItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Accessories";
        public bool HasFlavorTooltip => true;
        public Color? TooltipExtensionColor => new(195, 223, 255);

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 7));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 60;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.defense = 4;
            Item.lifeRegen = 1;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<CosmicPurple>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.darkSunRing = true;
            player.noKnockback = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UelibloomBar>(10).
                AddIngredient<DarksunFragment>(20).
                AddTile<CosmicAnvil>().
                Register();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            AccessoryInventoryDrawing.DrawCustomScale(Type, spriteBatch, position, frame, drawColor, itemColor, origin, scale, 0.8f);
            return false;
        }
    }
}
