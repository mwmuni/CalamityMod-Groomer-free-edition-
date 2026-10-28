using System;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class PhantomSpiritL : PhantomSpiritVariant
    {
        public static int ShotDamage = 60; // 240

        protected override int VariantDamage => 100; // 200
        protected override int VariantDefense => 60;
        protected override int VariantLifeMax => 3500;
        protected override float VariantKnockBackResist => 0.1f;
        protected override int ValueInSilver => 60;
        protected override float? ScaleMultiplier => 1.2f;
        protected override float MovementSpeed => (CalamityWorld.death || BossRushEvent.BossRushActive) ? 16f : CalamityWorld.revenge ? 14f : 12f;
        protected override int MinimumNecroplasmDrop => 2;
        protected override int MaximumNecroplasmDrop => 4;

        protected override void SetAIStyleAndDamage()
        {
            NPC.damage = VariantDamage;
            NPC.aiStyle = -1;
        }

        public override void AI()
        {
            base.AI();
            Vector2 spiritPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float targetXDist = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - spiritPosition.X;
            float targetYDist = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - spiritPosition.Y;
            float targetDistance = (float)Math.Sqrt((double)(targetXDist * targetXDist + targetYDist * targetYDist));

            if (NPC.justHit)
                NPC.ai[2] = 0f;

            NPC.ai[2] += 1f;
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[2] >= 150f)
            {
                NPC.ai[2] = 0f;
                float projSpeed = 10f;
                int type = ModContent.ProjectileType<PhantomGhostShot>();
                targetDistance = projSpeed / targetDistance;
                targetXDist *= targetDistance;
                targetYDist *= targetDistance;
                Projectile.NewProjectile(NPC.GetSource_FromAI(), spiritPosition.X, spiritPosition.Y, targetXDist, targetYDist, type, ShotDamage, 0f, Main.myPlayer, 0f, 0f);
            }
        }
    }
}
