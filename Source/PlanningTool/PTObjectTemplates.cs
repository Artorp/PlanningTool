using UnityEngine;
using UnityEngine.Rendering;

namespace PlanningTool
{
    public class PTObjectTemplates
    {
        protected static GameObject planningTileSprite;
        protected static GameObject planningTileMesh;

        public static void CreateTemplates()
        {
            InitializePlanningTileSprite();
            InitializePlanningTileMesh();
        }

        private static void InitializePlanningTileSprite()
        {
            planningTileSprite = new GameObject("planningTileSprite");
            planningTileSprite.SetActive(false);
            planningTileSprite.AddComponent<SpriteRenderer>().sprite = PTAssets.RectangleSprite;
            planningTileSprite.SetLayerRecursively(LayerMask.NameToLayer("PlaceWithDepth"));
            Object.DontDestroyOnLoad(planningTileSprite);
        }

        private static void InitializePlanningTileMesh()
        {
            // based on CommonPlacerConfig.CreatePrefab
            planningTileMesh = new GameObject("planningTileMesh");
            planningTileMesh.SetActive(false);
            GameObject gameObject = new GameObject("Mask");
            gameObject.transform.parent = planningTileMesh.transform;
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
            planningTileSprite.SetLayerRecursively(LayerMask.NameToLayer("PlaceWithDepth"));

            Object.DontDestroyOnLoad(planningTileMesh);
        }

        public static GameObject CreatePlanningTileSprite(string id)
        {
            var go = Object.Instantiate(planningTileSprite);
            go.name = id;
            go.SetLayerRecursively(LayerMask.NameToLayer("PlaceWithDepth"));
            return go;
        }

        public static GameObject CreatePlanningTileMesh(string id)
        {
            var go = Object.Instantiate(planningTileMesh);
            go.name = id;
            go.SetLayerRecursively(LayerMask.NameToLayer("PlaceWithDepth"));
            return go;
        }
    }
}
