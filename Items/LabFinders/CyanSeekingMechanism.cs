using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    public class CyanSeekingMechanism : BaseLabSeekingMechanism
    {
        protected override Vector2 LabCenter => CalamityWorld.SunkenSeaLabCenter;

        protected override int SeekerProjectileType => ModContent.ProjectileType<CyanLabSeeker>();

        protected override void AddRecipeIngredient(Recipe recipe) => recipe.AddIngredient(ItemID.WaterBucket);
    }
}
