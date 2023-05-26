using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PeterHan.PLib.Options;

namespace PlanningTool
{
    [JsonObject(MemberSerialization.OptIn)]
    [ModInfo("https://github.com/Artorp/PlanningTool")]
    public class ModOptions : IOptions
    {
        public static ModOptions Options { get; private set; }

        [Option("PlanningTool.PTStrings.SETTINGS.SWITCH_PLAN_FILTER_TITLE", "PlanningTool.PTStrings.SETTINGS.SWITCH_PLAN_FILTER_TOOLTIP")]
        [JsonProperty]
        public bool SwitchPlanFilter { get; set; }

        [Option("PlanningTool.PTStrings.SETTINGS.AUTO_SWITCH_TO_TITLE", "PlanningTool.PTStrings.SETTINGS.AUTO_SWITCH_TO_TOOLTIP")]
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public AutoSwitchTarget AutoSwitchTo { get; set; }

        public ModOptions()
        {
            SwitchPlanFilter = true;
            AutoSwitchTo = AutoSwitchTarget.Plans;
        }

        public IEnumerable<IOptionsEntry> CreateOptions()
        {
            return new List<IOptionsEntry>();
        }

        public void OnOptionsChanged()
        {
            LoadOptions();
        }

        public static void LoadOptions()
        {
            Options = POptions.ReadSettings<ModOptions>() ?? new ModOptions();
        }

        public enum AutoSwitchTarget
        {
            [Option("PlanningTool.PTStrings.PLANNING_TOOL_FILTER_ITEM")]
            Plans,
            [Option("STRINGS.UI.TOOLS.FILTERLAYERS.ALL")]
            All
        }
    }
}
