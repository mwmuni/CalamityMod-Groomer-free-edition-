using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PrimordialWyrm
{
    [LongDistanceNetSync(SyncWith = typeof(PrimordialWyrmHead))]
    public class PrimordialWyrmBody : PrimordialWyrmBodyBase
    {
        public static Asset<Texture2D> GlowTexture;

        protected override int BodyWidth => 100;
        protected override Asset<Texture2D> Lightmask => GlowTexture;
        protected override string DeathGoreName => "PrimordialWyrm3";

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
            bool spawnAncientLights = (calamityGlobalNPC_Head.newAI[0] == (float)PrimordialWyrmHead.Phase.ShadowFireballSpin && calamityGlobalNPC_Head.newAI[2] > 0f) ||
                (calamityGlobalNPC_Head.newAI[0] == (float)PrimordialWyrmHead.Phase.FinalPhase && calamityGlobalNPC_Head.newAI[1] > 0f);
            if (spawnAncientLights)
            {
                if (Vector2.Distance(NPC.Center, Main.player[Main.npc[(int)NPC.ai[2]].target].Center) > 160f)
                {
                    NPC.localAI[0] += 1f;
                    float spawnAncientLightGateValue = death ? 100f : revenge ? 105f : expertMode ? 110f : 120f;
                    float divisor = 4f;
                    if (NPC.ai[3] % divisor == 0f && NPC.localAI[0] >= spawnAncientLightGateValue)
                    {
                        NPC.localAI[0] = 0f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float distanceVelocityBoost = MathHelper.Clamp((Vector2.Distance(Main.npc[(int)NPC.ai[2]].Center, Main.player[Main.npc[(int)NPC.ai[2]].target].Center) - 1600f) * 0.025f, 0f, 16f);
                            float lightVelocity = (Main.player[Main.npc[(int)NPC.ai[2]].target].Calamity().ZoneAbyssLayer4 ? 6f : 8f) + distanceVelocityBoost;
                            Vector2 destination = Main.player[Main.npc[(int)NPC.ai[2]].target].Center - NPC.Center;
                            Vector2 velocity = Vector2.Normalize(destination) * lightVelocity;
                            int type = NPCID.AncientLight;
                            float ai = (Main.rand.NextFloat() - 0.5f) * 0.3f * MathHelper.TwoPi / 60f;
                            int light = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type, 0, 0f, ai, velocity.X, velocity.Y, 255);
                            Main.npc[light].velocity = velocity;
                        }
                    }
                }
            }
        }
    }
}
