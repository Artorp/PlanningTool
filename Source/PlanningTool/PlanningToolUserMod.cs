using System;
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
        public static Action PlanningToolAction  { get; private set; }

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary(true);
            new PLocalization().Initialize(harmony);
            var pActionManager = new PActionManager();
            var planningToolPAction = pActionManager.CreateAction("planningtool.use_planning_tool", PTStrings.ACTION_PLANNING_TOOL_NAME);
            PlanningToolAction = planningToolPAction.GetKAction();

            // see: ToolParameterMenu.PopulateMenu
            Strings.Add("STRINGS.UI.TOOLS.FILTERLAYERS." + CancelTool_Patch.PLANNINGTOOL_PLAN, PTStrings.PLANNING_TOOL_FILTER_ITEM);
        }
    }
}
