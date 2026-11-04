using System;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Placeables.PlaceableTurrets;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.UI.DraedonLogs;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.DraedonMisc
{
    public class EncryptedSchematicPlanetoid : EncryptedSchematic
    {
        protected override string SchematicName => "Planetoid";

        protected override ref bool HasFoundSchematic => ref RecipeUnlockHandler.HasFoundPlanetoidSchematic;

        protected override ref bool HasUnlockedRecipes => ref RecipeUnlockHandler.HasUnlockedT2ArsenalRecipes;

        protected override Type SchematicGUIType => typeof(DraedonSchematicPlanetoidGUI);

        protected override SchematicDisplay[] Displays => new[]
        {
            Display<HydraulicVoltCrasher>("MeleeDisplay", new Color(31, 242, 245)),
            Display<HolofibreImmolator>("RangedDisplay", new Color(149, 243, 43)),
            Display<Vulcan>("MageDisplay", new Color(236, 255, 31)),
            Display<MountedScanner>("SummonDisplay", new Color(255, 64, 31)),
            Display<PulseGrenade>("RogueDisplay", new Color(201, 41, 255)),
            Display<FireTurret>("CodeDisplay", MachineColor),
            Display<IceTurret>("CodeDisplay", MachineColor),
            Display<LaserTurret>("CodeDisplay", MachineColor),
            Display<LongRangedSensorArray>("CodeDisplay", MachineColor),
            Display<AstralChunk>("MeleeDisplay", new Color(149, 169, 182))
        };
    }
}
