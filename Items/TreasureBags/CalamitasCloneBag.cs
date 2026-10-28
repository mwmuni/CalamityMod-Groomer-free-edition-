using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.CalClone;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    [LegacyName("CalamitasBag")]
    public class CalamitasCloneBag : BossTreasureBag, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.TreasureBags";
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossBags;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Money
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<CalamitasClone>()));

            // Materials
            itemLoot.Add(ModContent.ItemType<AshesofCalamity>(), 1, 30, 35);
            itemLoot.Add(ModContent.ItemType<EssenceofHavoc>(), 1, 10, 12);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<Oblivion>(),
                ModContent.ItemType<Animosity>(),
                ModContent.ItemType<LashesofChaos>(),
                ModContent.ItemType<EntropysVigil>()
            }));

            // Equipment
            itemLoot.Add(ModContent.ItemType<ChaosStone>(), DropHelper.BagWeaponDropRateFraction);
            itemLoot.Add(ModContent.ItemType<Regenerator>(), DropHelper.BagWeaponDropRateFraction);
            itemLoot.Add(ModContent.ItemType<VoidofCalamity>());
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<CalamitasCloneMask>(), 7);
            var calamityRobes = ItemDropRule.Common(ModContent.ItemType<RobesOfCalamity>(), 10);
            calamityRobes.OnSuccess(ItemDropRule.Common(ModContent.ItemType<HoodOfCalamity>()));
            itemLoot.Add(calamityRobes);
            itemLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
        }
    }
}
