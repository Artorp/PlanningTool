using System.IO;
using System.Reflection;
using UnityEngine;

namespace PlanningTool
{
    public class PTAssets
    {
        public static bool IsInitialized = false;

        public const string RectangleImageAssemblyPath = "PlanningTool.Images.Rectangle.png";
        public static Sprite RectangleSprite;

        public const string CirclePath = "Circle.png";
        public static Material CircleMaterial;
        public const string RectanglePath = "Rectangle.png";
        public static Material RectangleMaterial;
        public const string SmallRectPath = "SmallRect.png";
        public static Material SmallRectMaterial;
        public const string SelectionOutlinePath = "SelectionOutline.png";
        public static Material SelectionOutlineMaterial;

        public static Shader SelectedShader;

        public static void Initialize()
        {
            if (IsInitialized)
            {
                Debug.LogWarning("[PlanningTool] PTAssets.Initialize() called but already initialized.");
                return;
            }

            // TODO: check performance of both SpriteRenderer and MeshRenderer, use best one
            // RectangleSprite = PUIUtils.LoadSprite(RectangleImageAssemblyPath);

            // attempt to find suiting shader with blended transparency
            InitializeShader();
            var grey = new Color(0.96f, 0.96f, 0.96f, 0.5f);
            var tex = LoadResourceFileTexture(CirclePath);
            CircleMaterial = CreateMaterialWithTexture(tex, grey);
            tex = LoadResourceFileTexture(RectanglePath);
            RectangleMaterial = CreateMaterialWithTexture(tex, grey);
            tex = LoadResourceFileTexture(SmallRectPath);
            SmallRectMaterial = CreateMaterialWithTexture(tex, grey);
            tex = LoadResourceFileTexture(SelectionOutlinePath);
            SelectionOutlineMaterial = CreateMaterialWithTexture(tex, grey);

            IsInitialized = true;
        }

        private static void InitializeShader()
        {
            var blendedShader = Shader.Find("Klei/Area Visualizer");
            blendedShader = null;
            if (blendedShader == null)
            {
                // Klei/BuildingCell is blended, but has a color tint
                Debug.LogWarning(
                    "[PlanningTool] Unable to find shader 'Klei/Area Visualizer', attempting fallback shader 'Klei/BuildingCell'");
                blendedShader = Shader.Find("Klei/BuildingCell");
            }

            if (blendedShader == null)
            {
                Debug.LogWarning(
                    "[PlanningTool] Unable to find shader 'Klei/BuildingCell', attempting fallback to non-blended shader 'Klei/Unlit Transparent'");
                blendedShader = Shader.Find("Klei/Unlit Transparent");
            }

            if (blendedShader == null)
            {
                Debug.LogWarning("[PlanningTool] No suitable shader found, falling back to digPlacer material");
                blendedShader = Assets.instance.digPlacerAssets.materials[0].shader;
            }

            SelectedShader = blendedShader;
        }

        public static Material CreateMaterialWithTexture(Texture2D texture, Color color)
        {
            var material =  new Material(SelectedShader);
            material.mainTexture = texture;
            material.color = color;
            return material;
        }

        public static Texture2D LoadEmbeddedTexture(string resourcePath)
        {
            Texture2D texture = new Texture2D(2, 2);
            var assembly = Assembly.GetExecutingAssembly();
            try
            {
                using (var stream = assembly.GetManifestResourceStream(resourcePath))
                {
                    var length = (int)stream.Length;
                    byte[] data = new byte[length];
                    int offset = 0;
                    while (offset < length)
                    {
                        int bytesRead = stream.Read(data, offset, length - offset);
                        if (bytesRead == 0)
                        {
                            throw new EndOfStreamException("Unexpected end of stream");
                        }

                        offset += bytesRead;
                    }
                    texture.LoadImage(data);
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to load texture {RectangleImageAssemblyPath}: {e.Message}" );
            }

            return texture;
        }

        public static Texture2D LoadResourceFileTexture(string fileName, TextureFormat textureFormat = TextureFormat.RGBA32)
        {
            string fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Resources", fileName);
            Texture2D texture = new Texture2D(2, 2, textureFormat, false)
            {
                filterMode = FilterMode.Trilinear
            };
            byte[] data = File.ReadAllBytes(fullPath);
            texture.LoadImage(data);

            return texture;
        }
    }
}
