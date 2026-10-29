using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class StatisCurse : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.shadowMinions = true;
            modPlayer.statisMinions = true;
            player.GetKnockback<SummonDamageClass>() += 2.75f;
            player.GetDamage<SummonDamageClass>() += 0.12f;
            player.buffImmune[ModContent.BuffType<Shadowflame>()] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StatisBlessing>().
                AddIngredient<TheFirstShadowflame>().
                AddIngredient(ItemID.FragmentStardust, 6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            AccessoryInventoryDrawing.DrawCustomScale(Type, spriteBatch, position, frame, drawColor, itemColor, origin, scale, 1f);
            return false;
        }
    }
}
