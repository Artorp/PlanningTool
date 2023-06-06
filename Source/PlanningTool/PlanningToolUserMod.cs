using HarmonyLib;
using KMod;
using PeterHan.PLib.Actions;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;

namespace PlanningTool
{
    public class PlanningToolUserMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary(true);
            new PLocalization().Register();
            new POptions().RegisterOptions(this, typeof(ModOptions));
            var pActionManager = new PActionManager();
            ToolKeyBindings.SetupPActionsOnLoad(pActionManager);
            ModOptions.LoadOptions();

#if false
            // will create *.pot next to dll, move to source directory manually
            PTStrings.GenerateTemplate();
#endif
        }

        public static void OnDbInitialized()
        {
            AddStrings();
            PTAssets.Initialize();
            PTObjectTemplates.CreateTemplates();
            ToolKeyBindings.SetActionsIgnoreConflicts();
            ModOptions.OnOptionsChangedEvent += options =>
            {
                PTAssets.LoadShapeTextures(options);
                PTObjectTemplates.CreateTemplates();
            };
        }

        private static void AddStrings()
        {
            // KeyBinding strings
            ToolKeyBindings.AddStrings();
        }
    }
}
