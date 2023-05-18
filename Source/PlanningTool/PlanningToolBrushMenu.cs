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

        // public override float GetSortKey() => 50f;  // TODO: increase when PlanningTool is active to hijack inputs

        public GameObject row;
        private List<PlanColor> _planColors;

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
            ppanel.AddChild(new UIComponentWrapper(hideButton));

            var maxSliderValue = 20f;
            var alphaSlider = new PSliderSingle("ActiveAlphaSlider")
            {
                InitialValue = SaveLoadPlans.Instance.ActiveFloat * maxSliderValue,
                IntegersOnly = true,
                MaxValue = maxSliderValue,
                OnValueChanged = (source, value) =>
                {
                    var valueScaled = value / maxSliderValue;
                    SaveLoadPlans.Instance.ActiveFloat = valueScaled;
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
            ppanel.AddChild(alphaSlider);

            row = ppanel.Build();
            row.transform.SetParent(transform, false);
            row.transform.SetSiblingIndex(row.transform.GetSiblingIndex() - 2); // move to just below the priority screen (todo: find priority screen by name, then move it)

            // try to add buttons like sandbox tools

            var toggleParent = new PPanel("ColorShapeGroup")
            {
                Direction = PanelDirection.Horizontal
            }.Build();
            toggleParent.transform.SetParent(row.transform, false);
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
        }

    }
}
