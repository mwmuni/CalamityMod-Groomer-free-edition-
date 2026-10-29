using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    public class OnyxSeekingMechanism : BaseLabSeekingMechanism
    {
        protected override Vector2 LabCenter => CalamityWorld.CavernLabCenter;

        protected override int SeekerProjectileType => ModContent.ProjectileType<OnyxLabSeeker>();

        // Temporary recipe probably.
        protected override void AddRecipeIngredient(Recipe recipe) => recipe.AddIngredient(ItemID.Obsidian, 25);
    }
}
