using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PrimordialWyrm
{
    [LongDistanceNetSync(SyncWith = typeof(PrimordialWyrmHead))]
    public class PrimordialWyrmBodyAlt : PrimordialWyrmBodyBase
    {
        public static Asset<Texture2D> GlowTexture;
        public static int FireballDamage = 110; // 440

        protected override int BodyWidth => 110;
        protected override Asset<Texture2D> Lightmask => GlowTexture;
        protected override string DeathGoreName => "PrimordialWyrm4";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "_Lightmask", AssetRequestMode.AsyncLoad);
            }
        }

        protected override void DoVariantAttack(CalamityGlobalNPC calamityGlobalNPC_Head, bool death, bool revenge, bool expertMode)
        {
            bool shootShadowFireballs = (calamityGlobalNPC_Head.newAI[0] == (float)PrimordialWyrmHead.Phase.ShadowFireballSpin && calamityGlobalNPC_Head.newAI[2] > 0f) ||
                (calamityGlobalNPC_Head.newAI[0] == (float)PrimordialWyrmHead.Phase.FinalPhase && calamityGlobalNPC_Head.newAI[1] > 0f);
            if (shootShadowFireballs)
            {
                if (Vector2.Distance(NPC.Center, Main.player[Main.npc[(int)NPC.ai[2]].target].Center) > 160f)
                {
                    NPC.localAI[0] += 1f;
                    float shootShadowFireballGateValue = death ? 70f : revenge ? 75f : expertMode ? 80f : 90f;
                    float divisor = 2f;
                    if (NPC.ai[3] % divisor == 0f && NPC.localAI[0] >= shootShadowFireballGateValue)
                    {
                        NPC.localAI[0] = 0f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float distanceVelocityBoost = MathHelper.Clamp((Vector2.Distance(Main.npc[(int)NPC.ai[2]].Center, Main.player[Main.npc[(int)NPC.ai[2]].target].Center) - 1600f) * 0.025f, 0f, 16f);
                            float fireballVelocity = (Main.player[Main.npc[(int)NPC.ai[2]].target].Calamity().ZoneAbyssLayer4 ? 6f : 8f) + distanceVelocityBoost;
                            Vector2 destination = Main.player[Main.npc[(int)NPC.ai[2]].target].Center - NPC.Center;
                            Vector2 velocity = Vector2.Normalize(destination) * fireballVelocity;
                            int type = ProjectileID.CultistBossFireBallClone;
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, type, FireballDamage, 0f, Main.myPlayer);
                            Main.projectile[proj].tileCollide = false;
                        }
                    }
                }
            }
        }
    }
}
