using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class Levi : VanityPetItem<LeviPet, LeviBuff>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.Calamity().devItem = true;
        }

    }
}
