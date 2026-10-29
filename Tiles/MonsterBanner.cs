using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.Deconstructors;
using CalamityMod.NPCs.DraedonLabThings;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.NormalNPCs.HorribleHog;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Tiles
{
    public class MonsterBanner : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.MultiTileSway[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom | AnchorType.PlanterBox, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.DrawYOffset = -10;
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);
            DustType = -1;
            AddMapEntry(new Color(13, 88, 130), Language.GetText("MapObject.Banner"));
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
                return;

            int style = Main.tile[i, j].TileFrameX / 18;
            int npc = GetBannerNPC(style);
            if (npc == -1)
                return;

            int itemType = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
            if (ItemID.Sets.BannerStrength.IndexInRange(itemType) && ItemID.Sets.BannerStrength[itemType].Enabled)
            {
                Main.SceneMetrics.NPCBannerBuff[npc] = true;
                Main.SceneMetrics.hasBanner = true;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => CalamityUtils.DrawSwayingMultiTile(i, j);

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY += 2;

        public static int GetBannerNPC(int style)
        {
            switch (style)
            {
                case 0: return NPCType<RepairUnitCritter>();
                case 1: return NPCType<Sulflounder>();
                case 2: return NPCType<Gnasher>();
                case 3: return NPCType<Trasher>();
                case 4: return NPCType<Toxicatfish>();
                case 5: return NPCType<SlabCrab>();
                case 6: return NPCType<Androomba>();
                case 7: return NPCType<AquaticUrchin>();
                case 8: return NPCType<Frogfish>();
                case 9: return NPCType<MantisShrimp>();
                case 10: return NPCType<AuroraSpirit>();
                case 11: return NPCType<WildBumblebirb>(); // There's also the boss variant but I dont think we want banners to affect them?
                /*case 12:
                    npc = NPCType<Unused>(); - Formerly Sea Urchin
                    break*/
                case 13: return NPCType<BoxJellyfish>();
                case 14: return NPCType<MorayEel>();
                case 15: return NPCType<DevilFish>();
                case 16: return NPCType<Cuttlefish>();
                case 17: return NPCType<ToxicMinnow>();
                case 18: return NPCType<Viperfish>();
                case 19: return NPCType<LuminousCorvina>();
                case 20: return NPCType<GiantSquid>();
                case 21: return NPCType<Laserfish>();
                case 22: return NPCType<OarfishHead>();
                case 23: return NPCType<ColossalSquid>();
                case 24: return NPCType<MirageJelly>();
                case 25: return NPCType<Eidolist>();
                case 26: return NPCType<GulperEelHead>();
                case 27: return NPCType<EidolonWyrmHead>();
                case 28: return NPCType<Bloatfish>();
                case 29: return NPCType<BobbitWormHead>();
                case 30: return NPCType<ChaoticPuffer>();
                case 31: return NPCType<AstralProbe>();
                case 32: return NPCType<SightseerCollider>();
                case 33: return NPCType<SightseerSpitter>();
                case 34: return NPCType<Aries>();
                case 35: return NPCType<AstralSlime>();
                case 36: return NPCType<Atlas>();
                case 37: return NPCType<Mantis>();
                case 38: return NPCType<Nova>();
                case 39: return NPCType<AstralachneaGround>();
                case 40: return NPCType<Astraglomerate>();
                case 41: return NPCType<StellarCulex>();
                case 42: return NPCType<FusionFeeder>();
                case 43: return NPCType<Hadarian>();
                case 44: return NPCType<HeatSpirit>();
                case 45: return NPCType<Scryllar>();
                case 46: return NPCType<DespairStone>();
                case 47: return NPCType<SoulSlurper>();
                case 48: return NPCType<ImpiousImmolator>();
                case 49: return NPCType<ScornEater>();
                case 50: return NPCType<ProfanedEnergyBody>();
                case 51: return NPCType<Shroomble>();
                case 52: return NPCType<WulfrumDrone>();
                case 53: return NPCType<Rotdog>();
                case 54: return NPCType<CladCrab>();
                case 55: return NPCType<CalamityEye>();
                case 56: return NPCType<Sunskater>();
                case 57: return NPCType<ShockstormShuttle>();
                case 58: return NPCType<CloudElemental>();
                case 59: return NPCType<Rimehound>();
                case 60: return NPCType<Cryon>();
                case 61: return NPCType<IceClasper>();
                case 62: return NPCType<Stormlion>();
                case 63: return NPCType<Cnidrion>();
                /*case 64:
                    npc = NPCType<Unused>(); - Formerly Sand Tortoise
                    break*/
                /*case 65:
                    npc = NPCType<Unused>(); - Formerly Great Sand Shark
                    break*/
                case 66: return NPCType<CrawlerAmethyst>();
                case 67: return NPCType<CrawlerTopaz>();
                case 68: return NPCType<CrawlerSapphire>();
                case 69: return NPCType<CrawlerEmerald>();
                case 70: return NPCType<CrawlerRuby>();
                case 71: return NPCType<CrawlerDiamond>();
                case 72: return NPCType<CrawlerAmber>();
                case 73: return NPCType<CrawlerCrystal>();
                /*case 74:
                    npc = NPCType<Unused>(); - Formerly Sun Bat
                    break*/
                /*case 75:
                    npc = NPCType<Unused>(); - Formerly Cosmic Elemental
                    break*/
                case 76: return NPCType<EarthElemental>();
                case 77: return NPCType<Burrower>();
                case 78: return NPCType<Melter>();
                case 79: return NPCType<PestilentSlime>();
                case 80: return NPCType<Plagueshell>();
                case 81: return NPCType<PlagueCharger>();
                case 82: return NPCType<Viruling>();
                case 83: return NPCType<PlaguebringerMiniboss>();
                case 84: return NPCType<PhantomSpirit>();
                case 85: return NPCType<OverloadedSoldier>();
                /*case 86:
                    npc = NPCType<Unused>(); - Formerly Phantom Debris
                    break*/
                case 87: return NPCType<Bohldohr>();
                case 88: return NPCType<EbonianBlightSlime>();
                case 89: return NPCType<CrimulanBlightSlime>();
                case 90: return NPCType<AeroSlime>();
                case 91: return NPCType<CryoSlime>();
                case 92: return NPCType<PerennialSlime>();
                case 93: return NPCType<InfernalCongealment>();
                case 94: return NPCType<BloomSlime>();
                case 95: return NPCType<RenegadeWarlock>();
                case 96: return NPCType<ReaperShark>();
                case 97: return NPCType<IrradiatedSlime>();
                case 98: return NPCType<PrismBack>();
                case 99: return NPCType<Clam>();
                case 100: return NPCType<EutrophicRay>();
                case 101: return NPCType<GhostBell>();
                case 102: return NPCType<BabyGhostBell>();
                case 103: return NPCType<SeaFloaty>();
                case 104: return NPCType<BlindedAngler>();
                case 105: return NPCType<SeaMinnow>();
                case 106: return NPCType<SeaSerpent1>();
                /*case 107:
                    npc = NPCType<Unused>(); - Formerly Giant Clam
                    break*/
                case 108: return NPCType<Piggy>();
                case 109: return NPCType<FearlessGoldfishWarrior>();
                case 110: return NPCType<Radiator>();
                case 111: return NPCType<Trilobite>();
                case 112: return NPCType<Orthocera>();
                case 113: return NPCType<Skyfin>();
                /*case 114:
                    npc = NPCType<Unused>(); - Formerly Water Leech
                    break*/
                case 115: return NPCType<AcidEel>();
                case 116: return NPCType<NuclearToad>();
                case 117: return NPCType<FlakCrab>();
                case 118: return NPCType<SulphurousSkater>();
                case 119: return NPCType<BabyFlakCrab>();
                case 120: return NPCType<AnthozoanCrab>();
                case 121: return NPCType<BelchingCoral>();
                case 122: return NPCType<GammaSlime>();
                case 123: return NPCType<WulfrumGyrator>();
                case 124: return NPCType<WulfrumHovercraft>();
                case 125: return NPCType<WulfrumRover>();
                case 126: return NPCType<WulfrumAmplifier>();
                case 127: return NPCType<CannonballJellyfish>();
                case 128: return NPCType<BabyCannonballJellyfish>();
                // Hello source code diver, here are a bunch of Sunken Sea enemy names, have fun
                /*case 129:
                    npc = NPCType<Sharkoon>();
                    break; */
                /* case 130:
                    npc = NPCType<Probesnout>();
                    break; */
                /* case 131:
                    npc = NPCType<CarrionSwallower>();
                    break; */
                /* case 132:
                    npc = NPCType<GildedAxolotl>();
                    break; */
                /* case 133:
                    npc = NPCType<Grubbolt>();
                    break; */
                /* case 134:
                    npc = NPCType<HauntedChum>();
                    break; */
                /* case 135:
                    npc = NPCType<Jellyghoul>();
                    break; */
                /* case 136:
                    npc = NPCType<KelpieSeadragon>();
                    break; */
                /* case 137:
                    npc = NPCType<Leerslug>();
                    break; */
                /* case 138:
                    npc = NPCType<LostShoal>();
                    break; */
                /* case 139:
                    npc = NPCType<MirageNewt>();
                    break; */
                /* case 140:
                    npc = NPCType<Panasea>();
                    break; */
                /* case 141:
                    npc = NPCType<PearlpodWhite>(); // Logic should be applying to other variants
                    break; */
                /* case 142:
                    npc = NPCType<StormlionSentry>();
                    break; */
                /* case 143:
                    npc = NPCType<PodobooKoi>();
                    break; */
                /* case 144:
                    npc = NPCType<Polyperil>();
                    break; */
                /* case 145:
                    npc = NPCType<PrismaticGuppy>();
                    break; */
                /* case 146:
                    npc = NPCType<Scavenger>();
                    break; */
                /* case 147:
                    npc = NPCType<Searslug>();
                    break; */
                /* case 148:
                    npc = NPCType<Shoreskipper>();
                    break; */
                /* case 149:
                    npc = NPCType<Slugbun>();
                    break; */
                /* case 150:
                    npc = NPCType<Steampod>();
                    break; */
                /* case 151:
                    npc = NPCType<StormlionScout>();
                    break; */
                case 152: return NPCType<HorribleHog>();
                default: return -1;
            }
        }
    }
}
