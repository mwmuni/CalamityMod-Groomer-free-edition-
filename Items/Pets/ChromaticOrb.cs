using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class ChromaticOrb : VanityPetItem<BendyPet, Dreamfog>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Cyan;
            Item.Calamity().donorItem = true;
        }

    }
}
