using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class ThiefsDime : VanityPetItem<GoldiePet, GoldieBuff>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.UseSound = SoundID.CoinPickup;
            Item.value = Item.buyPrice(platinum: 1); // Sold by Bandit
            Item.rare = ItemRarityID.Pink;
        }

    }
}
