using System;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.UI.DraedonLogs;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.DraedonMisc
{
    public class EncryptedSchematicIce : EncryptedSchematic
    {
        protected override string SchematicName => "Ice";

        protected override ref bool HasFoundSchematic => ref RecipeUnlockHandler.HasFoundIceSchematic;

        protected override ref bool HasUnlockedRecipes => ref RecipeUnlockHandler.HasUnlockedT5ArsenalRecipes;

        protected override Type SchematicGUIType => typeof(DraedonSchematicIceGUI);

        protected override SchematicDisplay[] Displays => new[]
        {
            Display<Phaseslayer>("MeleeDisplay", new Color(255, 64, 31)),
            Display<PulseRifle>("RangedDisplay", new Color(201, 41, 255)),
            Display<TheAnomalysNanogun>("RangedDisplay2", new Color(255, 64, 31)),
            Display<TeslaCannon>("MageDisplay", new Color(31, 242, 245)),
            Display<PoleWarper>("SummonDisplay", new Color(236, 255, 31)),
            Display<PlasmaGrenade>("RogueDisplay", new Color(149, 243, 43)),
            Display<AuricQuantumCoolingCell>("CodeDisplay", new Color(255, 215, 0))
        };
    }
}
