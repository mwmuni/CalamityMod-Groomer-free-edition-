using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class CosmicPlushie : VanityPetItem<ChibiiDoggo, ChibiiDoGBuff>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        protected override int PetBuffDuration => 15;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.UseSound = SoundID.Meowmere;
            Item.value = Item.sellPrice(gold: 7); // Terry reference
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.Calamity().devItem = true;
        }

    }
}
