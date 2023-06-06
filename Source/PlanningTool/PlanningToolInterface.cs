using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using PlanningTool.HarmonyPatches;
using UnityEngine;

namespace PlanningTool
{
    public class PlanningToolInterface : DragTool
    {
        public static PlanningToolInterface Instance;

        private bool isInitialized;

        protected Texture2D defaultCursor;
        protected Vector2 defaultCursorOffset;

        public PlanClipboard Clipboard;

        private GameObject _visualizerPlan;
        private GameObject _visualizerClipboard;
        private List<GameObject> _visualizerClipboardObjects = new List<GameObject>();

        public GameObject _clipboardAreaVisualizer;

        private bool _toolActive;
        /// <summary>
        /// Due to DragTool not being intended to be using Brush mode, changing mode can cause multiple drag events
        /// to happen for the same action. Brush OnDragTool is in OnLeftClickDown while Box is in OnLeftClickUp,
        /// causing a single click of brush that changes mode to box, to also trigger the box tool's OnDragTool
        /// as well as OnDragComplete.
        /// Workaround: just flag this bool if changing mode while handling an click event.
        /// </summary>
        private bool _skipFurtherEventsThisClick;

        private int _cellLastDragThisClick;
        private int _prevMouseMoveCell;

        public bool ToolActive
        {
            get => _toolActive;
            set
            {
                if (_toolActive != value)
                {
                    _toolActive = value;
                    OnToolActive.Signal(value);
                }
            }
        }

        protected override string GetConfirmSound()
        {
            if (PlanningToolSettings.Instance.PlanningMode == PlanningToolSettings.PlanningToolMode.RemovePlan)
                return "Tile_Confirm_NegativeTool";
            return base.GetConfirmSound();
        }

        public event Action<bool> OnToolActive;

        public static void DestroyInstance() => Instance = null;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Instance = this;

            // populate all fields tagged with [SerializeField] (or public) in DragTool that is probably
            // set through the Unity inspector, using values from DigTool

            FieldInfo areaVisualizerField = AccessTools.Field(typeof(DragTool), "areaVisualizer");

            visualizer = Util.KInstantiate(DigTool.Instance.visualizer);
            // change visualizer to show current selected config
            var mr = visualizer.transform.Find("Mask").GetComponent<MeshRenderer>();
            mr.material = PTAssets.SelectionOutlineMaterial;
            _visualizerPlan = PTObjectTemplates.CreatePlanningTileMesh("PlanPreview", PlanShape.Rectangle, PlanColor.Gray);
            var visualizerMask = _visualizerPlan.transform.Find("Mask");
            var vmPos = visualizerMask.position;
            vmPos.z -= 0.3f;
            visualizerMask.position = vmPos;
            _visualizerPlan.transform.SetParent(visualizer.transform, false);
            _visualizerPlan.SetActive(true);

            _visualizerClipboard = new GameObject("VisualizerClipboard");
            _visualizerClipboard.transform.SetParent(visualizer.transform, false);
            _visualizerClipboard.SetActive(false);

            visualizerLayer = Grid.SceneLayer.SceneMAX;

            var avOriginal = areaVisualizerField.GetValue(DigTool.Instance) as GameObject;
            var av = Util.KInstantiate(avOriginal, gameObject);
            av.SetActive(false);
            areaVisualizerField.SetValue(this, av);
            areaVisualizerSpriteRenderer = av.GetComponent<SpriteRenderer>();
            av.transform.SetParent(transform);
            av.GetComponent<Renderer>().material.color = new Color(0.231f, 0.525f, 0.984f);

            // clipboard area visualiser
            _clipboardAreaVisualizer = Util.KInstantiate(avOriginal, _visualizerClipboard);
            _clipboardAreaVisualizer.SetActive(true);
            var cavPos = _clipboardAreaVisualizer.transform.localPosition;
            cavPos.y += Grid.HalfCellSizeInMeters;
            _clipboardAreaVisualizer.transform.localPosition = cavPos;
            _clipboardAreaVisualizer.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

            defaultCursor = PTAssets.CursorPlanning;
            defaultCursorOffset = new Vector2(2f, 2f);
            SetAllCursors(defaultCursor, defaultCursorOffset);
            var areaVisualizerTextPrefabField = AccessTools.Field(typeof(DragTool), "areaVisualizerTextPrefab");
            areaVisualizerTextPrefabField.SetValue(this, areaVisualizerTextPrefabField.GetValue(DigTool.Instance));

            var pthc = gameObject.AddComponent<PlanningToolHoverCard>();
        }

        private void SetAllCursors(Texture2D newCursor, Vector2 newOffset)
        {
            cursor = newCursor;
            cursorOffset = newOffset;
            var boxCursorField = AccessTools.Field(typeof(DragTool), "boxCursor");
            boxCursorField.SetValue(this, newCursor);
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

        public void RefreshClipboardVisualisationPreview()
        {
            if (_visualizerClipboard == null) return;
            foreach (var go in _visualizerClipboardObjects)
            {
                go.SetActive(false);
                Destroy(go);
            }
            _visualizerClipboardObjects.Clear();

            if (!Clipboard.HasData()) return;

            foreach (var element in Clipboard.Elements())
            {
                var go = PTObjectTemplates.CreatePlanningTileMesh("ClipboardVisualisationPreview", element.Shape, element.Color, false);
                var meshRenderer = go.transform.Find("Mask").GetComponent<MeshRenderer>();
                var material = meshRenderer.material;
                var col = material.color;
                col.a = 0.6f;
                material.color = col;
                var pos = new Vector3(element.OffsetX, element.OffsetY, -0.1f);
                go.transform.localPosition = pos;
                go.transform.SetParent(_visualizerClipboard.transform, false);
                _visualizerClipboardObjects.Add(go);
                go.SetActive(true);
            }

            if (_clipboardAreaVisualizer != null)
            {
                Vector2Int bottomLeft = new Vector2Int(int.MaxValue, int.MaxValue);
                Vector2Int topRight = new Vector2Int(int.MinValue, int.MinValue);
                foreach (var element in Clipboard.Elements())
                {
                    var elementPos = new Vector2Int(element.OffsetX, element.OffsetY);
                    bottomLeft = Vector2Int.Min(bottomLeft, elementPos);
                    topRight = Vector2Int.Max(topRight, elementPos);
                }
                // since parent transform is visualizer as CBC, adjust points to cover whole area
                Vector2 bottomLeftF = new Vector2(bottomLeft.x - Grid.HalfCellSizeInMeters, bottomLeft.y);
                Vector2 topRightF = new Vector2(topRight.x + Grid.HalfCellSizeInMeters, topRight.y + Grid.CellSizeInMeters);
                var spriteSize = topRightF - bottomLeftF;
                var spriteCenter = (bottomLeftF + topRightF) / 2f;
                _clipboardAreaVisualizer.transform.SetLocalPosition(spriteCenter);
                _clipboardAreaVisualizer.GetComponent<SpriteRenderer>().size = spriteSize;
            }

        }

        protected override void OnDragTool(int cell, int distFromOrigin)
        {
            if (_skipFurtherEventsThisClick)
                return;
            if (PlanningToolSettings.Instance.PlanningMode == PlanningToolSettings.PlanningToolMode.DragPlan)
                ToolPlacePlan(cell);
            else if (PlanningToolSettings.Instance.PlanningMode == PlanningToolSettings.PlanningToolMode.PlaceClipboard)
            {
                ToolPlaceClipboard(cell);
            }
            else if (PlanningToolSettings.Instance.PlanningMode == PlanningToolSettings.PlanningToolMode.SamplePlan)
            {
                ToolSamplePlan(cell);
            }
            else if (PlanningToolSettings.Instance.PlanningMode == PlanningToolSettings.PlanningToolMode.RemovePlan)
                ToolRemovePlan(cell);
            _cellLastDragThisClick = cell;
        }

        private void ToolRemovePlan(int cell)
        {
            var go = PlanGrid.Plans[cell];
            if (go != null)
            {
                PlanGrid.Plans[cell] = null;
                SaveLoadPlans.Instance.PlanState.Remove(cell);
                Destroy(go);
            }
        }

        private void ToolSamplePlan(int cell)
        {
            if (SaveLoadPlans.Instance.PlanState.TryGetValue(cell, out var planData))
            {
                PlanningToolSettings.Instance.ActiveShape = planData.Shape;
                PlanningToolSettings.Instance.ActiveColor = planData.Color;
            }
            PlanningToolSettings.Instance.PlanningMode = PlanningToolSettings.PlanningToolMode.DragPlan;
            _skipFurtherEventsThisClick = true;
        }

        private void ToolPlaceClipboard(int cell)
        {
            if (_cellLastDragThisClick == cell)
            {
                return;
            }

            Grid.CellToXY(cell, out var originX, out var originY);

            foreach (var element in Clipboard.Elements())
            {
                var x = originX + element.OffsetX;
                var y = originY + element.OffsetY;
                var elementCell = Grid.XYToCell(x, y);
                if (!Grid.IsValidCell(elementCell) ||
                    !Grid.IsVisible(elementCell))
                    continue;
                var planData = new SaveLoadPlans.PlanData
                {
                    Cell = elementCell,
                    Color = element.Color,
                    Shape = element.Shape
                };

                PlacePlan(elementCell, planData);
            }

            PlaySound(GlobalAssets.GetSound(GetConfirmSound()));
        }

        protected override void OnDragComplete(Vector3 cursorDown, Vector3 cursorUp)
        {
            if (_skipFurtherEventsThisClick)
            {
                return;
            }
            base.OnDragComplete(cursorDown, cursorUp);

            if (PlanningToolSettings.Instance.PlanningMode == PlanningToolSettings.PlanningToolMode.CopyArea || PlanningToolSettings.Instance.PlanningMode == PlanningToolSettings.PlanningToolMode.CutArea)
            {
                // copy all plans between start and stop to clipboard, use cursorUp as origin
                Grid.PosToXY(cursorUp, out var originX, out var originY);
                var startX = originX;
                var startY = originY;
                Grid.PosToXY(cursorDown, out var endX, out var endY);
                // loop over all values, start top left
                if (startX > endX)
                    Util.Swap(ref startX, ref endX);
                if (startY > endY)
                    Util.Swap(ref startY, ref endY);
                Clipboard.Clear();
                var isCutting = PlanningToolSettings.Instance.PlanningMode ==
                                PlanningToolSettings.PlanningToolMode.CutArea;
                for (int y = startY; y <= endY; y++)
                {
                    for (int x = startX; x <= endX; x++)
                    {
                        var cell = Grid.XYToCell(x, y);
                        if (!SaveLoadPlans.Instance.PlanState.TryGetValue(cell, out var existingData))
                            continue;
                        Clipboard.AddPlan(existingData, originX, originY);
                        if (isCutting)
                        {
                            SaveLoadPlans.Instance.PlanState.Remove(cell);
                            var go = PlanGrid.Plans[cell];
                            PlanGrid.Plans[cell] = null;
                            go.SetActive(false);
                            Destroy(go);
                        }
                    }
                }

                Clipboard.AdjustOffsetsToElements();

                RefreshClipboardVisualisationPreview();

                PlanningToolSettings.Instance.PlanningMode = PlanningToolSettings.PlanningToolMode.PlaceClipboard;
            }
        }

        private static void ToolPlacePlan(int cell)
        {
            var planData = new SaveLoadPlans.PlanData
            {
                Cell = cell,
                Color = PlanningToolSettings.Instance.ActiveColor,
                Shape = PlanningToolSettings.Instance.ActiveShape
            };

            PlacePlan(cell, planData);
        }

        private static void PlacePlan(int cell, SaveLoadPlans.PlanData planData)
        {
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
            var go = PTObjectTemplates.CreatePlanningTileMesh("PlanOverlay", planData.Shape, planData.Color);
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
                Clipboard = new PlanClipboard();
                PlanningToolSettings.Instance.OnPlanningToolModeChanged += toolMode =>
                {

                    if (toolMode != PlanningToolSettings.PlanningToolMode.DragPlan)
                        PlaySound(GlobalAssets.GetSound("hud_click"));

                    SetAllCursors(defaultCursor, defaultCursorOffset);
                    if (toolMode == PlanningToolSettings.PlanningToolMode.PlaceClipboard)
                    {
                        SetMode(Mode.Brush);
                        _visualizerClipboard.SetActive(true);
                    }
                    else if (toolMode == PlanningToolSettings.PlanningToolMode.SamplePlan)
                    {
                        SetAllCursors(PTAssets.CursorPipette, defaultCursorOffset);
                        SetMode(Mode.Brush);
                        _visualizerClipboard.SetActive(false);
                    }
                    else if (toolMode == PlanningToolSettings.PlanningToolMode.RemovePlan)
                    {
                        SetAllCursors(PTAssets.CursorEraser, defaultCursorOffset);
                        SetMode(Mode.Box);
                        _visualizerClipboard.SetActive(false);
                    }
                    else
                    {
                        SetMode(Mode.Box);
                        _visualizerClipboard.SetActive(false);
                    }

                    if (toolMode == PlanningToolSettings.PlanningToolMode.DragPlan)
                    {
                        _visualizerPlan.SetActive(true);
                    }
                    else
                    {
                        _visualizerPlan.SetActive(false);
                    }
                };
                isInitialized = true;
            }

            if (PlanningToolSettings.Instance.PlanningMode != PlanningToolSettings.PlanningToolMode.DragPlan)
                PlanningToolSettings.Instance.PlanningMode = PlanningToolSettings.PlanningToolMode.DragPlan;
            ToolActive = true;
            KScreenManager.Instance.RefreshStack();
            PlanningToolBrushMenu.Instance.row.SetActive(true);
            GridCompositor.Instance.ToggleMajor(true);
        }

        protected override void OnDeactivateTool(InterfaceTool new_tool)
        {
            base.OnDeactivateTool(new_tool);
            ToolActive = false;
            KScreenManager.Instance.RefreshStack();
            PlanningToolBrushMenu.Instance.row.SetActive(false);
            GridCompositor.Instance.ToggleMajor(false);
            AdjustCancelFilterIfNeeded(new_tool);
        }

        private static void AdjustCancelFilterIfNeeded(InterfaceTool new_tool)
        {
            if (new_tool == null) return;
            if (new_tool.name != CancelTool.Instance.name || !ModOptions.Options.SwitchPlanFilter) return;
            var overlayFilterTargetsField = AccessTools.Field(typeof(CancelTool), "overlayFilterTargets");
            if (overlayFilterTargetsField == null)
            {
                Debug.LogWarning("[PlanningTool] CancelTool.overlayFilterTargets not found.");
                return;
            }

            if (!(overlayFilterTargetsField.GetValue(CancelTool.Instance) is Dictionary<string, ToolParameterMenu.ToggleState> overlayFilterTargets))
            {
                Debug.LogWarning("[PlanningTool] CancelTool.filterTargets not able to be retrieved.");
                return;
            }

            if (overlayFilterTargets.Count == 0)
            {
                // shouldn't happen, as it is initialized in PlanningToolPatches CancelTool_Patch.OnPrefabInitPatch
                Debug.LogWarning("[PlanningTool] CancelTool.overlayFilterTargets is not yet initialized, won't change setting.");
                return;
            }

            if (!CancelTool.Instance.IsActiveLayer(CancelTool_Patch.PLANNINGTOOL_PLAN))
            {
                var targetKey = ModOptions.Options.AutoSwitchTo == ModOptions.AutoSwitchTarget.Plans
                    ? CancelTool_Patch.PLANNINGTOOL_PLAN
                    : ToolParameterMenu.FILTERLAYERS.ALL;
                var keys = new List<string>(overlayFilterTargets.Keys);
                foreach (var targetsKey in keys)
                {
                    overlayFilterTargets[targetsKey] = targetsKey == targetKey
                        ? ToolParameterMenu.ToggleState.On
                        : ToolParameterMenu.ToggleState.Off;
                }
            }

        }

        public override void OnLeftClickUp(Vector3 cursor_pos)
        {
            base.OnLeftClickUp(cursor_pos);
            _skipFurtherEventsThisClick = false;
            _cellLastDragThisClick = -1;
        }

        public override void OnMouseMove(Vector3 cursorPos)
        {
            base.OnMouseMove(cursorPos);
            var cell = Grid.PosToCell(cursorPos);

            if (cell != _prevMouseMoveCell && ToolActive && PlanningToolSettings.Instance.PlanningMode ==
                PlanningToolSettings.PlanningToolMode.SamplePlan)
            {
                RefreshSampleCursor(cell);
            }

            _prevMouseMoveCell = cell;
        }

        private void RefreshSampleCursor(int cell)
        {
            var hasPlan = PlanGrid.PlansDict.ContainsKey(cell);
            var nextCursor = hasPlan ? PTAssets.CursorPipette : PTAssets.CursorPipetteInvalid;
            if (cursor != nextCursor)
            {
                SetAllCursors(nextCursor, defaultCursorOffset);
                SetCursor(nextCursor, defaultCursorOffset, CursorMode.Auto);
            }
        }
    }
}
