using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    [LegacyName("BumblebirbBag")]
    public class DragonfollyBag : BossTreasureBag, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.TreasureBags";
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossBags;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Money
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Dragonfolly>()));

            // Materials
            itemLoot.Add(ModContent.ItemType<EffulgentFeather>(), 1, 30, 35);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<GildedProboscis>(),
                ModContent.ItemType<GoldenEagle>(),
                ModContent.ItemType<RougeSlash>()
            }));

            // Equipment
            itemLoot.Add(ModContent.ItemType<FollyFeed>(), DropHelper.BagWeaponDropRateFraction);
            itemLoot.Add(ModContent.ItemType<DynamoStemCells>());
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<BumblefuckMask>(), 7);
            itemLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

            // Other
            itemLoot.AddIf((info) => CalamityWorld.revenge && !info.player.Calamity().rageBoostThree, ModContent.ItemType<RedLightningContainer>());
        }
    }
}
