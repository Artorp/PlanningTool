using System;
using System.Reflection;
using HarmonyLib;
using PeterHan.PLib.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PlanningTool
{
    public class PlanningToolInterface : DragTool
    {
        public static PlanningToolInterface Instance;

        public static void DestroyInstance() => Instance = null;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Debug.Log("PlanningToolInterface.OnPrefabInit()");
            Instance = this;

            // populate all fields tagged with [SerializeField] (or public) in DragTool that is probably
            // set through the Unity inspector, using values from DigTool

            FieldInfo areaVisualizerField = AccessTools.Field(typeof(DragTool), "areaVisualizer");

            visualizer = Util.KInstantiate(DigTool.Instance.visualizer, gameObject);
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
            Debug.Log("PlanningToolInterface.OnDragTool with cell " + cell);
            // InterfaceTool.ActiveConfig.DigAction.Uproot(cell);
            // InterfaceTool.ActiveConfig.DigAction.Dig(cell, distFromOrigin);
            // TODO: manage this on my own implementation of Grid that keeps track of plans?
            // for now, just add gameobjects with custom texture
            var go = new GameObject("PlanOverlay");
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.color = new Color(0.96f, 0.95f, 0.96f, 0.5f);
            // TODO: move sprite loading to own class
            Sprite sprite = PUIUtils.LoadSprite("PlanningTool.Images.Rectangle.png");
            sr.sprite = sprite;
            var pos = Grid.CellToPosCCC(cell, Grid.SceneLayer.TileFront);
            go.transform.localPosition = pos;
            go.transform.localScale = new Vector3(
                Grid.CellSizeInMeters / (sprite.texture.width / sprite.pixelsPerUnit),
                Grid.CellSizeInMeters / (sprite.texture.height / sprite.pixelsPerUnit)
            );

            go.SetLayerRecursively(LayerMask.NameToLayer("PlaceWithDepth"));

            // TODO: use distFromOrigin as a delay
            /*
            float animationDelay = distFromOrigin * 0.02f;
            go.AddComponent<EasingAnimations>().scales = new EasingAnimations.AnimationScales[2]
            {
                new EasingAnimations.AnimationScales()
                {
                    name = "ScaleUp",
                    startScale = 0.0f,
                    endScale = 1f,
                    type = EasingAnimations.AnimationScales.AnimationType.EaseInOutBack,
                    easingMultiplier = 5f
                },
                new EasingAnimations.AnimationScales()
                {
                    name = "ScaleDown",
                    startScale = 1f,
                    endScale = 0.0f,
                    type = EasingAnimations.AnimationScales.AnimationType.EaseOutBack,
                    easingMultiplier = 1f
                }
            };
            go.AddOrGet<EasingAnimations>().PlayAnimation("ScaleUp", Mathf.Max(0.0f, animationDelay));
            */

            GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(new Tag("DigPlacer")));
            Debug.Log($"digPlacerAssets has {Assets.instance.digPlacerAssets.materials.Length} materials.");
            var mat = Assets.instance.digPlacerAssets.materials[0];
            Debug.Log($"color={mat.color} instanceable={mat.enableInstancing}");
            Debug.Log($"tex={mat.mainTexture}, tex offset={mat.mainTextureOffset} tex scale={mat.mainTextureScale}");
            Debug.Log($"tex dims: width={mat.mainTexture.width} height={mat.mainTexture.height}");
            // todo:
            // show grid when placing plan (like when placing building)
            // show the plan to be placed when in tool menu
            // todo: only place icon if it doesn't exist in the grid, see DigTool.PlaceDig
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
