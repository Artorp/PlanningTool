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

        public static void DestroyInstance() => Instance = null;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Instance = this;

            Settings = new PlanningToolSettings();

            row = new PPanel("PlanningToolBrushMenuPanel")
            {
                Direction = PanelDirection.Vertical,
                Alignment = TextAnchor.LowerRight,
                Spacing = 15
            }.Build();

            row.transform.SetParent(transform, false);
            row.transform.SetSiblingIndex(row.transform.GetSiblingIndex() - 2); // move to just below the priority screen (todo: find priority screen by name, then move it)

            // first row: visibility and transparency
            var visibilityRow = new PPanel("VisibilityRow")
            {
                Direction = PanelDirection.Horizontal,
                Alignment = TextAnchor.UpperRight,
                Spacing = 10
            }.Build();
            visibilityRow.transform.SetParent(row.transform, false);

            var hideButton = PTObjectTemplates.CreateSquareButton("Hide / show", PTAssets.IconToolHideShow, null);
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
                hideButton.transform.Find("FG").GetComponent<Image>().sprite = b ? PTAssets.IconToolHideShowHidden : PTAssets.IconToolHideShow;
            };
            hideButton.transform.SetParent(visibilityRow.transform, false);

            var sliderParent = new PPanel("SliderParent")
            {
                Direction = PanelDirection.Vertical,
                Spacing = 5,
                BackColor = new Color(0f, 0f, 0f, 0.5f),
                Margin = new RectOffset(4, 4, 4, 4)
            }.Build();
            sliderParent.transform.SetParent(visibilityRow.transform, false);

            var sliderText = new PLabel("sliderText")
            {
                Text = $"Transparency:\n{SaveLoadPlans.Instance.ActiveAlpha:0.00}"
            }.Build();
            sliderText.transform.SetParent(sliderParent.transform, false);

            var maxSliderValue = 20f;
            var alphaSlider = new PSliderSingle("ActiveAlphaSlider")
            {
                InitialValue = SaveLoadPlans.Instance.ActiveAlpha * maxSliderValue,
                IntegersOnly = true,
                MaxValue = maxSliderValue,
                MinValue = 1f,
                OnValueChanged = (source, value) =>
                {
                    var valueScaled = value / maxSliderValue;
                    sliderText.GetComponentInChildren<LocText>().text = $"Transparency:\n{valueScaled:0.00}";
                    SaveLoadPlans.Instance.ActiveAlpha = valueScaled;
                }
            }.Build();
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
            alphaSlider.transform.SetParent(sliderParent.transform, false);

            // second row, shapes and misc tools

            var miscToolsRow = new PPanel("HorizontalMisc")
            {
                Direction = PanelDirection.Horizontal,
                Alignment = TextAnchor.MiddleRight,
                Spacing = 5
            }.Build();
            miscToolsRow.transform.SetParent(row.transform, false);

            var shapeButtons = new PPanel("ShapeButtons")
            {
                Direction = PanelDirection.Horizontal,
                Margin = new RectOffset(0, 5, 0, 0)
            }.Build();
            shapeButtons.transform.SetParent(miscToolsRow.transform, false);

            var planShapes = new List<PlanShape>() { PlanShape.Rectangle, PlanShape.Circle, PlanShape.Diamond };
            var planSprites = new List<Sprite>()
                { PTAssets.RectangleSprite, PTAssets.CircleSprite, PTAssets.DiamondSprite };
            for (int i = 0; i < planShapes.Count; i++)
            {
                var planShape = planShapes[i];

                var shapeButton = PTObjectTemplates.CreateSquareButton(Enum.GetName(typeof(PlanShape), planShape), planSprites[i], shapeButtons);
                var image = shapeButton.transform.Find("FG")?.GetComponent<Image>();
                if (image)
                    image.color = PlanColor.Gray.AsColor();

                var kToggle = shapeButton.GetComponent<KToggle>();
                if (i == 0)
                    kToggle.isOn = true;
                kToggle.onClick += () =>
                {
                    if (kToggle.isOn && Settings.ActiveShape != planShape)
                        Settings.ActiveShape = planShape;
                    // don't allow the user to deselect this button
                    if (!kToggle.isOn)
                        kToggle.isOn = true;
                };
                Settings.OnActiveShapeChange += shape =>
                {
                    if (shape == planShape && !kToggle.isOn)
                        kToggle.isOn = true;
                    else if (shape != planShape && kToggle.isOn)
                        kToggle.isOn = false;
                };
            }

            var verticalBar = new PPanel("VerticalBar")
            {
                BackColor = new Color(0f, 0f, 0f, 0.7f)
            }.BuildWithFixedSize(new Vector2(3f, 42f));
            verticalBar.transform.SetParent(miscToolsRow.transform, false);

            var copyButton = PTObjectTemplates.CreateSquareButton("Copy", PTAssets.IconToolCopy, null);
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
            copyButton.transform.SetParent(miscToolsRow.transform, false);

            var cutButton = PTObjectTemplates.CreateSquareButton("Cut", PTAssets.IconToolCut, null);
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
            cutButton.transform.SetParent(miscToolsRow.transform, false);

            var pasteButton = PTObjectTemplates.CreateSquareButton("Paste", PTAssets.IconToolPaste, null);
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
            pasteButton.transform.SetParent(miscToolsRow.transform, false);

            var sampleButton = PTObjectTemplates.CreateSquareButton("Sample", Assets.GetSprite((HashedString) "sample"), null);
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
            sampleButton.transform.SetParent(miscToolsRow.transform, false);

            // third row, color buttons

            var colorButtons = new PPanel("ColorButtons")
            {
                Direction = PanelDirection.Horizontal
            }.Build();
            colorButtons.transform.SetParent(row.transform, false);

            _planColors = new List<PlanColor>()
            {
                PlanColor.Gray, PlanColor.Blue, PlanColor.Green, PlanColor.Red, PlanColor.Cyan, PlanColor.Magenta,
                PlanColor.Violet, PlanColor.Orange, PlanColor.Yellow, PlanColor.White, PlanColor.Black
            };
            for (int i = 0; i < _planColors.Count; i++)
            {
                var planColor = _planColors[i];

                var colorButton = PTObjectTemplates.CreateSquareButton(Enum.GetName(typeof(PlanColor), planColor), PTAssets.WhiteBGSprite, colorButtons);
                var image = colorButton.transform.Find("FG")?.GetComponent<Image>();
                if (image)
                {
                    image.color = planColor.AsColor();
                }

                var kToggle = colorButton.GetComponent<KToggle>();
                if (i == 0)
                    kToggle.isOn = true;
                kToggle.onClick += () =>
                {
                    if (kToggle.isOn && Settings.ActiveColor != planColor)
                        Settings.ActiveColor = planColor;
                    // don't allow the user to deselect this button
                    if (!kToggle.isOn)
                        kToggle.isOn = true;
                };
                Settings.OnActiveColorChange += color =>
                {
                    if (color == planColor && !kToggle.isOn)
                        kToggle.isOn = true;
                    else if (color != planColor && kToggle.isOn)
                        kToggle.isOn = false;
                };
            }

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
