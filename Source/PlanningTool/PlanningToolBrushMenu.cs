using System;
using System.Collections.Generic;
using PeterHan.PLib.UI;
using UnityEngine;
using UnityEngine.UI;

namespace PlanningTool
{
    public class PlanningToolBrushMenu : KScreen
    {
        public static PlanningToolBrushMenu Instance { get; private set; }

        public PlanningToolSettings Settings;

        public override float GetSortKey() => PlanningToolInterface.Instance.ToolActive ? 50f : base.GetSortKey();

        public GameObject row;
        private List<PlanColor> _planColors;

        private GameObject _horizMisc;

        public static void DestroyInstance() => Instance = null;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Instance = this;

            Settings = new PlanningToolSettings();

            var ppanel = new PPanel("PlanningToolBrushMenuPanel");

            ppanel.Direction = PanelDirection.Vertical;
            ppanel.Alignment = TextAnchor.LowerRight;
            ppanel.Spacing = 15;

            var horizMisc = new PPanel("HorizontalMisc");
            horizMisc.Direction = PanelDirection.Horizontal;
            horizMisc.Alignment = TextAnchor.LowerRight;
            horizMisc.OnRealize += realized => _horizMisc = realized;
            ppanel.AddChild(horizMisc);

            var sampleButton = PTObjectTemplates.CreateSquareButton("Sample", PTAssets.WhiteBGSprite, null);
            var sampleButtonKToggle = sampleButton.GetComponent<KToggle>();
            sampleButtonKToggle.onClick += () =>
            {
                if (sampleButtonKToggle.isOn && Settings.PlanningMode != PlanningToolSettings.PlanningToolMode.SamplePlan)
                {
                    Settings.PlanningMode = PlanningToolSettings.PlanningToolMode.SamplePlan;
                } else if (!sampleButtonKToggle.isOn &&
                           Settings.PlanningMode != PlanningToolSettings.PlanningToolMode.DragPlan)
                {
                    Settings.PlanningMode = PlanningToolSettings.PlanningToolMode.DragPlan;
                }
            };
            Settings.OnPlanningToolModeChanged += mode =>
            {
                var isSampling = mode == PlanningToolSettings.PlanningToolMode.SamplePlan;
                if (sampleButtonKToggle.isOn != isSampling)
                    sampleButtonKToggle.isOn = isSampling;
            };
            horizMisc.AddChild(new UIComponentWrapper(sampleButton));

            var pasteButton = PTObjectTemplates.CreateSquareButton("Paste", PTAssets.WhiteBGSprite, null);
            var pasteButtonKToggle = pasteButton.GetComponent<KToggle>();
            pasteButtonKToggle.onClick += () =>
            {
                if (pasteButtonKToggle.isOn && Settings.PlanningMode != PlanningToolSettings.PlanningToolMode.PlaceClipboard)
                {
                    Settings.PlanningMode = PlanningToolSettings.PlanningToolMode.PlaceClipboard;
                } else if (!pasteButtonKToggle.isOn &&
                           Settings.PlanningMode != PlanningToolSettings.PlanningToolMode.DragPlan)
                {
                    Settings.PlanningMode = PlanningToolSettings.PlanningToolMode.DragPlan;
                }
            };
            Settings.OnPlanningToolModeChanged += mode =>
            {
                var isPasting = mode == PlanningToolSettings.PlanningToolMode.PlaceClipboard;
                if (pasteButtonKToggle.isOn != isPasting)
                    pasteButtonKToggle.isOn = isPasting;
            };
            horizMisc.AddChild(new UIComponentWrapper(pasteButton));

            var cutButton = PTObjectTemplates.CreateSquareButton("Cut", PTAssets.WhiteBGSprite, null);
            var cutButtonKToggle = cutButton.GetComponent<KToggle>();
            cutButtonKToggle.onClick += () =>
            {
                if (cutButtonKToggle.isOn && Settings.PlanningMode != PlanningToolSettings.PlanningToolMode.CutArea)
                {
                    Settings.PlanningMode = PlanningToolSettings.PlanningToolMode.CutArea;
                } else if (!cutButtonKToggle.isOn &&
                           Settings.PlanningMode != PlanningToolSettings.PlanningToolMode.DragPlan)
                {
                    Settings.PlanningMode = PlanningToolSettings.PlanningToolMode.DragPlan;
                }
            };
            Settings.OnPlanningToolModeChanged += mode =>
            {
                var isCopying = mode == PlanningToolSettings.PlanningToolMode.CutArea;
                if (cutButtonKToggle.isOn != isCopying)
                    cutButtonKToggle.isOn = isCopying;
            };
            horizMisc.AddChild(new UIComponentWrapper(cutButton));

            var copyButton = PTObjectTemplates.CreateSquareButton("Copy", PTAssets.WhiteBGSprite, null);
            var copyButtonKToggle = copyButton.GetComponent<KToggle>();
            copyButtonKToggle.onClick += () =>
            {
                if (copyButtonKToggle.isOn && Settings.PlanningMode != PlanningToolSettings.PlanningToolMode.CopyArea)
                {
                    Settings.PlanningMode = PlanningToolSettings.PlanningToolMode.CopyArea;
                } else if (!copyButtonKToggle.isOn &&
                           Settings.PlanningMode != PlanningToolSettings.PlanningToolMode.DragPlan)
                {
                    Settings.PlanningMode = PlanningToolSettings.PlanningToolMode.DragPlan;
                }
            };
            Settings.OnPlanningToolModeChanged += mode =>
            {
                var isCopying = mode == PlanningToolSettings.PlanningToolMode.CopyArea;
                if (copyButtonKToggle.isOn != isCopying)
                    copyButtonKToggle.isOn = isCopying;
            };
            horizMisc.AddChild(new UIComponentWrapper(copyButton));

            var hideButton = PTObjectTemplates.CreateSquareButton("Hide / show plans", PTAssets.WhiteBGSprite, null);
            hideButton.GetComponent<KToggle>().onValueChanged += b =>
            {
                SaveLoadPlans.Instance.HidePlans = b;
            };
            SaveLoadPlans.Instance.OnHidePlansChanged += b =>
            {
                foreach (var plan in PlanGrid.PlansDict.Values)
                {
                    plan.SetActive(!b);
                }
            };
            horizMisc.AddChild(new UIComponentWrapper(hideButton));

            var maxSliderValue = 20f;
            var alphaSlider = new PSliderSingle("ActiveAlphaSlider")
            {
                InitialValue = SaveLoadPlans.Instance.ActiveAlpha * maxSliderValue,
                IntegersOnly = true,
                MaxValue = maxSliderValue,
                OnValueChanged = (source, value) =>
                {
                    var valueScaled = value / maxSliderValue;
                    SaveLoadPlans.Instance.ActiveAlpha = valueScaled;
                }
            };
            SaveLoadPlans.Instance.OnActiveAlphaChange += f =>
            {
                foreach (var plan in PlanGrid.PlansDict.Values)
                {
                    var meshRenderer = plan.transform.Find("Mask").GetComponent<MeshRenderer>();
                    var material = meshRenderer.material;
                    var color = material.color;
                    color.a = f;
                    material.color = color;
                }
            };
            horizMisc.AddChild(alphaSlider);

            row = ppanel.Build();
            row.transform.SetParent(transform, false);
            row.transform.SetSiblingIndex(row.transform.GetSiblingIndex() - 2); // move to just below the priority screen (todo: find priority screen by name, then move it)

            // try to add buttons like sandbox tools

            var toggleParent = new PPanel("ColorShapeGroup")
            {
                Direction = PanelDirection.Horizontal
            }.Build();
            toggleParent.transform.SetParent(_horizMisc.transform, false);
            var tg = toggleParent.AddComponent<ToggleGroup>();
            tg.allowSwitchOff = false;

            var planShapes = new List<PlanShape>() { PlanShape.Rectangle, PlanShape.Circle, PlanShape.Diamond };
            var planSprites = new List<Sprite>()
                { PTAssets.RectangleSprite, PTAssets.CircleSprite, PTAssets.DiamondSprite };
            for (int i = 0; i < planShapes.Count; i++)
            {
                var planShape = planShapes[i];

                var shapeButton = PTObjectTemplates.CreateSquareButton(Enum.GetName(typeof(PlanShape), planShape), planSprites[i], toggleParent);
                var image = shapeButton.transform.Find("FG")?.GetComponent<Image>();
                if (image)
                    image.color = PlanColor.Gray.AsColor();

                var kToggle = shapeButton.GetComponent<KToggle>();
                if (i == 0)
                    kToggle.isOn = true;
                kToggle.group = tg;
                kToggle.onValueChanged += b =>
                {
                    if (b)
                        Settings.ActiveShape = planShape;
                };
            }

            toggleParent = new PPanel("ColorToggleGroup")
            {
                Direction = PanelDirection.Horizontal
            }.Build();
            toggleParent.transform.SetParent(row.transform, false);
            tg = toggleParent.AddComponent<ToggleGroup>();
            tg.allowSwitchOff = false;

            _planColors = new List<PlanColor>()
            {
                PlanColor.Gray, PlanColor.Blue, PlanColor.Green, PlanColor.Red, PlanColor.Cyan, PlanColor.Magenta,
                PlanColor.Violet, PlanColor.Orange, PlanColor.Yellow, PlanColor.White, PlanColor.Black
            };
            for (int i = 0; i < _planColors.Count; i++)
            {
                var planColor = _planColors[i];

                var colorButton = PTObjectTemplates.CreateSquareButton(Enum.GetName(typeof(PlanColor), planColor), PTAssets.WhiteBGSprite, toggleParent);
                var image = colorButton.transform.Find("FG")?.GetComponent<Image>();
                if (image)
                {
                    image.color = planColor.AsColor();
                }

                var kToggle = colorButton.GetComponent<KToggle>();
                if (i == 0)
                    kToggle.isOn = true;
                kToggle.group = tg;
                kToggle.onValueChanged += b =>
                {
                    if (b)
                        Settings.ActiveColor = planColor;
                };
            }

            tg.EnsureValidState();

            row.SetActive(false);
        }

        public override void OnKeyDown(KButtonEvent e)
        {
            if (!e.Consumed && PlanningToolInterface.Instance.ToolActive &&
                Settings.PlanningMode == PlanningToolSettings.PlanningToolMode.PlaceClipboard)
            {
                var action = e.GetAction();
                var keyCode = e.Controller.GetInputForAction(action);
                if (keyCode == KKeyCode.Q)
                {
                    e.TryConsume(action);
                    PlanningToolInterface.Instance.Clipboard.Rotate(false);
                    PlanningToolInterface.Instance.RefreshClipboardVisualisationPreview();
                } else if (keyCode == KKeyCode.E)
                {
                    e.TryConsume(action);
                    PlanningToolInterface.Instance.Clipboard.Rotate(true);
                    PlanningToolInterface.Instance.RefreshClipboardVisualisationPreview();
                } else if (keyCode == KKeyCode.F)
                {
                    e.TryConsume(action);
                    PlanningToolInterface.Instance.Clipboard.Flip(true);
                    PlanningToolInterface.Instance.RefreshClipboardVisualisationPreview();
                }
            }
            base.OnKeyDown(e);
        }

        public override void OnKeyUp(KButtonEvent e)
        {
            if (!e.Consumed && PlanningToolInterface.Instance.ToolActive &&
                (Settings.PlanningMode == PlanningToolSettings.PlanningToolMode.PlaceClipboard ||
                 Settings.PlanningMode == PlanningToolSettings.PlanningToolMode.CopyArea ||
                 Settings.PlanningMode == PlanningToolSettings.PlanningToolMode.CutArea ||
                 Settings.PlanningMode == PlanningToolSettings.PlanningToolMode.SamplePlan))
            {
                var action = e.GetAction();
                var keyCode = e.Controller.GetInputForAction(action);
                if (keyCode == KKeyCode.Mouse1)
                {
                    // cancel while placing clipboard or other submenu tools should just go back to drag mode
                    PlaySound(GlobalAssets.GetSound("Tile_Cancel"));
                    Settings.PlanningMode = PlanningToolSettings.PlanningToolMode.DragPlan;
                    if (PlanningToolInterface.Instance.Dragging)
                        PlanningToolInterface.Instance.CancelDragging();
                    e.TryConsume(action);
                }
            }
            base.OnKeyUp(e);
        }
    }
}
