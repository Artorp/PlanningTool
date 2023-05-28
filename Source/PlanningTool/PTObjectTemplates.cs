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
            if (ToolMenu.Instance.sandboxToolIconPrefab == null)
                Debug.LogWarning("[PlanningTool] ToolMenu.Instance.sandboxToolIconPrefab was null, planning tool submenu likely to fail");
            var button = Util.KInstantiateUI(ToolMenu.Instance.sandboxToolIconPrefab, parent, true);
            ValidateButtonPrefab(button);
            button.name = name;
            button.transform.Find("FG").GetComponent<Image>().sprite = sprite;
            var lt = button.transform.Find("Text")?.GetComponent<LocText>();
            if (lt != null) lt.text = name;
            var tComp = button.GetComponent<KToggle>();
            tComp.artExtension.animator = null;
            return button;
        }

        /// <summary>
        /// If sandboxToolIconPrefab changes in the future, this method will give warnings so that the changes can be addressed
        /// </summary>
        /// <param name="button">instantiation of ToolMenu.Instance.sandboxToolIconPrefab</param>
        private static void ValidateButtonPrefab(GameObject button)
        {
            if (button == null)
            {
                Debug.LogWarning("[PlanningTool] Instantiation of sandboxToolIconPrefab was null, planning tool submenu likely to fail");
                return;
            }

            if (button.transform.Find("FG") == null)
            {
                Debug.LogWarning("[PlanningTool] sandboxToolIconPrefab.[name=FG] was null, planning tool submenu likely to fail");
                return;
            }

            if (button.transform.Find("FG").GetComponent<Image>() == null)
            {
                Debug.LogWarning("[PlanningTool] sandboxToolIconPrefab.FG had no Image component, planning tool submenu likely to fail");
                return;
            }

            if (button.transform.Find("Text") == null)
            {
                Debug.LogWarning("[PlanningTool] sandboxToolIconPrefab.[name=Text] was null, planning tool submenu likely to fail");
                return;
            }

            if (button.transform.Find("Text").GetComponent<LocText>() == null)
            {
                Debug.LogWarning("[PlanningTool] sandboxToolIconPrefab.Text had no LocText component, planning tool submenu likely to fail");
                return;
            }

            if (button.GetComponent<KToggle>() == null)
            {
                Debug.LogWarning("[PlanningTool] sandboxToolIconPrefab had no KToggle component, planning tool submenu likely to fail");
            }
        }
    }
}
