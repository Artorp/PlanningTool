using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace PlanningTool
{
    public class PTObjectTemplates
    {
        protected static GameObject PlanningTileSprite;
        protected static GameObject PlanningTileMesh;

        public static void CreateTemplates()
        {
            InitializePlanningTileSprite();
            InitializePlanningTileMesh();
        }

        private static void InitializePlanningTileSprite()
        {
            if (PlanningTileSprite != null) Object.Destroy(PlanningTileSprite);
            PlanningTileSprite = new GameObject("PlanningTileSprite");
            PlanningTileSprite.SetActive(false);
            PlanningTileSprite.AddComponent<SpriteRenderer>().sprite = PTAssets.RectangleSprite;
            PlanningTileSprite.SetLayerRecursively(LayerMask.NameToLayer("PlaceWithDepth"));
            Object.DontDestroyOnLoad(PlanningTileSprite);
        }

        private static void InitializePlanningTileMesh()
        {
            // based on CommonPlacerConfig.CreatePrefab
            if (PlanningTileMesh != null) Object.Destroy(PlanningTileMesh);
            PlanningTileMesh = new GameObject("PlanningTileMesh");
            PlanningTileMesh.SetActive(false);
            GameObject gameObject = new GameObject("Mask");
            gameObject.transform.parent = PlanningTileMesh.transform;
            gameObject.transform.SetLocalPosition(new Vector3(0.0f, 0.5f, -3.537f));
            gameObject.transform.eulerAngles = new Vector3(0.0f, 180f, 0.0f);
            gameObject.AddComponent<MeshFilter>().sharedMesh = Assets.instance.commonPlacerAssets.mesh;
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            meshRenderer.sharedMaterial = PTAssets.RectangleMaterial;

            gameObject.AddComponent<EasingAnimations>().scales = new[]
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
            PlanningTileSprite.SetLayerRecursively(LayerMask.NameToLayer("PlaceWithDepth"));

            Object.DontDestroyOnLoad(PlanningTileMesh);
        }

        public static GameObject CreatePlanningTileSprite(string id)
        {
            var go = Object.Instantiate(PlanningTileSprite);
            go.name = id;
            go.SetLayerRecursively(LayerMask.NameToLayer("PlaceWithDepth"));
            return go;
        }

        public static GameObject CreatePlanningTileMesh(string id, PlanShape shape, PlanColor planColor, bool useActiveAlpha = true)
        {
            var go = Object.Instantiate(PlanningTileMesh);
            go.name = id;
            var meshRenderer = go.transform.Find("Mask").GetComponent<MeshRenderer>();
            if (shape == PlanShape.Circle)
            {
                meshRenderer.sharedMaterial = PTAssets.CircleMaterial;
            }
            else if (shape == PlanShape.Diamond)
            {
                meshRenderer.sharedMaterial = PTAssets.DiamondMaterial;
            }

            var color = planColor.AsColor();
            if (useActiveAlpha)
                color.a = SaveLoadPlans.Instance.ActiveAlpha;
            meshRenderer.material.color = color;

            go.SetLayerRecursively(LayerMask.NameToLayer("PlaceWithDepth"));
            return go;
        }

        public static GameObject CreateSquareButton(string name, string sprite, GameObject parent)
        {
            var s = Assets.GetSprite((HashedString) sprite);
            return CreateSquareButton(name, s, parent);
        }

        public static GameObject CreateSquareButton(string name, Sprite sprite, GameObject parent)
        {
            var button = Util.KInstantiateUI(ToolMenu.Instance.sandboxToolIconPrefab, parent, true);
            button.name = name;
            button.transform.Find("FG").GetComponent<Image>().sprite = sprite;
            var lt = button.transform.Find("Text")?.GetComponent<LocText>();
            if (lt != null) lt.text = name;
            var tComp = button.GetComponent<KToggle>();
            tComp.artExtension.animator = null;
            return button;
        }
    }
}
