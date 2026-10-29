using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class TrashmanTrashcan : VanityPetItem<DannyDevitoPet, DannyDevito>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        protected override int PetBuffDuration => 15;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.UseSound = SoundID.NPCDeath13;
            Item.value = Item.sellPrice(gold: 1); // "Common drop" pet price (Monster Meat)
            Item.rare = ItemRarityID.Orange;
            Item.Calamity().devItem = true;
        }
    }
}
