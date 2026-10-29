using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class LittleLight : VanityPetItem<LittleLightProj, LittleLightBuff>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.UseSound = SoundID.Item83;
            Item.value = Item.sellPrice(gold: 12);
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.Calamity().donorItem = true;
        }

    }
}
