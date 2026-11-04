using System;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Placeables.PlaceableTurrets;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.UI.DraedonLogs;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.DraedonMisc
{
    public class EncryptedSchematicSunkenSea : EncryptedSchematic
    {
        // No decryption is needed, so holding this schematic unlocks tier 1 directly; its recipe exists for posterity.
        protected override bool UnlocksRecipesWhenFound => true;

        protected override string SchematicName => "Sunken Sea";

        protected override ref bool HasFoundSchematic => ref RecipeUnlockHandler.HasFoundSunkenSeaSchematic;

        protected override ref bool HasUnlockedRecipes => ref RecipeUnlockHandler.HasUnlockedT1ArsenalRecipes;

        protected override Type SchematicGUIType => typeof(DraedonSchematicSunkenSeaGUI);

        protected override string TooltipInsertionMarker => "Tooltip2";

        protected override bool ChangesTooltipWhenUnlocked => false;

        protected override SchematicDisplay[] Displays => new[]
        {
            Display<Auger>("MeleeDisplay", new Color(236, 255, 31)),
            Display<ShortCircuit>("RangedDisplay", new Color(31, 242, 245)),
            Display<PulsePistol>("MageDisplay", new Color(201, 41, 255)),
            Display<AqueousHunterDrone>("SummonDisplay", new Color(149, 243, 43)),
            Display<AerialTracker>("RogueDisplay", new Color(255, 64, 31)),
            Display<WaterTurret>("CodeDisplay", MachineColor),
            Display<OnyxTurret>("CodeDisplay", MachineColor),
            Display<LabTurret>("CodeDisplay", MachineColor),
            Display<DecryptionComputer>("CodeDisplay", MachineColor)
        };
    }
}
