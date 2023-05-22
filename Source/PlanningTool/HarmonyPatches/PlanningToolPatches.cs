using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace PlanningTool.HarmonyPatches
{
    [HarmonyPatch(typeof(SaveGame), "OnPrefabInit")]
    public class SaveGame_OnPrefabInit_Patch
    {
        public static void Postfix(SaveGame __instance)
        {
            __instance.gameObject.AddOrGet<SaveLoadPlans>();
        }
    }

    [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
    public class Db_Initialize_Patch
    {
        public static void Postfix()
        {
            PTAssets.Initialize();
            PTObjectTemplates.CreateTemplates();
            ToolKeyBindings.SetActionsIgnoreConflicts();
        }
    }

    [HarmonyPatch(typeof(ToolMenu), "CreateBasicTools")]
    public class ToolMenu_CreateBasicTools_Patch
    {
        public static void Postfix(ToolMenu __instance)
        {
            __instance.basicTools.Insert(0,
                ToolMenu.CreateToolCollection(PTStrings.PLANNING_TOOL_NAME, PTAssets.IconToolPlanning.name,
                    ToolKeyBindings.PlanningToolAction.GetKAction(), nameof(PlanningToolInterface),
                    string.Format(PTStrings.PLANNING_TOOL_TOOLTIP, "{Hotkey}"), true));
        }
    }

    [HarmonyPatch(typeof(GridSettings), nameof(GridSettings.Reset))]
    public class GridSettings_Reset_Patch
    {
        public static void Postfix()
        {
            PlanGrid.Initialize();
        }
    }

    [HarmonyPatch(typeof(Game), "DestroyInstances")]
    public class Game_DestroyInstances_Patch
    {
        public static void Postfix()
        {
            PlanningToolInterface.DestroyInstance();
            PlanningToolBrushMenu.DestroyInstance();
            PlanningToolSettings.DestroyInstance();
            PlanGrid.Clear();
            SaveLoadPlans.DestroyInstance();
        }
    }

    [HarmonyPatch(typeof(PlayerController), "OnPrefabInit")]
    public class PlayerController_OnPrefabInit_Patch
    {
        public static void Postfix(PlayerController __instance)
        {
            var tools = new List<InterfaceTool>(__instance.tools);
            var createPlanTool = new GameObject(nameof(PlanningToolInterface), typeof(PlanningToolInterface));
            createPlanTool.transform.SetParent(__instance.gameObject.transform);
            createPlanTool.SetActive(true);
            createPlanTool.SetActive(false);
            tools.Add(createPlanTool.GetComponent<InterfaceTool>());
            __instance.tools = tools.ToArray();
        }
    }

    [HarmonyPatch(typeof(ToolMenu), "OnPrefabInit")]
    public class ToolMenu_OnPrefabInit_Patch
    {
        public static void Postfix()
        {
            // set up planning tool submenu
            var screen = ToolMenu.Instance.gameObject.AddComponent<PlanningToolBrushMenu>();
            screen.Activate();
        }
    }

    [HarmonyPatch(typeof(CancelTool), "OnDragTool")]
    public class CancelTool_Patch
    {
        public static string PLANNINGTOOL_PLAN = nameof(PLANNINGTOOL_PLAN);

        [HarmonyPatch("GetDefaultFilters")]
        [HarmonyPostfix]
        public static void GetDefaultFiltersPatch(Dictionary<string, ToolParameterMenu.ToggleState> filters)
        {
            filters.Add(PLANNINGTOOL_PLAN, ToolParameterMenu.ToggleState.Off);
        }

        [HarmonyPatch("OnDragTool")]
        [HarmonyPostfix]
        public static void OnDragToolPatch(int cell, CancelTool __instance)
        {
            var go = PlanGrid.Plans[cell];
            if (go != null && __instance.IsActiveLayer(PLANNINGTOOL_PLAN))
            {
                PlanGrid.Plans[cell] = null;
                SaveLoadPlans.Instance.PlanState.Remove(cell);
                Object.Destroy(go);
            }
        }
    }

}
