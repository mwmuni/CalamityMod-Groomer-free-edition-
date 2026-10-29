using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class RomajedaOrchid : VanityPetItem<KendraPet, Kendra>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        protected override int PetBuffDuration => 15;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.UseSound = SoundID.Item44;
            Item.value = Item.buyPrice(gold: 40); // Sold by Dryad
            Item.rare = ItemRarityID.Pink;
            Item.Calamity().devItem = true;
        }

    }
}
