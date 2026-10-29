using CalamityMod.Buffs.Pets;
using CalamityMod.Items.Placeables.Crags;
using CalamityMod.Items.Placeables.SunkenSea;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    [LegacyName("RotomRemote")]
    public class TheEtomer : VanityPetItem<RotomPet, ElectricTroublemaker>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.UseSound = SoundID.Item113;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Orange;
            Item.Calamity().devItem = true;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Pets/TheEtomerGlow").Value);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyGoldBar", 8).
                AddIngredient<PrismShard>(5).
                AddIngredient<ScorchedBone>(5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
