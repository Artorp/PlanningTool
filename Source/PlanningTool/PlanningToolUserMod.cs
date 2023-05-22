using HarmonyLib;
using KMod;
using PeterHan.PLib.Actions;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PlanningTool.HarmonyPatches;

namespace PlanningTool
{
    public class PlanningToolUserMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary(true);
            new PLocalization().Initialize(harmony);
            var pActionManager = new PActionManager();
            ToolKeyBindings.SetupPActionsOnLoad(pActionManager);

            AddStrings();
        }

        private static void AddStrings()
        {
            // KeyBinding strings
            ToolKeyBindings.AddStrings();

            // see: ToolParameterMenu.PopulateMenu
            Strings.Add("STRINGS.UI.TOOLS.FILTERLAYERS." + CancelTool_Patch.PLANNINGTOOL_PLAN,
                PTStrings.PLANNING_TOOL_FILTER_ITEM);
        }
    }
}
