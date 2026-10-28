using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Ravager;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class RavagerBag : BossTreasureBag, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.TreasureBags";
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossBags;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Money
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<RavagerBody>()));

            // Materials (contained in Geodes to prevent overwhelming item floods)
            itemLoot.AddIf(() => !DownedBossSystem.downedProvidence, ModContent.ItemType<FleshyGeode>());
            itemLoot.AddIf(() => DownedBossSystem.downedProvidence, ModContent.ItemType<NecromanticGeode>());

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<UltimusCleaver>(),
                ModContent.ItemType<RealmRavager>(),
                ModContent.ItemType<Hematemesis>(),
                ModContent.ItemType<SpikecragStaff>(),
                ModContent.ItemType<CraniumSmasher>(),
            }));
            itemLoot.Add(ModContent.ItemType<Vesuvius>(), 10);
            itemLoot.Add(ModContent.ItemType<CorpusAvertor>(), 20);

            // Equipment
            itemLoot.Add(ModContent.ItemType<BloodPact>());
            itemLoot.Add(ModContent.ItemType<FleshTotem>(), 2);
            itemLoot.AddIf(() => DownedBossSystem.downedProvidence, ModContent.ItemType<BloodflareCore>());
            itemLoot.AddRevBagAccessories();
            itemLoot.AddIf((info) => CalamityWorld.revenge && !info.player.Calamity().rageBoostTwo, ModContent.ItemType<InfernalBlood>());

            // Vanity
            itemLoot.Add(ModContent.ItemType<RavagerMask>(), 7);
            itemLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
        }
    }
}
