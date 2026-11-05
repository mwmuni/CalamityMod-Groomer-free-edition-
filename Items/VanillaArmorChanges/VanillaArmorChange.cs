using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    // Certain things are virtual instead of abstract because not all pieces of armor are explicitly a part of any particular set.
    // The Wizard Hat is an example of this. The IDs are left abstract so that you don't mistakenly forget to override one of them.
    public abstract class VanillaArmorChange : ILoadable
    {
        public abstract int? HeadPieceID { get; }

        public abstract int? BodyPieceID { get; }

        public abstract int? LegPieceID { get; }

        public virtual int[] AlternativeHeadPieceIDs => Array.Empty<int>();

        public virtual int[] AlternativeBodyPieceIDs => Array.Empty<int>();

        public virtual int[] AlternativeLegPieceIDs => Array.Empty<int>();

        // Overrides build their alternative ID arrays on every access, and these are checked for every player every tick,
        // so they are resolved once on first use (after loading, so modded item IDs are valid).
        private int[] cachedAlternativeHeadPieceIDs;
        private int[] cachedAlternativeBodyPieceIDs;
        private int[] cachedAlternativeLegPieceIDs;

        internal int[] CachedAlternativeHeadPieceIDs => cachedAlternativeHeadPieceIDs ??= AlternativeHeadPieceIDs ?? Array.Empty<int>();

        internal int[] CachedAlternativeBodyPieceIDs => cachedAlternativeBodyPieceIDs ??= AlternativeBodyPieceIDs ?? Array.Empty<int>();

        internal int[] CachedAlternativeLegPieceIDs => cachedAlternativeLegPieceIDs ??= AlternativeLegPieceIDs ?? Array.Empty<int>();

        public virtual string ArmorSetName => null;

        public virtual bool NeedsToCreateSetBonusTextManually => true;

        public virtual void UpdateSetBonusText(ref string setBonusText) { }

        public virtual void ApplyHeadPieceEffect(Player player) { }

        public virtual void ApplyBodyPieceEffect(Player player) { }

        public virtual void ApplyLegPieceEffect(Player player) { }

        public virtual void ApplyArmorSetBonus(Player player) { }

        public bool IsWearingEntireSet(Player player)
        {
            // Check each individual piece of armor on the player with the respective expected item ID.
            // If no ID is applied/it's null, that signals that it doesn't matter, and to just fall through anyway.
            if ((HeadPieceID ?? player.armor[0].type) != player.armor[0].type && !CachedAlternativeHeadPieceIDs.Contains(player.armor[0].type))
                return false;
            if ((BodyPieceID ?? player.armor[1].type) != player.armor[1].type && !CachedAlternativeBodyPieceIDs.Contains(player.armor[1].type))
                return false;
            if ((LegPieceID ?? player.armor[2].type) != player.armor[2].type && !CachedAlternativeLegPieceIDs.Contains(player.armor[2].type))
                return false;

            return true;
        }

        public void ApplyIndividualPieceEffects(Player player)
        {
            if ((HeadPieceID ?? ItemID.None) == player.armor[0].type || CachedAlternativeHeadPieceIDs.Contains(player.armor[0].type))
                ApplyHeadPieceEffect(player);
            if ((BodyPieceID ?? ItemID.None) == player.armor[1].type || CachedAlternativeBodyPieceIDs.Contains(player.armor[1].type))
                ApplyBodyPieceEffect(player);
            if ((LegPieceID ?? ItemID.None) == player.armor[2].type || CachedAlternativeLegPieceIDs.Contains(player.armor[2].type))
                ApplyLegPieceEffect(player);
        }

        public void Load(Mod mod)
        {
            VanillaArmorChangeManager.ArmorChanges.Add(this);
        }

        public void Unload()
        {
            // Don't really need to do anything here yet
        }
    }
}
