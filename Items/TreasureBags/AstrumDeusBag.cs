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
using CalamityMod.NPCs.AstrumDeus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class AstrumDeusBag : BossTreasureBag, ILocalizedModType
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
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<AstrumDeusHead>()));

            // Materials
            itemLoot.Add(ModContent.ItemType<StarblightSoot>(), 1, 60, 90);
            itemLoot.Add(ItemID.FallenStar, 1, 30, 50);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<TheMicrowave>(),
                ModContent.ItemType<StarSputter>(),
                ModContent.ItemType<StarShower>(),
                ModContent.ItemType<StarspawnHelixStaff>(),
                ModContent.ItemType<RegulusRiot>()
            }));

            // Equipment
            itemLoot.Add(ModContent.ItemType<HideofAstrumDeus>());
            itemLoot.AddRevBagAccessories();
            itemLoot.Add(ModContent.ItemType<ChromaticOrb>(), 5);

            // Vanity
            itemLoot.Add(ModContent.ItemType<AstrumDeusMask>(), 7);
            itemLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
        }
    }
}
