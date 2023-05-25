using System.Collections.Generic;
using PeterHan.PLib.UI;
using UnityEngine;
using UnityEngine.UI;

namespace PlanningTool
{
    public class RadialMenuHelper
    {
        public const float MenuGapDegrees = 2f;
        public const float InnerRadius = 0.3f;
        public const float OuterRadius = 1.1f;
        public class OptionComponents
        {
            public GameObject GameObject;
            public GameObject IconOrText;
            public MeshRenderer MeshRenderer;
        }

        public static List<OptionComponents> GenerateOptionsFor(GameObject parent, List<RadialMenuOption> options, Color defaultColor)
        {
            var optionArcDegrees = 360f / options.Count - MenuGapDegrees;
            var currentAngle = MenuGapDegrees / 2f;
            var circleVerticesTotal = 128;
            var verticesPerAngle = circleVerticesTotal / 360f;
            var resolution = Mathf.RoundToInt(verticesPerAngle * optionArcDegrees);
            var unlitTransparent = Shader.Find("Klei/Unlit Transparent");

            List<OptionComponents> generatedObjects = new List<OptionComponents>();

            foreach (var menuOption in options)
            {
                var annulusPiece = new GameObject("Annulus");

                RadialMenuMeshGeneration.GenerateAnnulus(annulusPiece, InnerRadius, OuterRadius, optionArcDegrees, resolution, true);

                if (menuOption.WasSelected)
                {
                    var selectionIndicator = new GameObject("SelectionIndicator");
                    var thickness = 0.05f;
                    var gap = 0.02f;
                    RadialMenuMeshGeneration.GenerateAnnulus(selectionIndicator, InnerRadius - gap - thickness, InnerRadius - gap, optionArcDegrees, resolution, true);
                    var indicatorRenderer = selectionIndicator.AddComponent<MeshRenderer>();
                    var indicatorMat = new Material(unlitTransparent)
                    {
                        color = Color.Lerp(Color.yellow, Color.red, 0.5f),
                        mainTexture = PTAssets.RadialMenuElement
                    };
                    indicatorRenderer.material = indicatorMat;
                    selectionIndicator.transform.SetParent(annulusPiece.transform, false);
                }

                var meshRenderer = annulusPiece.AddComponent<MeshRenderer>();
                var mat = new Material(unlitTransparent)
                {
                    color = menuOption.Color ?? defaultColor,
                    mainTexture = PTAssets.RadialMenuElement
                };
                meshRenderer.material = mat;

                // anchor for icon and label
                var textIconGameObject = new GameObject("TextIcons");
                textIconGameObject.transform.SetParent(parent.transform, false);
                var textIconPos = textIconGameObject.transform.localPosition;
                textIconPos.y += (InnerRadius + OuterRadius) / 2f;
                textIconPos.z -= 0.05f;
                textIconGameObject.transform.localPosition = textIconPos;
                textIconGameObject.transform.RotateAround(parent.transform.position, Vector3.back, currentAngle + optionArcDegrees / 2f);
                textIconGameObject.transform.rotation = Quaternion.identity;

                if (menuOption.Label != null || menuOption.Icon != null)
                {
                    // Outer container stretches to fill entire canvas, inner container size is that of the inner elements
                    var container = new PPanel("IconTextContainerInner")
                    {
                        Direction = PanelDirection.Vertical,
                    };
                    var container2 = new PPanel("IconTextContainerOuter");
                    container2.AddChild(container);
                    if (menuOption.Icon != null)
                    {
                        var icon = new GameObject("Icon");
                        icon.AddComponent<CanvasRenderer>();
                        var image = icon.AddComponent<Image>();
                        image.sprite = menuOption.Icon;
                        image.color = Color.white;
                        image.preserveAspect = true;
                        icon.SetUISize(Vector2.one * 50f, true);
                        container.AddChild(new UIComponentWrapper(icon));
                    }

                    if (menuOption.Label != null)
                    {
                        var label = new PLabel("Text")
                        {
                            Text = menuOption.Label
                        };
                        container.AddChild(label);
                    }
                    var rectTransform = textIconGameObject.AddComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(200f, 200f);
                    var canvas = textIconGameObject.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.sortingOrder = 1;
                    textIconGameObject.transform.localScale = Vector3.one * 0.008f;

                    var containerGameObject = container2.Build();
                    containerGameObject.transform.SetParent(textIconGameObject.transform, false);
                }

                textIconGameObject.SetLayerRecursively(LayerMask.NameToLayer("UI"));

                annulusPiece.transform.SetParent(parent.transform, false);
                annulusPiece.transform.Rotate(Vector3.back, currentAngle, Space.Self);
                annulusPiece.SetLayerRecursively(LayerMask.NameToLayer("UI"));

                generatedObjects.Add(new OptionComponents
                {
                    GameObject = annulusPiece,
                    IconOrText = textIconGameObject,
                    MeshRenderer = meshRenderer
                });

                currentAngle += optionArcDegrees + MenuGapDegrees;
            }

            return generatedObjects;
        }
    }
}
