using System;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.UI.DraedonLogs;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.DraedonMisc
{
    public class EncryptedSchematicHell : EncryptedSchematic
    {
        protected override string SchematicName => "Hell";

        protected override ref bool HasFoundSchematic => ref RecipeUnlockHandler.HasFoundHellSchematic;

        protected override ref bool HasUnlockedRecipes => ref RecipeUnlockHandler.HasUnlockedT4ArsenalRecipes;

        protected override Type SchematicGUIType => typeof(DraedonSchematicHellGUI);

        protected override SchematicDisplay[] Displays => new[]
        {
            Display<PulseDragon>("MeleeDisplay", new Color(201, 41, 255)),
            Display<PhalanxSurge>("RangedDisplay", new Color(255, 64, 31)),
            Display<PlasmaCaster>("MageDisplay", new Color(149, 243, 43)),
            Display<SnakeEyes>("SummonDisplay", new Color(31, 242, 245)),
            Display<WavePounder>("RogueDisplay", new Color(236, 255, 31)),
            Display<VoltageRegulationSystem>("CodeDisplay", MachineColor)
        };
    }
}
