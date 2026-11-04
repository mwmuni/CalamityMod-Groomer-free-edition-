using System.Collections.Generic;
using System.Linq;
using CalamityMod.Cooldowns;
using CalamityMod.Packets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer
{
    public partial class CalamityPlayer : ModPlayer
    {
        #region Standard Syncs
        internal const int GlobalSyncPacketTimer = 15;
        private const int StandardSyncHeartbeatChecks = 120 / GlobalSyncPacketTimer;
        private bool hasSyncedStandardState;
        private int standardSyncChecksSinceHeartbeat;
        private (float rage, int combatFrames) lastSyncedRage;
        private float lastSyncedAdrenaline;
        private (int damage, int recoveryFrames, int totalRecoveryFrames, int delayFrames) lastSyncedDefenseDamage;

        private void EnterWorldSync()
        {
            StandardSync(force: true);
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            CombatStatsSyncPacket.Send(this, toWho, fromWho);
            MousePositionSyncPacket.Send(this, toWho, fromWho);
            RightClickSyncPacket.Send(this, toWho, fromWho);
            SyncPlayerDrawParameterPacket.Send(this, toWho, fromWho);
            ExaltationDirectionSyncPacket.Send(this, toWho, fromWho);

            if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer)
                SyncCooldownDictionaryPacket.Send(this, toWho, fromWho);
            else if (Main.netMode == NetmodeID.Server && toWho >= 0 && toWho != Player.whoAmI)
                // Cooldown handlers run on clients; ask the owner for a fresh snapshot for the joining peer.
                RequestCooldownSyncPacket.Send(Player.whoAmI);
        }

        internal void StandardSync(bool force = false)
        {
            force |= !hasSyncedStandardState || ++standardSyncChecksSinceHeartbeat >= StandardSyncHeartbeatChecks;
            if (force)
                standardSyncChecksSinceHeartbeat = 0;

            var rageState = (rage, rageCombatFrames);
            var defenseState = (totalDefenseDamage, defenseDamageRecoveryFrames, totalDefenseDamageRecoveryFrames, defenseDamageDelayFrames);

            bool shouldSync = force || rageState != lastSyncedRage || adrenaline != lastSyncedAdrenaline ||
                defenseState != lastSyncedDefenseDamage || rage != 0f || rageCombatFrames != 0 ||
                adrenaline != 0f || totalDefenseDamage != 0 || defenseDamageRecoveryFrames != 0 ||
                defenseDamageDelayFrames != 0;
            if (shouldSync)
            {
                CombatStatsSyncPacket.Send(this);
                lastSyncedRage = rageState;
                lastSyncedAdrenaline = adrenaline;
                lastSyncedDefenseDamage = defenseState;
            }
            hasSyncedStandardState = true;
        }

        internal void MousePositionSync()
        {
            MousePositionSyncPacket.Send(this);
        }

        internal void MouseRotationSync()
        {
            MouseRotationSyncPacket.Send(this);
        }

        internal void MouseRightClickSync()
        {
            RightClickSyncPacket.Send(this);
        }
        #endregion

        #region Creating and Sending Packets
        private bool CanSyncCooldowns => Main.netMode == NetmodeID.Server ||
            Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer;

        public void SyncCooldownAddition(bool server, CooldownInstance cd)
        {
            if (!CanSyncCooldowns)
                return;

            CooldownAdditionPacket.Send(this, cd);
        }

        public void SyncCooldownRemoval(bool server, IList<string> cooldownIDs)
        {
            if (!CanSyncCooldowns)
                return;

            CooldownRemovalPacket.Send(this, cooldownIDs.Select(id => CooldownRegistry.Get(id).netID).ToArray());
        }

        public void SyncCooldownDictionary(bool server)
        {
            if (!CanSyncCooldowns)
                return;

            SyncCooldownDictionaryPacket.Send(this);
        }

        #endregion
    }
}
