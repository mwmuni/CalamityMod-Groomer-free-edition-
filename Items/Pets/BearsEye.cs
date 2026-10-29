using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    [LegacyName("BearEye")]
    public class BearsEye : VanityPetItem<Bear, BearBuff>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        protected override int PetBuffDuration => 15;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.UseSound = SoundID.Meowmere;
            Item.value = Item.buyPrice(platinum: 5); // Sold by Bandit
            Item.rare = ItemRarityID.Pink;
            Item.Calamity().devItem = true;
        }

    }
}
