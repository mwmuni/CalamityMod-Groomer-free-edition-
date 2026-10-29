using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    public class WhiteSeekingMechanism : BaseLabSeekingMechanism
    {
        protected override Vector2 LabCenter => CalamityWorld.IceLabCenter;

        protected override int SeekerProjectileType => ModContent.ProjectileType<WhiteLabSeeker>();

        protected override void AddRecipeIngredient(Recipe recipe) => recipe.AddRecipeGroup("AnyIceBlock", 50);
    }
}
