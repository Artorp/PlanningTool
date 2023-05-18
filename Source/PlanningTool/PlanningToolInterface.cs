using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace PlanningTool
{
    public class PlanningToolInterface : DragTool
    {
        public static PlanningToolInterface Instance;

        private bool isInitialized;

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
            // change visualizer to show current selected config
            var mr = visualizer.transform.Find("Mask").GetComponent<MeshRenderer>();
            mr.material = PTAssets.SelectionOutlineMaterial;
            var visualizerPlan = PTObjectTemplates.CreatePlanningTileMesh("PlanPreview", new SaveLoadPlans.PlanData());
            var visualizerMask = visualizerPlan.transform.Find("Mask");
            var vmPos = visualizerMask.position;
            vmPos.z -= 0.3f;
            visualizerMask.position = vmPos;
            visualizerPlan.transform.SetParent(visualizer.transform, false);
            visualizerPlan.SetActive(true);

            visualizerLayer = Grid.SceneLayer.SceneMAX;

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

        public void RefreshVisualizerPreview()
        {
            var visualizerPlanPreviewMaskMeshRenderer =
                visualizer.transform.Find("PlanPreview").Find("Mask").GetComponent<MeshRenderer>();
            var activeShape = PlanningToolSettings.Instance.ActiveShape;
            if (activeShape == PlanShape.Rectangle)
                visualizerPlanPreviewMaskMeshRenderer.material = PTAssets.RectangleMaterial;
            else if (activeShape == PlanShape.Circle)
                visualizerPlanPreviewMaskMeshRenderer.material = PTAssets.CircleMaterial;
            else if (activeShape == PlanShape.Diamond)
                visualizerPlanPreviewMaskMeshRenderer.material = PTAssets.DiamondMaterial;
            var col = PlanningToolSettings.Instance.ActiveColor.AsColor();
            col.a = 0.4f;
            visualizerPlanPreviewMaskMeshRenderer.material.color = col;
        }

        protected override void OnDragTool(int cell, int distFromOrigin)
        {
            var planData = new SaveLoadPlans.PlanData
            {
                Cell = cell,
                Color = PlanningToolSettings.Instance.ActiveColor,
                Shape = PlanningToolSettings.Instance.ActiveShape
            };

            var cellOccupied = SaveLoadPlans.Instance.PlanState.TryGetValue(cell, out var existingData);
            if (cellOccupied)
            {
                // compare with existing, if it is identical, no need to add gameobject
                if (planData.IsEquivalentTo(existingData))
                {
                    return;
                }
            }

            var go = CreatePlanTile(planData);

            if (cellOccupied)
            {
                var existingGameObject = PlanGrid.Plans[cell];
                existingGameObject.SetActive(false);
                Destroy(existingGameObject);
            }

            SaveLoadPlans.Instance.PlanState[cell] = planData;
            PlanGrid.Plans[cell] = go;
        }

        public static GameObject CreatePlanTile(SaveLoadPlans.PlanData planData)
        {
            var go = PTObjectTemplates.CreatePlanningTileMesh("PlanOverlay", planData);
            var pos = Grid.CellToPosCBC(planData.Cell, Grid.SceneLayer.TileFront);
            pos.z -= 0.1f;
            go.transform.localPosition = pos;
            go.SetActive(true);
            return go;
        }

        protected override void OnActivateTool()
        {
            base.OnActivateTool();
            if (!isInitialized)
            {
                PlanningToolSettings.Instance.OnActiveColorChange += color => RefreshVisualizerPreview();
                PlanningToolSettings.Instance.OnActiveShapeChange += shape => RefreshVisualizerPreview();
                isInitialized = true;
            }
        }

        protected override void OnDeactivateTool(InterfaceTool new_tool)
        {
            base.OnDeactivateTool(new_tool);
        }
    }
}
