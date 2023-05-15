using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace PlanningTool
{
    public class PlanningToolInterface : DragTool
    {
        public static PlanningToolInterface Instance;

        public static void DestroyInstance() => Instance = null;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Instance = this;

            // populate all fields tagged with [SerializeField] (or public) in DragTool that is probably
            // set through the Unity inspector, using values from DigTool

            // TODO:
            // show grid effect when placing (like when placing building)
            // show the plan to be placed when in tool menu

            FieldInfo areaVisualizerField = AccessTools.Field(typeof(DragTool), "areaVisualizer");

            visualizer = Util.KInstantiate(DigTool.Instance.visualizer);
            visualizerLayer = Grid.SceneLayer.Background;
            var avOriginal = areaVisualizerField.GetValue(DigTool.Instance) as GameObject;
            var av = Util.KInstantiate(avOriginal, gameObject);
            av.SetActive(false);
            areaVisualizerField.SetValue(this, av);
            areaVisualizerSpriteRenderer = av.GetComponent<SpriteRenderer>();
            av.transform.SetParent(transform);
            FieldInfo areaColourField = AccessTools.Field(typeof(DragTool), "areaColour");
            var areaColor = (Color32)areaColourField.GetValue(DigTool.Instance);
            av.GetComponent<Renderer>().material.color = areaColor;

            var boxCursorField = AccessTools.Field(typeof(DragTool), "boxCursor");
            boxCursorField.SetValue(this, boxCursorField.GetValue(DigTool.Instance));
            var areaVisualizerTextPrefabField = AccessTools.Field(typeof(DragTool), "areaVisualizerTextPrefab");
            areaVisualizerTextPrefabField.SetValue(this, areaVisualizerTextPrefabField.GetValue(DigTool.Instance));

            var pthc = gameObject.AddComponent<PlanningToolHoverCard>();
            pthc.ToolName = "Planning";
            pthc.ActionName = "Set plan";
        }

        protected override void OnDragTool(int cell, int distFromOrigin)
        {
            if (PlanGrid.Plans[cell] != null)
            {
                // TODO: allow overwrite with different plan type (for now, only gray square)
                return;
            }
            var go = CreatePlanTile(cell);

            SaveLoadPlans.Instance.PlanState[cell] = new SaveLoadPlans.PlanData
            {
                Cell = cell
            };
            PlanGrid.Plans[cell] = go;
        }

        public static GameObject CreatePlanTile(int cell)
        {
            var go = PTObjectTemplates.CreatePlanningTileMesh("PlanOverlay");
            var pos = Grid.CellToPosCBC(cell, Grid.SceneLayer.TileFront);
            pos.z -= 0.1f;
            go.transform.localPosition = pos;
            go.SetActive(true);
            return go;
        }

        protected override void OnActivateTool()
        {
            base.OnActivateTool();
        }

        protected override void OnDeactivateTool(InterfaceTool new_tool)
        {
            base.OnDeactivateTool(new_tool);
        }
    }
}
