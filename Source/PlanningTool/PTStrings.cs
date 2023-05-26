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
