using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class BloodyVein : VanityPetItem<PerforaMini, BloodBound>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.UseSound = SoundID.NPCHit9;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Orange;
            Item.Calamity().donorItem = true;
        }

    }
}
