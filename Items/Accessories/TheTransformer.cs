using System.Collections.Generic;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.SunkenSea;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Accessories
{
    public class TheTransformer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            list.FindAndReplace("[GFB]", Lang.SupportGlyphs(this.GetLocalizedValue(Main.zenithWorld ? "TooltipGFB" : "TooltipNormal")));
            list.IntegrateDynamicHotkey(Item);
        }
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 16));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        public static int blobCap = 30;
        public static int blobDamage = 50;
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 56;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.transformer = true;
            modPlayer.transformerVisual = !hideVisual;

            if (player.ownedProjectileCounts[ProjectileType<TransformerAura>()] < 1 && !hideVisual && !player.dead && player.Calamity().transformerCooldown == 0)
            {
                Projectile light = Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ProjectileType<TransformerAura>(), 0, 0f, player.whoAmI);
            }

        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaPrism>(10).
                AddRecipeGroup("AnyMythrilBar", 5).
                AddIngredient<EssenceofSunlight>(2).
                AddIngredient<EssenceofHavoc>(2).
                AddIngredient<EssenceofEleum>(2).
                AddIngredient(ItemID.SoulofLight, 3).
                AddIngredient(ItemID.SoulofNight, 3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            AccessoryInventoryDrawing.DrawCustomScale(Type, spriteBatch, position, frame, drawColor, itemColor, origin, scale, 0.95f);
            return false;
        }
    }
}
