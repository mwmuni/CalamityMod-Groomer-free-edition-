using System.Collections.Generic;
using CalamityMod.NPCs.OldDuke;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.ForegroundDrawing.LoopingTextures
{
    public class NuclearTorrentPlayer : ModPlayer
    {
        public bool ShouldDisplayTorrentMonolith = false;
        public override void ResetEffects()
        {
            ShouldDisplayTorrentMonolith = false;
        }
    }
    public class NuclearTorrentForeground : LoopingTextureForeground
    {
        public override Vector2 ParallaxDepth => new Vector2(1f, 1f);

        public override float IntensityMaximum => 0.045f;

        public static List<NuclearRaindrop> Raindrops = new List<NuclearRaindrop>();

        public class NuclearRaindrop
        {
            public Vector2 Position;
            public Vector2 Velocity;

            public NuclearRaindrop(Vector2 pos, Vector2 vel)
            {
                Position = pos;
                Velocity = vel;
            }

            public void Update(NuclearRaindrop drop, NuclearTorrentForeground foreground)
            {
                drop.Position += drop.Velocity;
                drop.Position -= Main.LocalPlayer.velocity;

                drop.Velocity = new Vector2(0, 20).RotatedBy(-Main.windSpeedCurrent);

                if (drop.Position.Y > Main.screenHeight + 200)
                {
                    RemoveDrop(drop);                
                }
            }

            public void Draw(NuclearRaindrop drop, NuclearTorrentForeground foreground)
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>("CalamityMod/ForegroundDrawing/LoopingTextures/NuclearTorrentRaindrop");

                float intensity = foreground.Intensity / foreground.IntensityMaximum / 4f;

                Main.EntitySpriteDraw(tex.Value, drop.Position, tex.Frame(), Color.White.MultiplyRGBA(new Color(intensity, intensity, intensity, intensity)), Vector2.Zero.AngleTo(drop.Velocity) - MathHelper.ToRadians(90f), tex.Size() / 2, new Vector2(1f, 4f), SpriteEffects.None);
            }
        }

        public static void RemoveDrop(NuclearRaindrop drop)
        {
            Raindrops.Remove(drop);
        }

        public bool ShouldDisplayDuringOldDuke() => NPC.CountNPCS(ModContent.NPCType<OldDuke>()) > 0 && Main.LocalPlayer.Calamity().ZoneSulphur && !CalamityServerConfig.Instance.BossesStopWeather;

        public override bool DoesThisShow() => ShouldDisplayDuringOldDuke() || Main.LocalPlayer.GetModPlayer<NuclearTorrentPlayer>().ShouldDisplayTorrentMonolith;

        public override void Update()
        {
            if (NPC.CountNPCS(ModContent.NPCType<OldDuke>()) > 0)
            {
                int duke = NPC.FindFirstNPC(ModContent.NPCType<OldDuke>());

                Main.windSpeedCurrent = MathHelper.Lerp(Main.windSpeedCurrent, MathHelper.ToRadians(50f), 0.005f);

                foreach (Rain rain in Main.rain)
                {
                    rain.velocity = new Vector2(0, 15).RotatedBy(-Main.windSpeedCurrent);
                    rain.rotation = Vector2.Zero.AngleTo(rain.velocity) + MathHelper.PiOver2;
                }
            }

            // Raindrops are purely visual; dedicated servers never draw them.
            if (Main.dedServ)
                return;

            NuclearRaindrop dr = new NuclearRaindrop(new Vector2(Main.screenWidth / 2f, -100) + new Vector2(Main.rand.NextFloat(-Main.screenWidth, Main.screenWidth), -Main.screenWidth * 0.65f).RotatedBy(-Main.windSpeedCurrent),
               new Vector2(0, 20).RotatedBy(-Main.windSpeedCurrent));
            Raindrops.Add(dr);

            for (int i = 0; i < Raindrops.Count; i++)
            {
                NuclearRaindrop drop = Raindrops[i];

                drop.Update(drop, this);
            }
        }

        public override void PostDraw()
        {
            for (int i = 0; i < Raindrops.Count; i++)
            {
                NuclearRaindrop drop = Raindrops[i];

                drop.Draw(drop, this);
            }
        }
    }
}
