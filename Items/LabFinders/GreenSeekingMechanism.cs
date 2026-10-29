using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    public class GreenSeekingMechanism : BaseLabSeekingMechanism
    {
        protected override Vector2 LabCenter => CalamityWorld.JungleLabCenter;

        protected override int SeekerProjectileType => ModContent.ProjectileType<GreenLabSeeker>();

        protected override void AddRecipeIngredient(Recipe recipe) => recipe.AddIngredient(ItemID.Vine, 3);
    }
}
