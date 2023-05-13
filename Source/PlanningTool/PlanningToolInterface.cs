using System.Collections.Generic;
using HarmonyLib;
using PeterHan.PLib.UI;
using UnityEngine;

namespace PlanningTool
{
    public class PlanningToolInterface : InterfaceTool
    {
        protected override void OnActivateTool()
        {
            base.OnActivateTool();
            Debug.Log("PlanningToolInterface.OnActivateTool()");
            Debug.Log("Getting some debug info about SandboxToolParameterMenu.instance...");
        }

        protected override void OnDeactivateTool(InterfaceTool new_tool)
        {
            base.OnDeactivateTool(new_tool);
            Debug.Log("PlanningToolInterface.OnDeactivateTool()");
        }

        public override void OnLeftClickUp(Vector3 cursor_pos)
        {
            base.OnLeftClickUp(cursor_pos);
            var cell = Grid.PosToCell(cursor_pos);
            if (!Grid.IsValidCell(cell) || !Grid.IsVisible(cell) || Grid.WorldIdx[cell] != ClusterManager.Instance.activeWorldId)
                return;
            // for now, just list at what is at the position:
            Debug.Log("[PlanningTool] found:");
            var foundObjects = new List<GameObject>();
            for (int layer = 0; layer < 44; ++layer)
            {
                if (Grid.Objects[cell, layer] != null)
                {
                    foundObjects.Add(Grid.Objects[cell, layer]);
                }
            }
            Debug.Log($"{foundObjects.Join()}");
        }
    }
}
