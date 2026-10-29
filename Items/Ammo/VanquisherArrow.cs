using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class VanquisherArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<GodSlayerInferno>()];
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults() =>
            RangedAmmoDefaults.SetDefaults(
                Item, 22, 46, 16, 3.5f,
                Item.sellPrice(copper: 28), ModContent.RarityType<CosmicPurple>(),
                ModContent.ProjectileType<VanquisherArrowProj>(), 0.1f, AmmoID.Arrow);

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Ammo/VanquisherArrowGlow").Value);
        }

        public override void AddRecipes()
        {
            CreateRecipe(999).
                AddIngredient<CosmiliteBar>().
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
