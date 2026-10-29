using CalamityMod.Buffs.Pets;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class StarSwallowerUnit : VanityPetItem<StarSwallowerPet, StarSwallowerPetBuff>, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        protected override int PetBuffDuration => 15;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Frog).
                AddIngredient<MysteriousCircuitry>(8).
                AddIngredient<DubiousPlating>(4).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
