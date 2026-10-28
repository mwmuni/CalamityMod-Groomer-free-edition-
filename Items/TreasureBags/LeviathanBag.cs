using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Placeables.Furniture.Monoliths;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Leviathan;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class LeviathanBag : BossTreasureBag, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.TreasureBags";
        protected override bool ForceIntoWorld => true;
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossBags;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Money
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Leviathan>()));

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<Greentide>(),
                ModContent.ItemType<Leviatitan>(),
                ModContent.ItemType<AnahitasArpeggio>(),
                ModContent.ItemType<Atlantis>(),
                ModContent.ItemType<GastricBelcherStaff>(),
                ModContent.ItemType<Whitewater>(),
                ModContent.ItemType<LeviathanTeeth>(),
            }));

            // Equipment
            itemLoot.Add(ModContent.ItemType<LeviathanAmbergris>());
            itemLoot.Add(ModContent.ItemType<PearlofEnthrallment>(), DropHelper.BagWeaponDropRateFraction);
            itemLoot.Add(ModContent.ItemType<TheCommunity>(), 10);
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<AnahitaMask>(), 7);
            itemLoot.Add(ModContent.ItemType<LeviathanMask>(), 7);
            itemLoot.Add(ModContent.ItemType<DeepSeaAnchor>(), 10);
            itemLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
        }
    }
}
