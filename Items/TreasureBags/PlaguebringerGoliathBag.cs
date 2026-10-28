using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.PlaguebringerGoliath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class PlaguebringerGoliathBag : BossTreasureBag, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.TreasureBags";
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossBags;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Money
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<PlaguebringerGoliath>()));

            // Materials
            itemLoot.Add(ModContent.ItemType<PlagueCellCanister>(), 1, 25, 30);
            itemLoot.Add(ModContent.ItemType<InfectedArmorPlating>(), 1, 40, 50);
            itemLoot.Add(ItemID.Stinger, 1, 4, 8);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<Virulence>(),
                ModContent.ItemType<Malevolence>(),
                ModContent.ItemType<PlagueStaff>(),
                ModContent.ItemType<FuelCellBundle>(),
                ModContent.ItemType<InfectedRemote>(),
                ModContent.ItemType<TheSyringe>()
            }));
            itemLoot.Add(ModContent.ItemType<Malachite>(), 10);

            // Equipment
            itemLoot.Add(ModContent.ItemType<ToxicHeart>());
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<PlaguebringerGoliathMask>(), 7);
            itemLoot.Add(ModContent.ItemType<PlagueCaller>(), 10);
            itemLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
        }
    }
}
