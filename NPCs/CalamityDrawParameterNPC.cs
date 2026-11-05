using System.Collections.Generic;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Ares;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    // Why this class is exist?
    // It because we want to avoid Threading Issues while We using NPC's properties on Draw Thread
    public sealed class CalamityDrawParameterNPC : GlobalNPC
    {
        public override bool InstancePerEntity => false;

        #region Draw Parameters
        public static bool[] DrawingMiracleBlight { get; private set; }
        public static bool[] DrawingPolarity { get; private set; }
        public static int DoGDeathAnimationTimer { get; private set; }
        #endregion

        #region Filtering Fields
        private static List<int> miracleBlightExcludedNPCs;

        // Built once on first use since this is checked for every NPC every tick.
        public static List<int> MiracleBlightExcludedNPCs => miracleBlightExcludedNPCs ??= new()
        {
            // List the reason why the NPC(s) are excluded :)

            // The particle sets break with the visuals and this is the easiest way to fix this that isn't stupidly complex.
            ModContent.NPCType<AresBody>(),
            ModContent.NPCType<AresGaussNuke>(),
            ModContent.NPCType<AresLaserCannon>(),
            ModContent.NPCType<AresPlasmaFlamethrower>(),
            ModContent.NPCType<AresTeslaCannon>(),

            // Breaks with being behind tiles, and causes a funny interaction where his head goes behind his neck.
            NPCID.MoonLordCore,
            NPCID.MoonLordHand,
            NPCID.MoonLordHead
        };
        #endregion

        #region Load / Unload
        public override void Load()
        {
            DrawingMiracleBlight = new bool[Main.maxNPCs + 1];
            DrawingPolarity = new bool[Main.maxNPCs + 1];
        }

        public override void Unload()
        {
            DrawingMiracleBlight = null;
            DrawingPolarity = null;
            miracleBlightExcludedNPCs = null;
        }
        #endregion

        public override void SetDefaults(NPC entity) => ResetParameters(entity);
        public static void ResetParameters(NPC npc)
        {
            if (npc is null)
                return;

            if (npc.whoAmI < 0 || npc.whoAmI >= Main.maxNPCs)
                return;

            var whoAmI = npc.whoAmI;
            DrawingMiracleBlight[whoAmI] = false;
            DrawingPolarity[whoAmI] = false;
        }

        public override bool PreAI(NPC npc)
        {
            // These parameters are only read while drawing NPCs, which never happens on a dedicated server.
            if (Main.dedServ)
                return true;

            if (npc is null)
                return true;

            if (npc.whoAmI < 0 || npc.whoAmI >= Main.maxNPCs)
                return true;

            var whoAmI = npc.whoAmI;
            DrawingMiracleBlight[whoAmI] = ShouldDrawMiracleBlight(npc);
            DrawingPolarity[whoAmI] = ShouldDrawPolarity(npc);
            return true;
        }

        public static void PostUpdateNPCs()
        {
            DoGDeathAnimationTimer = GetDoGDeathTimer();
        }

        #region Miracle Blight Effect
        public static bool ShouldDrawMiracleBlight(NPC npc)
        {
            if (npc is null || !npc.active)
                return false;

            // Do not draw weird MP types less than or equal to 0.
            if (npc.type <= NPCID.None)
                return false;

            // Do not draw other mod's bosses.
            if (npc.ModNPC != null && npc.ModNPC.Mod != CalamityMod.Instance && npc.boss)
                return false;

            // Don't draw excluded NPCs, or if the npc is a bestiary dummy.
            if (MiracleBlightExcludedNPCs.Contains(npc.type) || npc.IsABestiaryIconDummy)
                return false;

            // Safety check for weird MP bug when getting global npcs.
            if (!npc.TryGetGlobalNPC<CalamityGlobalNPC>(out var calNPC) || !npc.TryGetGlobalNPC<CalamityPolarityNPC>(out var polNPC))
                return false;

            // Do not draw if the npc does not have miracle blight, or has the polarity effect.
            if (!calNPC.miracleBlight || polNPC.CurPolarity > 0f)
                return false;

            // Do not draw if the current player has the trippy effect.
            if (Main.LocalPlayer.Calamity().trippy)
                return false;

            return true;
        }
        #endregion

        #region Polarity Effect
        public static bool ShouldDrawPolarity(NPC npc)
        {
            if (npc is null || !npc.active)
                return false;

            // Safety check for weird MP bug when getting global npcs.
            if (!npc.TryGetGlobalNPC<CalamityGlobalNPC>(out var calNPC) || !npc.TryGetGlobalNPC<CalamityPolarityNPC>(out var polNPC))
                return false;

            // I don't know who would be using this while also inflicting miracle blight, but in that rare case, do not draw these.
            if (calNPC.miracleBlight)
                return false;

            // Do not draw if the npc doesn't have the polarity effect.
            if (polNPC.CurPolarity <= 0f)
                return false;

            return true;
        }
        #endregion

        #region DoG Death Animation
        public static bool ShouldDrawDoGDeathAnimation(NPC npc)
        {
            // Do not draw inactive npcs, or ones with weird MP types less than or equal to 0.
            if (npc == null || !npc.active || npc.type <= NPCID.None)
                return false;

            if (npc.type != ModContent.NPCType<DevourerofGodsHead>() && npc.type != ModContent.NPCType<DevourerofGodsBody>() && npc.type != ModContent.NPCType<DevourerofGodsTail>())
                return false;

            // Do not draw if Death Animation did not triggered
            if (GetDoGDeathTimer() <= 0)
                return false;

            return true;
        }

        public static int GetDoGDeathTimer()
        {
            if (!Main.npc.IndexInRange(CalamityGlobalNPC.DoGHead))
                return 0;

            var head = Main.npc[CalamityGlobalNPC.DoGHead].ModNPC<DevourerofGodsHead>();
            if (head == null)
                return 0;

            // Don't draw if DoG hasn't started it's death animation yet.
            if (head.DeathAnimationTimer <= 0)
                return 0;

            return head.DeathAnimationTimer;
        }
        #endregion

        #region PostUpdateNPCs Hook
        private sealed class PostUpdateNPCsHook : ModSystem
        {
            public override void PostUpdateNPCs()
            {
                CalamityDrawParameterNPC.PostUpdateNPCs();
            }
        }
        #endregion
    }
}
