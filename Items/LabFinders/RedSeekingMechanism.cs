using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    public class RedSeekingMechanism : BaseLabSeekingMechanism
    {
        protected override Vector2 LabCenter => CalamityWorld.HellLabCenter;

        protected override int SeekerProjectileType => ModContent.ProjectileType<RedLabSeeker>();

        protected override void AddRecipeIngredient(Recipe recipe) => recipe.AddIngredient(ItemID.HellstoneBar, 4);
    }
}
