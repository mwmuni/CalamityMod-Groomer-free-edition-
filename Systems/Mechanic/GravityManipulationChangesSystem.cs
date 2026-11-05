using CalamityMod.Effects;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems.Mechanic
{
    public class GravityManipulationChangesSystem : ModSystem
    {
        public override void Load()
        {
            On_Player.UpdateControlHolds += DelayGravity;
            On_PlayerInput.SetZoom_MouseInWorld += AdjustMousePositionWithGravityConfig;

            if (!Main.dedServ)
            {
                Main.QueueMainThreadAction(() =>
                {
                    On_Main.DoDraw += ForceGravityDirectionForDrawing;
                    On_LegacyPlayerRenderer.DrawPlayerInternal += FlipPlayerWithConfig;
                });
            }
        }

        public override void Unload()
        {
            On_Player.UpdateControlHolds -= DelayGravity;
            On_PlayerInput.SetZoom_MouseInWorld -= AdjustMousePositionWithGravityConfig;

            if (!Main.dedServ)
            {
                Main.QueueMainThreadAction(() =>
                {
                    On_Main.DoDraw -= ForceGravityDirectionForDrawing;
                    On_LegacyPlayerRenderer.DrawPlayerInternal -= FlipPlayerWithConfig;
                });
            }
        }

        private static void DelayGravity(On_Player.orig_UpdateControlHolds orig, Player Player)
        {
            var cplay = Player.Calamity();
            if (CalamityKeybinds.SwitchGravityHotkey.HasAssignedKeys() && (Player.gravControl || Player.gravControl2) && !Player.mount.Active)
            {
                if (Player.controlUp && Player.releaseUp)
                {
                    Player.gravDir *= -1;
                }
                if (CalamityKeybinds.SwitchGravityHotkey.JustPressed)
                {
                    Player.gravDir *= -1;
                    Player.fallStart = (int)(Player.position.Y / 16f);
                    Player.jump = 0;
                    SoundEngine.PlaySound(SoundID.Item8, Player.position);
                }

                if (Player.forcedGravity > 0)
                {
                    Player.gravDir = -1f;
                }
            }

            if (cplay.justChangedGravity)
            {
                Player.gravDir = cplay.oldGravDir;
            }
            cplay.justChangedGravity = cplay.oldGravDir != Player.gravDir;

            cplay.oldGravDir = Player.gravDir;

            if (cplay.justChangedGravity)
            {
                Player.gravDir *= -1;
            }
            orig(Player);
        }

        private static void AdjustMousePositionWithGravityConfig(On_PlayerInput.orig_SetZoom_MouseInWorld orig)
        {
            orig();
            if (!Main.gameMenu && CalamityClientConfig.Instance.DisableGravityScreenSwap && ((Main.LocalPlayer.gravDir == -1 && !Main.LocalPlayer.Calamity().justChangedGravity) || (Main.LocalPlayer.Calamity().oldGravDir == -1 && Main.LocalPlayer.Calamity().justChangedGravity)))//((Main.LocalPlayer.gravDir == -1 && !Main.LocalPlayer.Calamity().justChangedGravity) || (Main.LocalPlayer.Calamity().oldGravDir == -1 && Main.LocalPlayer.Calamity().justChangedGravity))
            {
                var center = Main.screenHeight / 2;
                Main.mouseY = center - (Main.mouseY - center);
            }
        }

        private void ForceGravityDirectionForDrawing(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
        {
            if (!CalamityClientConfig.Instance.DisableGravityScreenSwap || Main.gameMenu || Main.dedServ)
            {
                orig(self, gameTime);
            }
            else
            {
                Main.LocalPlayer.Calamity().tempGravDir = Main.LocalPlayer.gravDir;
                Main.LocalPlayer.gravDir = 1;
                orig(self, gameTime);
                Main.LocalPlayer.gravDir = Main.LocalPlayer.Calamity().tempGravDir;
            }
        }

        private void FlipPlayerWithConfig(On_LegacyPlayerRenderer.orig_DrawPlayerInternal orig, LegacyPlayerRenderer self, Camera camera, Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow, float alpha, float scale, bool headOnly)
        {
            if (!CalamityClientConfig.Instance.DisableGravityScreenSwap || Main.gameMenu || Main.dedServ || drawPlayer.whoAmI != Main.myPlayer)
            {
                orig(self, camera, drawPlayer, position, rotation, rotationOrigin, shadow, alpha, scale, headOnly);
            }
            else
            {
                drawPlayer.gravDir = drawPlayer.Calamity().tempGravDir;
                orig(self, camera, drawPlayer, position, rotation, rotationOrigin, shadow, alpha, scale, headOnly);
                drawPlayer.gravDir = 1;
            }
        }
    }
}
