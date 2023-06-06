using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PeterHan.PLib.Options;

namespace PlanningTool
{
    [JsonObject(MemberSerialization.OptIn)]
    [ModInfo("https://github.com/Artorp/PlanningTool")]
    [ConfigFile(SharedConfigLocation: true)]
    public class ModOptions : IOptions
    {
        private static ModOptions _options;
        public static ModOptions Options
        {
            get
            {
                if (_options == null)
                {
                    LoadOptions();
                }

                return _options;
            }
        }

        public static event Action<ModOptions> OnOptionsChangedEvent;

        [Option("PlanningTool.PTStrings.SETTINGS.REMOVE_PLAN_ON_CONSTRUCTION_TITLE", "PlanningTool.PTStrings.SETTINGS.REMOVE_PLAN_ON_CONSTRUCTION_TOOLTIP")]
        [JsonProperty]
        public bool RemovePlansOnConstruction { get; set; }

        [Option("PlanningTool.PTStrings.SETTINGS.PLAN_STYLE_TITLE", "PlanningTool.PTStrings.SETTINGS.PLAN_STYLE_TOOLTIP")]
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public PlanStyle Style { get; set; }

        public ModOptions()
        {
            RemovePlansOnConstruction = true;
        }

        public IEnumerable<IOptionsEntry> CreateOptions()
        {
            return new List<IOptionsEntry>();
        }

        public void OnOptionsChanged()
        {
            LoadOptions();
            OnOptionsChangedEvent?.Signal(Options);
        }

        public static void LoadOptions()
        {
            _options = POptions.ReadSettings<ModOptions>() ?? new ModOptions();
        }

        public enum PlanStyle
        {
            [Option("PlanningTool.PTStrings.SETTINGS.PLAN_STYLE_SKETCH")]
            Sketch,
            [Option("PlanningTool.PTStrings.SETTINGS.PLAN_STYLE_SIMPLE")]
            Simple
        }
    }
}
