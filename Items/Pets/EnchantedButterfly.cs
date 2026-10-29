using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    [LegacyName("SparksSummon")]
    public class EnchantedButterfly : VanityPetItem<Sparks, SparksBuff>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Orange;
            Item.Calamity().donorItem = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(). //Oh my god this recipe is terrible no wonder no one knows this item exists
                AddIngredient(ItemID.GoldButterfly).
                AddIngredient(ItemID.MonarchButterfly).
                AddIngredient(ItemID.PurpleEmperorButterfly).
                AddIngredient(ItemID.RedAdmiralButterfly).
                AddIngredient(ItemID.UlyssesButterfly).
                AddIngredient(ItemID.SulphurButterfly).
                AddIngredient(ItemID.TreeNymphButterfly).
                AddIngredient(ItemID.ZebraSwallowtailButterfly).
                AddIngredient(ItemID.JuliaButterfly).
                AddIngredient(ItemID.HellButterfly).
                AddTile(TileID.CrystalBall).
                Register();
        }
    }
}
