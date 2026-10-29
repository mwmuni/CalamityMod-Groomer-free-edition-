using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class AbyssShellFossil : VanityPetItem<EidolonSnail, EidolonSnailBuff>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        protected override int PetBuffDuration => 15;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(platinum: 1);
            Item.rare = ItemRarityID.Cyan;
            Item.Calamity().devItem = true;
        }

    }
}
