using System.IO;
using System.Reflection;

namespace PlanningTool
{
    public static class PTStrings
    {
        public static void GenerateTemplate()
        {
            var outputFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Localization.GenerateStringsTemplate(typeof(PTStrings), outputFolder);
        }

        public static LocString ACTION_PLANNING_TOOL_NAME = "Start planning";

        public static LocString PLANNING_TOOL_NAME = "Planning";
        public static LocString PLANNING_TOOL_TOOLTIP = "Create a plan {0}";

        public static LocString PLANNING_TOOL_FILTER_ITEM = "Plans";

        public static class PLANNING_TOOL_ACTIVE_BINDINGS
        {
            public static LocString GROUP_NAME = "Planning tool (selected)";
            public static LocString ROTATE_CLIPBOARD_CCW = "Rotate clipboard counter clockwise";
            public static LocString ROTATE_CLIPBOARD_CW = "Rotate clipboard clockwise";
            public static LocString FLIP_CLIPBOARD = "Flip clipboard";
            public static LocString SAMPLE_TOOL = "Sample plan";
            public static LocString COPY_PLAN = "Copy plan";
            public static LocString CUT_PLAN = "Cut plan";
            public static LocString PASTE_PLAN = "Paste plan";
            public static LocString SWITCH_SHAPE = "Switch plan shape";
            public static LocString SWITCH_COLOR = "Switch plan color";
            public static LocString HIDE_SHOW = "Hide / show plans";
        }

        public static class BRUSH_MENU
        {
            public static LocString HIDE_BUTTON_LABEL = "Hide / show";
            public static LocString HIDE_BUTTON_TOOLTIP_HEADER = "Hide / Show Plans";
            public static LocString HIDE_BUTTON_TOOLTIP_BODY = "Toggle visibility of placed plans {Hotkey}";
            public static LocString SLIDER_TEXT = "Transparency:\n{0:0.00}";
            public static LocString SLIDER_TOOLTIP = "Change the transparency of plans";
            public static LocString COPY_BUTTON_LABEL = "Copy";
            public static LocString COPY_BUTTON_TOOLTIP_HEADER = "Copy";
            public static LocString COPY_BUTTON_TOOLTIP_BODY = "Copy plans {Hotkey}";
            public static LocString CUT_BUTTON_LABEL = "Cut";
            public static LocString CUT_BUTTON_TOOLTIP_HEADER = "Cut";
            public static LocString CUT_BUTTON_TOOLTIP_BODY = "Cut plans {Hotkey}";
            public static LocString PASTE_BUTTON_LABEL = "Paste";
            public static LocString PASTE_BUTTON_TOOLTIP_HEADER = "Paste";
            public static LocString PASTE_BUTTON_TOOLTIP_BODY = "Paste previously copied plans {Hotkey}";
            public static LocString SAMPLE_BUTTON_LABEL = "Sample";
            public static LocString SAMPLE_BUTTON_TOOLTIP_HEADER = "Sample";
            public static LocString SAMPLE_BUTTON_TOOLTIP_BODY = "Copy shape and color from already placed plan {Hotkey}";
            public static LocString EXPORT_BUTTON_LABEL = "Export";
            public static LocString EXPORT_BUTTON_TOOLTIP_HEADER = "Export";
            public static LocString EXPORT_BUTTON_TOOLTIP_BODY = "Place the plan currently on the game's clipboard to your system's clipboard";
            public static LocString IMPORT_BUTTON_LABEL = "Import";
            public static LocString IMPORT_BUTTON_TOOLTIP_HEADER = "Import";
            public static LocString IMPORT_BUTTON_TOOLTIP_BODY = "Attempt to import a plan from the system's clipboard to the game's clipboard\n" +
                                                                 "Import errors are logged to the game's player.log file.";

        }

        public static class SHAPES
        {
            public static LocString RECTANGLE = "Rectangle";
            public static LocString CIRCLE = "Circle";
            public static LocString DIAMOND = "Diamond";
        }

        public static class COLORS
        {
            public static LocString GRAY = "Gray";
            public static LocString BLUE = "Blue";
            public static LocString GREEN = "Green";
            public static LocString RED = "Red";
            public static LocString CYAN = "Cyan";
            public static LocString MAGENTA = "Magenta";
            public static LocString VIOLET = "Violet";
            public static LocString ORANGE = "Orange";
            public static LocString YELLOW = "Yellow";
            public static LocString WHITE = "White";
            public static LocString BLACK = "Black";
        }

        public static class HOVER_CARD
        {
            public static LocString PLANNING_ACTION_NAME = "Place plan";
            public static LocString COPY_ACTION_NAME = "Copy plan";
            public static LocString CUT_ACTION_NAME = "Cut plan";
            public static LocString PASTE_ACTION_NAME = "Paste plan";
            public static LocString SAMPLE_ACTION_NAME = "Sample plan";
            public static LocString COPY_HOTKEY = "{Hotkey} - Copy";
            public static LocString CUT_HOTKEY = "{Hotkey} - Cut";
            public static LocString PASTE_HOTKEY = "{Hotkey} - Paste";
            public static LocString SAMPLE_HOTKEY = "{Hotkey} - Sample";
            public static LocString SWITCH_SHAPE_HOTKEY = "{Hotkey} - Switch shape";
            public static LocString SWITCH_COLOR_HOTKEY = "{Hotkey} - Switch color";
            public static LocString ROTATE_LEFT_HOTKEY = "{Hotkey} - Rotate left";
            public static LocString ROTATE_RIGHT_HOTKEY = "{Hotkey} - Rotate right";
            public static LocString FLIP_HORIZONTALLY_HOTKEY = "{Hotkey} - Flip horizontally";
        }

        public static class SETTINGS
        {
            public static LocString SWITCH_PLAN_FILTER_TITLE = "Auto-switch Cancel Tool Filter";

            public static LocString SWITCH_PLAN_FILTER_TOOLTIP =
                "If the cancel tool is selected while the planning tool is active and the filter is not 'Plans' or 'All', the cancel tool filter will automatically be set to the below setting.\n" +
                "This should save you a click if you left the cancel tool on something else, and ensure the cancel tool will always remove a plan when selected from planning mode.";

            public static LocString AUTO_SWITCH_TO_TITLE = "Auto-switch to";

            public static LocString AUTO_SWITCH_TO_TOOLTIP =
                "Which filter to switch to when selecting cancel tool with planning tool active. Does nothing if the previous setting is disabled.";
        }
    }
}
