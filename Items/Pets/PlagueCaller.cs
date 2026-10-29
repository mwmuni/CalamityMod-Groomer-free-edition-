using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class PlagueCaller : VanityPetItem<PlaguebringerBab, PlaguebringerBabBuff>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Yellow;
        }

    }
}
