using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ExoMechs.Ares
{
    public abstract class AresCannonBase : ModNPC
    {
        protected abstract string CannonGorePrefix { get; }
        protected virtual bool HasThirdCannonGore => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TerraBlade, 0f, 0f, 100, new Color(0, 255, 255), 1f);

            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 3;
                SoundEngine.PlaySound(CommonCalamitySounds.ExoHitSound, NPC.Center);
            }

            if (NPC.life <= 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TerraBlade, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                }
                for (int j = 0; j < 20; j++)
                {
                    int plasmaDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TerraBlade, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
                    Main.dust[plasmaDust].noGravity = true;
                    Main.dust[plasmaDust].velocity *= 3f;
                    plasmaDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TerraBlade, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                    Main.dust[plasmaDust].velocity *= 2f;
                    Main.dust[plasmaDust].noGravity = true;
                }

                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(CannonGorePrefix + "1").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(CannonGorePrefix + "2").Type, 1f);
                    if (HasThirdCannonGore)
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(CannonGorePrefix + "3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresHandBase1").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresHandBase2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AresHandBase3").Type, 1f);
                }
            }
        }

        public override bool CheckActive() => false;

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }
    }
}
