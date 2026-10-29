using System.IO;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Pets;
using CalamityMod.Packets;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.DraedonSummoner;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.TileEntities
{
    public class TECodebreaker : TEBasePlaceable
    {
        public int InputtedCellCount;
        public int InitialCellCountBeforeDecrypting;
        public int HeldSchematicID;

        protected override int PlacementTileWidth => CodebreakerTile.Width;
        protected override int PlacementTileHeight => CodebreakerTile.Height;

        public int DecryptionCountdown;
        public int DecryptionTotalTime
        {
            get
            {
                // Decryption takes 2 minutes typically.
                int decryptTime = 7200;

                // However, if this codebreaker has a quantum cell, it only takes 15 seconds.
                if (ContainsCoolingCell)
                    decryptTime = 900;
                return decryptTime;
            }
        }

        public int DecryptionCellCost
        {
            get
            {
                // You can't decrypt nothing.
                if (HeldSchematicID == 0 || !EncryptedSchematicIDRelationshipDict.TryGet(HeldSchematicID, out var schematicItemType))
                    return 0;

                if (schematicItemType == ModContent.ItemType<EncryptedSchematicPlanetoid>())
                    return 500;
                if (schematicItemType == ModContent.ItemType<EncryptedSchematicJungle>())
                    return 950;
                if (schematicItemType == ModContent.ItemType<EncryptedSchematicHell>())
                    return 1750;
                if (schematicItemType == ModContent.ItemType<EncryptedSchematicIce>())
                    return 5000;

                return 0;
            }
        }

        public float DecryptionCompletion => 1f - DecryptionCountdown / (float)DecryptionTotalTime;
        public bool ReadyToSummonDraedon => ContainsCoolingCell && DecryptionCountdown <= 0;

        public bool ContainsDecryptionComputer;
        public bool ContainsSensorArray;
        public bool ContainsAdvancedDisplay;
        public bool ContainsVoltageRegulationSystem;
        public bool ContainsCoolingCell;

        public bool ContainsBloodyVein;

        public bool CanDecryptHeldSchematic
        {
            get
            {
                // You can't decrypt nothing.
                if (HeldSchematicID == 0 || !EncryptedSchematicIDRelationshipDict.TryGet(HeldSchematicID, out var schematicItemType))
                    return false;

                if (schematicItemType == ModContent.ItemType<EncryptedSchematicPlanetoid>())
                    return ContainsDecryptionComputer;
                if (schematicItemType == ModContent.ItemType<EncryptedSchematicJungle>())
                    return ContainsDecryptionComputer && ContainsSensorArray;
                if (schematicItemType == ModContent.ItemType<EncryptedSchematicHell>())
                    return ContainsDecryptionComputer && ContainsSensorArray && ContainsAdvancedDisplay;
                if (schematicItemType == ModContent.ItemType<EncryptedSchematicIce>())
                    return ContainsDecryptionComputer && ContainsSensorArray && ContainsAdvancedDisplay && ContainsVoltageRegulationSystem;

                return false;
            }
        }

        public string UnderlyingSchematicText
        {
            get
            {
                // You can't decrypt nothing.
                if (HeldSchematicID == 0 || !EncryptedSchematicIDRelationshipDict.TryGet(HeldSchematicID, out var schematicItemType))
                    return string.Empty;

                if (schematicItemType == ModContent.ItemType<EncryptedSchematicPlanetoid>()
                || schematicItemType == ModContent.ItemType<EncryptedSchematicJungle>()
                || schematicItemType == ModContent.ItemType<EncryptedSchematicHell>()
                || schematicItemType == ModContent.ItemType<EncryptedSchematicIce>())
                    return CalamityUtils.GetTextValueFromModItem(schematicItemType, "Content");

                return string.Empty;
            }
        }

        public Vector2 Center => Position.ToWorldCoordinates(8f * CodebreakerTile.Width, 8f * CodebreakerTile.Height);
        public const int MaxCellCapacity = 9999;

        // This guarantees that this tile entity will not persist if not placed directly on the top left corner of a Codebreaker tile.
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
            return tile.HasTile && tile.TileType == ModContent.TileType<CodebreakerTile>() && tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }

        public override void Update() => UpdateTime();

        public void DropConstituents(int x, int y)
        {
            var source = new EntitySource_TileEntity(this);
            if (ContainsDecryptionComputer)
                Item.NewItem(source, x * 16, y * 16, 32, 32, ModContent.ItemType<DecryptionComputer>());
            if (ContainsSensorArray)
                Item.NewItem(source, x * 16, y * 16, 32, 32, ModContent.ItemType<LongRangedSensorArray>());
            if (ContainsAdvancedDisplay)
                Item.NewItem(source, x * 16, y * 16, 32, 32, ModContent.ItemType<AdvancedDisplay>());
            if (ContainsVoltageRegulationSystem)
                Item.NewItem(source, x * 16, y * 16, 32, 32, ModContent.ItemType<VoltageRegulationSystem>());
            if (ContainsCoolingCell)
                Item.NewItem(source, x * 16, y * 16, 32, 32, ModContent.ItemType<AuricQuantumCoolingCell>());

            if (EncryptedSchematicIDRelationshipDict.TryGet(HeldSchematicID, out var schematicItemType))
                Item.NewItem(source, x * 16, y * 16, 32, 32, schematicItemType);

            while (InputtedCellCount > 0)
            {
                int totalCellsToDrop = InputtedCellCount;
                if (totalCellsToDrop > 999)
                    totalCellsToDrop = 999;

                InputtedCellCount -= totalCellsToDrop;
                int itemType = ContainsBloodyVein ? ModContent.ItemType<BloodyVein>() : ModContent.ItemType<DraedonPowerCell>();
                Item.NewItem(new EntitySource_TileEntity(this), x * 16, y * 16, 32, 32, itemType, totalCellsToDrop);
            }
        }

        public void SyncConstituents(short sender)
        {
            // Don't bother sending packets in singleplayer.
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            TEUpdateCodebreakerConstituentsPacket.Send(this);
        }

        public void SyncContainedStuff()
        {
            // Don't bother sending packets in singleplayer.
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            TEUpdateCodebreakerContainedStuffPacket.Send(this);
        }

        public void SyncDecryptCountdown()
        {
            // Don't bother sending packets in singleplayer.
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            TEUpdateCodebreakerDecryptCountdownPacket.Send(this);
        }

        public void UpdateTime()
        {
            if (DecryptionCountdown > 0)
            {
                DecryptionCountdown--;

                // Gradually consume cells.
                if (DecryptionCountdown % 5 == 0)
                {
                    InputtedCellCount = InitialCellCountBeforeDecrypting - (int)(DecryptionCellCost * DecryptionCompletion);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        SyncContainedStuff();
                }

                if (DecryptionCountdown == 0)
                {
                    // Reset the cell count prior to decrypting.
                    InitialCellCountBeforeDecrypting = 0;

                    LearnFromHeldSchematic(out bool anythingChanged);
                    if (Main.dedServ)
                    {
                        SyncDecryptCountdown();
                        SyncContainedStuff();
                    }
                    else if (anythingChanged)
                        CombatText.NewText(Main.LocalPlayer.Hitbox, Color.Cyan, CalamityUtils.GetTextValue("Misc.LearnedSchematic"), true);
                }
            }
        }

        public void LearnFromHeldSchematic(out bool anythingChanged)
        {
            anythingChanged = false;

            // Do nothing if no valid schematic is inputted at the moment.
            if (!EncryptedSchematicIDRelationshipDict.TryGet(HeldSchematicID, out var schematicItemType))
                return;

            if (!RecipeUnlockHandler.HasUnlockedT2ArsenalRecipes && schematicItemType == ModContent.ItemType<EncryptedSchematicPlanetoid>())
            {
                RecipeUnlockHandler.HasUnlockedT2ArsenalRecipes = true;
                anythingChanged = true;
            }
            if (!RecipeUnlockHandler.HasUnlockedT3ArsenalRecipes && schematicItemType == ModContent.ItemType<EncryptedSchematicJungle>())
            {
                RecipeUnlockHandler.HasUnlockedT3ArsenalRecipes = true;
                anythingChanged = true;
            }
            if (!RecipeUnlockHandler.HasUnlockedT4ArsenalRecipes && schematicItemType == ModContent.ItemType<EncryptedSchematicHell>())
            {
                RecipeUnlockHandler.HasUnlockedT4ArsenalRecipes = true;
                anythingChanged = true;
            }
            if (!RecipeUnlockHandler.HasUnlockedT5ArsenalRecipes && schematicItemType == ModContent.ItemType<EncryptedSchematicIce>())
            {
                RecipeUnlockHandler.HasUnlockedT5ArsenalRecipes = true;
                anythingChanged = true;
            }

            if (Main.dedServ && anythingChanged)
                CalamityNetcode.SyncWorld();
        }

        public override void SaveData(TagCompound tag)
        {
            tag["ContainsDecryptionComputer"] = ContainsDecryptionComputer;
            tag["ContainsSensorArray"] = ContainsSensorArray;
            tag["ContainsAdvancedDisplay"] = ContainsAdvancedDisplay;
            tag["ContainsVoltageRegulationSystem"] = ContainsVoltageRegulationSystem;
            tag["ContainsCoolingCell"] = ContainsCoolingCell;
            tag["InputtedCellCount"] = InputtedCellCount;
            tag["HeldSchematicID"] = HeldSchematicID;
            tag["DecryptionCountdown"] = DecryptionCountdown;
            tag["InitialCellCountBeforeDecrypting"] = InitialCellCountBeforeDecrypting;
            tag["ContainsBloodyVein"] = ContainsBloodyVein;
        }

        public override void LoadData(TagCompound tag)
        {
            ContainsDecryptionComputer = tag.GetBool("ContainsDecryptionComputer");
            ContainsSensorArray = tag.GetBool("ContainsSensorArray");
            ContainsAdvancedDisplay = tag.GetBool("ContainsAdvancedDisplay");
            ContainsVoltageRegulationSystem = tag.GetBool("ContainsVoltageRegulationSystem");
            ContainsCoolingCell = tag.GetBool("ContainsCoolingCell");
            InputtedCellCount = tag.GetInt("InputtedCellCount");
            HeldSchematicID = tag.GetInt("HeldSchematicID");
            DecryptionCountdown = tag.GetInt("DecryptionCountdown");
            InitialCellCountBeforeDecrypting = tag.GetInt("InitialCellCountBeforeDecrypting");
            ContainsBloodyVein = tag.GetBool("ContainsBloodyVein");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(ContainsDecryptionComputer);
            writer.Write(ContainsSensorArray);
            writer.Write(ContainsAdvancedDisplay);
            writer.Write(ContainsVoltageRegulationSystem);
            writer.Write(ContainsCoolingCell);
            writer.Write(InputtedCellCount);
            writer.Write(HeldSchematicID);
            writer.Write(DecryptionCountdown);
            writer.Write(InitialCellCountBeforeDecrypting);
            writer.Write(ContainsBloodyVein);
        }

        public override void NetReceive(BinaryReader reader)
        {
            ContainsDecryptionComputer = reader.ReadBoolean();
            ContainsSensorArray = reader.ReadBoolean();
            ContainsAdvancedDisplay = reader.ReadBoolean();
            ContainsVoltageRegulationSystem = reader.ReadBoolean();
            ContainsCoolingCell = reader.ReadBoolean();
            InputtedCellCount = reader.ReadInt32();
            HeldSchematicID = reader.ReadInt32();
            DecryptionCountdown = reader.ReadInt32();
            InitialCellCountBeforeDecrypting = reader.ReadInt32();
            ContainsBloodyVein = reader.ReadBoolean();
        }
    }
}
