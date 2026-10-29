using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    [LegacyName("PurpleSeekingMechanism")]
    public class YellowSeekingMechanism : BaseLabSeekingMechanism
    {
        protected override Vector2 LabCenter => CalamityWorld.PlanetoidLabCenter;

        protected override int SeekerProjectileType => ModContent.ProjectileType<YellowLabSeeker>();

        protected override void AddRecipeIngredient(Recipe recipe) => recipe.AddIngredient(ItemID.Cloud, 20);
    }
}
