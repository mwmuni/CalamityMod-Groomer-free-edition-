using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    [LegacyName("IbarakiBox")]
    public class HermitsBoxofOneHundredMedicines : VanityPetItem<ThirdSage, ThirdSageBuff>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.UseSound = SoundID.Item3;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.LightRed;
            Item.Calamity().devItem = true;
        }

    }
}
