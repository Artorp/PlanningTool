using System;
using System.Collections.Generic;
using HarmonyLib;
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
        public GameObject RadialMenuObject;
        public RadialMenu RadialMenu;
        private List<PlanColor> _planColors;
        private List<PlanShape> _planShapes;
        private List<Sprite> _planShapeSprites;

        public static void DestroyInstance() {
            if (Instance.RadialMenuObject != null)
            {
                Instance.RadialMenuObject.transform.SetParent(null);
                Instance.RadialMenuObject = null;
            }
            Instance = null;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Instance = this;

            Settings = new PlanningToolSettings();

            RadialMenuObject = new GameObject("RadialMenu");
            RadialMenu = RadialMenuObject.AddComponent<RadialMenu>();

            row = new PPanel("PlanningToolBrushMenuPanel")
            {
                Direction = PanelDirection.Vertical,
                Alignment = TextAnchor.LowerRight,
                Spacing = 15
            }.Build();

            row.transform.SetParent(transform, false);
            row.transform.SetSiblingIndex(row.transform.GetSiblingIndex() - 2); // move to just below the priority screen (todo: find priority screen by name, then move it)

            // tooltip header text style
            var TooltipHeaderField = AccessTools.Field(typeof(ToolMenu), "TooltipHeader");
            var toolmenuHeaderStyleSetting = TooltipHeaderField.GetValue(ToolMenu.Instance) as TextStyleSetting;
            var tooltipHeaderStyle = Instantiate(toolmenuHeaderStyleSetting);

            // first row: visibility and transparency
            var visibilityRow = new PPanel("VisibilityRow")
            {
                Direction = PanelDirection.Horizontal,
                Alignment = TextAnchor.UpperRight,
                Spacing = 10
            }.Build();
            visibilityRow.transform.SetParent(row.transform, false);

            var hideButton = PTObjectTemplates.CreateSquareButton("Hide / show", PTAssets.IconToolHideShow, null);
            var hideButtonToolTip = hideButton.GetComponent<ToolTip>();
            hideButtonToolTip.AddMultiStringTooltip("Hide / Show Plans", tooltipHeaderStyle);
            hideButtonToolTip.AddMultiStringTooltip("Toggle visibility of placed plans", null);
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
                },
                ToolTip = "Change the transparency of plans"
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

            _planShapes = new List<PlanShape>() { PlanShape.Rectangle, PlanShape.Circle, PlanShape.Diamond };
            _planShapeSprites = new List<Sprite>()
                { PTAssets.RectangleSprite, PTAssets.CircleSprite, PTAssets.DiamondSprite };
            for (int i = 0; i < _planShapes.Count; i++)
            {
                var planShape = _planShapes[i];

                var shapeButton = PTObjectTemplates.CreateSquareButton(Enum.GetName(typeof(PlanShape), planShape), _planShapeSprites[i], shapeButtons);
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
            var copyButtonToolTip = copyButton.GetComponent<ToolTip>();
            copyButtonToolTip.AddMultiStringTooltip("Copy", tooltipHeaderStyle);
            copyButtonToolTip.AddMultiStringTooltip(GameUtil.ReplaceHotkeyString("Copy plans {Hotkey}", ToolKeyBindings.CopyPlanAction.GetKAction()), null);
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
            var cutButtonToolTip = cutButton.GetComponent<ToolTip>();
            cutButtonToolTip.AddMultiStringTooltip("Cut", tooltipHeaderStyle);
            cutButtonToolTip.AddMultiStringTooltip(GameUtil.ReplaceHotkeyString("Cut plans {Hotkey}", ToolKeyBindings.CutPlanAction.GetKAction()), null);
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
            var pasteButtonToolTip = pasteButton.GetComponent<ToolTip>();
            pasteButtonToolTip.AddMultiStringTooltip("Paste", tooltipHeaderStyle);
            pasteButtonToolTip.AddMultiStringTooltip(GameUtil.ReplaceHotkeyString("Paste previously copied plans {Hotkey}", ToolKeyBindings.PastePlanAction.GetKAction()), null);
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
            var sampleButtonToolTip = sampleButton.GetComponent<ToolTip>();
            sampleButtonToolTip.AddMultiStringTooltip("Sample", tooltipHeaderStyle);
            sampleButtonToolTip.AddMultiStringTooltip(GameUtil.ReplaceHotkeyString("Copy shape and color from already placed plan {Hotkey}", ToolKeyBindings.SampleToolAction.GetKAction()), null);
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

            var exportButton = PTObjectTemplates.CreateSquareButton("Export", PTAssets.IconToolExport, null);
            var exportButtonToolTip = exportButton.GetComponent<ToolTip>();
            exportButtonToolTip.AddMultiStringTooltip("Export", tooltipHeaderStyle);
            exportButtonToolTip.AddMultiStringTooltip("Place the plan currently on the game's clipboard to your system's clipboard", null);
            var exportButtonKToggle = exportButton.GetComponent<KToggle>();
            exportButtonKToggle.onClick += () =>
            {
                StartCoroutine(TurnKToggleOff(exportButtonKToggle, 0.05f));
                PlanningToolInterface.Instance.Clipboard.ExportToSystemClipboard();
            };
            exportButton.transform.SetParent(miscToolsRow.transform, false);

            var importButton = PTObjectTemplates.CreateSquareButton("Import", PTAssets.IconToolImport, null);
            var importButtonToolTip = importButton.GetComponent<ToolTip>();
            importButtonToolTip.AddMultiStringTooltip("Export", tooltipHeaderStyle);
            importButtonToolTip.AddMultiStringTooltip(
                "Attempt to import a plan from the system's clipboard to the game's clipboard\n" +
                "Import errors are logged to the game's player.log file.", null);
            var importButtonKToggle = importButton.GetComponent<KToggle>();
            importButtonKToggle.onClick += () =>
            {
                StartCoroutine(TurnKToggleOff(importButtonKToggle, 0.05f));
                try
                {
                    PlanningToolInterface.Instance.Clipboard.ImportFromSystemClipboard();
                    PlanningToolInterface.Instance.RefreshClipboardVisualisationPreview();
                    if (PlanningToolInterface.Instance.Clipboard.HasData())
                        Settings.PlanningMode = PlanningToolSettings.PlanningToolMode.PlaceClipboard;
                }
                catch (DeserializationException e)
                {
                    // Invalid string was on the system clipboard
                    var warningMessage =
                        $"[PlanningTool] Encountered {e.Errors.Count} errors while attempting to parse string from clipboard:";
                    foreach (var error in e.Errors)
                    {
                        warningMessage += $"\n{error}";
                    }

                    warningMessage += "\nThe string that was attempted to be parsed was:\n" + e.StringToDeserialize;
                    Debug.LogWarning(warningMessage);
                    PlaySound(GlobalAssets.GetSound("redalert_on"));
                }
            };
            importButton.transform.SetParent(miscToolsRow.transform, false);

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

        private IEnumerator<WaitForSecondsRealtime> TurnKToggleOff(KToggle kToggle, float afterSecondsUnscaled)
        {
            yield return new WaitForSecondsRealtime(afterSecondsUnscaled);
            if (kToggle.isOn)
                kToggle.isOn = false;
        }

        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.Consumed)
                return;

            if (!e.Consumed && PlanningToolInterface.Instance.ToolActive &&
                Settings.PlanningMode == PlanningToolSettings.PlanningToolMode.PlaceClipboard)
            {
                if (e.TryConsume(ToolKeyBindings.ClipBoardRotateCCWAction.GetKAction()))
                {
                    PlanningToolInterface.Instance.Clipboard.Rotate(false);
                    PlanningToolInterface.Instance.RefreshClipboardVisualisationPreview();
                }
                else if (e.TryConsume(ToolKeyBindings.ClipBoardRotateCWAction.GetKAction()))
                {
                    PlanningToolInterface.Instance.Clipboard.Rotate(true);
                    PlanningToolInterface.Instance.RefreshClipboardVisualisationPreview();
                }
                else if (e.TryConsume(ToolKeyBindings.ClipBoardFlipAction.GetKAction()))
                {
                    PlanningToolInterface.Instance.Clipboard.Flip(true);
                    PlanningToolInterface.Instance.RefreshClipboardVisualisationPreview();
                }
            }

            if (e.TryConsume(ToolKeyBindings.SampleToolAction.GetKAction()))
            {
                Settings.PlanningMode = PlanningToolSettings.PlanningToolMode.SamplePlan;
            }
            else if (e.TryConsume(ToolKeyBindings.CopyPlanAction.GetKAction()))
            {
                Settings.PlanningMode = PlanningToolSettings.PlanningToolMode.CopyArea;
            }
            else if (e.TryConsume(ToolKeyBindings.CutPlanAction.GetKAction()))
            {
                Settings.PlanningMode = PlanningToolSettings.PlanningToolMode.CutArea;
            }
            else if (e.TryConsume(ToolKeyBindings.PastePlanAction.GetKAction()))
            {
                Settings.PlanningMode = PlanningToolSettings.PlanningToolMode.PlaceClipboard;
            }
            else if (e.TryConsume(ToolKeyBindings.SwitchShapeAction.GetKAction()))
            {
                if (RadialMenu.IsStarted)
                    RadialMenu.CancelMenu();
                var radialOptions = new List<RadialMenuOption>();
                for (var i = 0; i < _planShapes.Count; i++)
                {
                    var planShape = _planShapes[i];

                    radialOptions.Add(new RadialMenuOption
                    {
                        WasSelected = PlanningToolSettings.Instance.ActiveShape == planShape,
                        Icon = _planShapeSprites[i],
                        Label = Enum.GetName(typeof(PlanShape), planShape),
                        Color = Color.Lerp(Color.black, Color.white, 0.1f),
                        HighlightColor = Color.Lerp(Color.black, Color.white, 0.3f)
                    });
                }

                RadialMenu.StartMenu(radialOptions, i =>
                {
                    var nextShape = _planShapes[i];
                    PlanningToolSettings.Instance.ActiveShape = nextShape;
                });
            }
            else if (e.TryConsume(ToolKeyBindings.SwitchColorAction.GetKAction()))
            {
                if (RadialMenu.IsStarted)
                    RadialMenu.CancelMenu();
                var radialOptions = new List<RadialMenuOption>();
                foreach (var planColor in _planColors)
                {
                    var color = planColor.AsColor();
                    var highlightColor = Color.Lerp(color, Color.white, 0.5f);
                    radialOptions.Add(new RadialMenuOption
                    {
                        Color = planColor.AsColor(),
                        HighlightColor = highlightColor,
                        WasSelected = PlanningToolSettings.Instance.ActiveColor == planColor
                    });
                }

                RadialMenu.StartMenu(radialOptions, i =>
                {
                    var nextColor = _planColors[i];
                    PlanningToolSettings.Instance.ActiveColor = nextColor;
                });
            }

            if (e.Consumed)
                return;
            base.OnKeyDown(e);
        }

        public override void OnKeyUp(KButtonEvent e)
        {
            if (e.Consumed)
                return;
            if (RadialMenu.IsStarted)
            {
                if (e.TryConsume(Action.MouseRight))
                {
                    RadialMenu.CancelMenu();
                    return;
                }

                if (e.TryConsume(ToolKeyBindings.SwitchColorAction.GetKAction()))
                {
                    RadialMenu.MakeSelection();
                    return;
                }

                if (e.TryConsume(ToolKeyBindings.SwitchShapeAction.GetKAction()))
                {
                    RadialMenu.MakeSelection();
                    return;
                }
            }
            var action = e.GetAction();
            var keyCode = e.Controller.GetInputForAction(action);
            if (keyCode == KKeyCode.Mouse1)
            {
                if (PlanningToolInterface.Instance.ToolActive &&
                    (Settings.PlanningMode == PlanningToolSettings.PlanningToolMode.PlaceClipboard ||
                     Settings.PlanningMode == PlanningToolSettings.PlanningToolMode.CopyArea ||
                     Settings.PlanningMode == PlanningToolSettings.PlanningToolMode.CutArea ||
                     Settings.PlanningMode == PlanningToolSettings.PlanningToolMode.SamplePlan))
                {
                    // cancel while placing clipboard or other submenu tools should just go back to drag mode
                    PlaySound(GlobalAssets.GetSound("Tile_Cancel"));
                    Settings.PlanningMode = PlanningToolSettings.PlanningToolMode.DragPlan;
                    if (PlanningToolInterface.Instance.Dragging)
                        PlanningToolInterface.Instance.CancelDragging();
                    e.TryConsume(action);
                    return;
                }
            }
            base.OnKeyUp(e);
        }
    }
}
