using System.IO;
using System.Reflection;
using PeterHan.PLib.UI;
using UnityEngine;

namespace PlanningTool
{
    public class PTAssets
    {
        public static bool IsInitialized;

        public const string RectanglePath = "Rectangle.png";
        public static Material RectangleMaterial;
        public static Sprite RectangleSprite;
        public const string CirclePath = "Circle.png";
        public static Material CircleMaterial;
        public static Sprite CircleSprite;
        public const string DiamondPath = "Diamond.png";
        public static Material DiamondMaterial;
        public static Sprite DiamondSprite;
        public const string SelectionOutlinePath = "SelectionOutline.png";
        public static Material SelectionOutlineMaterial;
        public const string WhiteBGPath = "White_16x16.png";
        public static Sprite WhiteBGSprite;

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

            InitializeShader();
            var grey = new Color(0.96f, 0.96f, 0.96f, 0.5f);
            var tex = LoadResourceFileTexture(RectanglePath);
            RectangleMaterial = CreateMaterialWithTexture(tex, grey);
            RectangleSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            tex = LoadResourceFileTexture(CirclePath);
            CircleMaterial = CreateMaterialWithTexture(tex, grey);
            CircleSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            tex = LoadResourceFileTexture(DiamondPath);
            DiamondMaterial = CreateMaterialWithTexture(tex, grey);
            DiamondSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            tex = LoadResourceFileTexture(SelectionOutlinePath);
            SelectionOutlineMaterial = CreateMaterialWithTexture(tex, grey);
            WhiteBGSprite = PUIUtils.LoadSpriteFile(AsInResourceFolder(WhiteBGPath));

            IsInitialized = true;
        }

        private static void InitializeShader()
        {
            // attempt to find suiting shader with blended transparency
            var blendedShader = Shader.Find("Klei/BuildingCell");
            if (blendedShader == null)
            {
                Debug.LogWarning(
                    "[PlanningTool] Unable to find shader 'Klei/BuildingCell', attempting fallback shader 'Klei/Area Visualizer'");
                blendedShader = Shader.Find("Klei/Area Visualizer");
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
            var material =  new Material(SelectedShader)
            {
                mainTexture = texture,
                color = color
            };
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
                Debug.LogError($"Failed to load texture {resourcePath}: {e.Message}" );
            }

            return texture;
        }

        protected static string AsInResourceFolder(string fileName) => Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Resources", fileName);

        public static Texture2D LoadResourceFileTexture(string fileName, TextureFormat textureFormat = TextureFormat.RGBA32)
        {
            string fullPath = AsInResourceFolder(fileName);
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