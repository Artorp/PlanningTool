using System.IO;
using System.Reflection;
using PeterHan.PLib.UI;
using UnityEngine;

namespace PlanningTool
{
    public class PTAssets
    {
        public static bool IsInitialized;

        public const string RectanglePrefix = "plans_rectangle";
        public const string CirclePrefix = "plans_circle";
        public const string DiamondPrefix = "plans_diamond";
        public const string SketchStyleSuffix = "_sketch";
        public const string SimpleStyleSuffix = "_simple";
        public const string PlanFileExtension = ".png";
        public static Material RectangleMaterial;
        public static Sprite RectangleSprite;
        public static Material CircleMaterial;
        public static Sprite CircleSprite;
        public static Material DiamondMaterial;
        public static Sprite DiamondSprite;
        public const string SelectionOutlinePath = "SelectionOutline.png";
        public static Material SelectionOutlineMaterial;
        public const string WhiteBGPath = "White_16x16.png";
        public static Sprite WhiteBGSprite;

        public const string CursorPlanningPath = "cursor_arrow_planning.png";
        public static Texture2D CursorPlanning;
        public const string CursorPipettePath = "cursor_pipette.png";
        public static Texture2D CursorPipette;
        public const string CursorPipetteInvalidPath = "cursor_pipette_invalid.png";
        public static Texture2D CursorPipetteInvalid;
        public const string CursorEraserPath = "cursor_eraser.png";
        public static Texture2D CursorEraser;
        public const string IconToolPlanningPath = "tool_planning.png";
        public static Sprite IconToolPlanning;
        public const string IconToolHideShowPath = "tool_hide_show.png";
        public static Sprite IconToolHideShow;
        public const string IconToolHideShowHiddenPath = "tool_hide_show_hidden.png";
        public static Sprite IconToolHideShowHidden;
        public const string IconToolEraserPath = "tool_eraser.png";
        public static Sprite IconToolEraser;
        public const string IconToolCopyPath = "tool_copy.png";
        public static Sprite IconToolCopy;
        public const string IconToolCutPath = "tool_cut.png";
        public static Sprite IconToolCut;
        public const string IconToolPastePath = "tool_paste.png";
        public static Sprite IconToolPaste;
        public const string IconToolImportPath = "tool_import.png";
        public static Sprite IconToolImport;
        public const string IconToolExportPath = "tool_export.png";
        public static Sprite IconToolExport;

        public static string RadialMenuElementPath = "radial_menu_element.png";
        public static Texture2D RadialMenuElement;

        public static Shader SelectedShader;

        public static void Initialize()
        {
            if (IsInitialized)
            {
                Debug.LogWarning("[PlanningTool] PTAssets.Initialize() called but already initialized.");
                return;
            }

            SelectedShader = FindBlendedShader();
            LoadShapeTextures(ModOptions.Options);
            Texture2D tex = LoadResourceFileTexture(SelectionOutlinePath);
            var grey = new Color(0.96f, 0.96f, 0.96f, 0.5f);
            SelectionOutlineMaterial = CreateMaterialWithTexture(tex, grey);
            WhiteBGSprite = PUIUtils.LoadSpriteFile(AsInResourceFolder(WhiteBGPath));
            CursorPlanning = LoadResourceFileTexture(CursorPlanningPath);
            CursorPipette = LoadResourceFileTexture(CursorPipettePath);
            CursorPipetteInvalid = LoadResourceFileTexture(CursorPipetteInvalidPath);
            CursorEraser = LoadResourceFileTexture(CursorEraserPath);
            IconToolPlanning = PUIUtils.LoadSpriteFile(AsInResourceFolder(IconToolPlanningPath));
            IconToolPlanning.name = "PlanningTool.tool_planning";
            Assets.Sprites.Add(IconToolPlanning.name, IconToolPlanning);
            IconToolHideShow = PUIUtils.LoadSpriteFile(AsInResourceFolder(IconToolHideShowPath));
            IconToolHideShowHidden = PUIUtils.LoadSpriteFile(AsInResourceFolder(IconToolHideShowHiddenPath));
            IconToolEraser = PUIUtils.LoadSpriteFile(AsInResourceFolder(IconToolEraserPath));
            IconToolCopy = PUIUtils.LoadSpriteFile(AsInResourceFolder(IconToolCopyPath));
            IconToolCut = PUIUtils.LoadSpriteFile(AsInResourceFolder(IconToolCutPath));
            IconToolPaste = PUIUtils.LoadSpriteFile(AsInResourceFolder(IconToolPastePath));
            IconToolImport = PUIUtils.LoadSpriteFile(AsInResourceFolder(IconToolImportPath));
            IconToolExport = PUIUtils.LoadSpriteFile(AsInResourceFolder(IconToolExportPath));
            RadialMenuElement = LoadResourceFileTexture(RadialMenuElementPath);

            IsInitialized = true;
        }

        public static void LoadShapeTextures(ModOptions options)
        {
            if (options == null) options = new ModOptions();
            var style = options.Style == ModOptions.PlanStyle.Sketch ? SketchStyleSuffix : SimpleStyleSuffix;
            var rectanglePath = RectanglePrefix + style + PlanFileExtension;
            var circlePath = CirclePrefix + style + PlanFileExtension;
            var diamondPath = DiamondPrefix + style + PlanFileExtension;
            var grey = new Color(0.96f, 0.96f, 0.96f, 0.5f);
            var tex = LoadResourceFileTexture(rectanglePath);
            RectangleMaterial = CreateMaterialWithTexture(tex, grey);
            RectangleSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            tex = LoadResourceFileTexture(circlePath);
            CircleMaterial = CreateMaterialWithTexture(tex, grey);
            CircleSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            tex = LoadResourceFileTexture(diamondPath);
            DiamondMaterial = CreateMaterialWithTexture(tex, grey);
            DiamondSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        private static Shader FindBlendedShader()
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
                    "[PlanningTool] Unable to find shader 'Klei/Area Visualizer', attempting fallback to non-blended shader 'Klei/Unlit Transparent'");
                blendedShader = Shader.Find("Klei/Unlit Transparent");
            }

            if (blendedShader == null)
            {
                Debug.LogWarning("[PlanningTool] No suitable shader found, falling back to digPlacer material");
                blendedShader = Assets.instance.digPlacerAssets.materials[0].shader;
            }

            return blendedShader;
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
            Texture2D texture = new Texture2D(2, 2, textureFormat, true)
            {
                filterMode = FilterMode.Trilinear
            };
            byte[] data = File.ReadAllBytes(fullPath);
            texture.LoadImage(data);

            return texture;
        }
    }
}
