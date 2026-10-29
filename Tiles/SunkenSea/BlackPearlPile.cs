using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.Abyss;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    public class BlackPearlPile : PearlPileBase
    {
        protected override int PearlDust => DustID.Lead;
        protected override int FallingProjectile => ModContent.ProjectileType<BlackPearlFalling>();
        protected override Color PearlMapColor => new Color(48, 55, 63);
        protected override Vector3 PearlLightColor => new Vector3(67f / 155f, 68f / 155f, 87f / 155f);
        protected override bool BlendAll => true;

        protected override void RegisterBlendMerges()
        {
            this.RegisterBlendMergeWith(ModContent.TileType<Shellstone>());
            this.RegisterBlendMergeWith(ModContent.TileType<PyreMantle>());
            this.RegisterBlendMergeWith(ModContent.TileType<Navystone>());
            this.RegisterBlendMergeWith(ModContent.TileType<Runestone>());
            this.RegisterBlendMergeWith(TileID.Stone);
            this.RegisterBlendMergeWith(TileID.Dirt);
            this.RegisterBlendMergeWith(TileID.Ash);
            this.RegisterBlendMergeWith(TileID.Mud);
            this.RegisterBlendMergeWith(ModContent.TileType<EutrophicSand>());
            this.RegisterBlendMergeWith(ModContent.TileType<VolcanicSand>());
            this.RegisterBlendMergeWith(TileID.Sandstone);
            this.RegisterBlendMergeWith(TileID.Sand);
            this.RegisterBlendMergeWith(TileID.HardenedSand);
        }
    }
}
