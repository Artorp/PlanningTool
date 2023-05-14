using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace PlanningTool.HarmonyPatches
{
    [HarmonyPatch(typeof(ToolMenu), "CreateBasicTools")]
    public class ToolMenuCreateBasicTools
    {
        public static void Postfix(ToolMenu __instance)
        {
            __instance.basicTools.Insert(0,
                ToolMenu.CreateToolCollection(PTStrings.PLANNING_TOOL_NAME, "icon_action_dig",
                    PlanningToolUserMod.PlanningToolAction, nameof(PlanningToolInterface),
                    string.Format(PTStrings.PLANNING_TOOL_TOOLTIP, "{Hotkey}"), true));
        }
    }

    [HarmonyPatch(typeof(Game), "DestroyInstances")]
    public class GameDestroyInstances
    {
        public static void Postfix()
        {
            PlanningToolInterface.DestroyInstance();
        }
    }

    [HarmonyPatch(typeof(PlayerController), "OnPrefabInit")]
    public class PlayerControllerOnPrefabInitPatch
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
    public class ToolMenuOnPrefabInit
    {
        public static void Postfix()
        {
            // set up submenu
            var subMenu = new PlanningSubMenu();
            subMenu.CreateSubmenuTools();
            subMenu.InstantiateCollectionsUI();
            var methodBuildRowToggles = AccessTools.Method(typeof(ToolMenu), "BuildRowToggles");
            if (methodBuildRowToggles == null)
            {
                Debug.LogWarning("ToolMenu.BuildRowToggles method not found!");
                return;
            }
            var methodBuildToolToggles = AccessTools.Method(typeof(ToolMenu), "BuildToolToggles");
            if (methodBuildToolToggles == null)
            {
                Debug.LogWarning("ToolMenu.BuildToolToggles method not found!");
                return;
            }

            methodBuildRowToggles.Invoke(ToolMenu.Instance, new object[] { subMenu.planTools });
            methodBuildToolToggles.Invoke(ToolMenu.Instance, new object[] { subMenu.planTools });
        }
    }

}
