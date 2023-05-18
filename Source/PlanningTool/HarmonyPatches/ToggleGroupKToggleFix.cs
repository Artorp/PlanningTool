using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine.UI;

namespace PlanningTool.HarmonyPatches
{
    /// <summary>
    /// The KToggle implementation's of property isOn uses 'public new bool isOn' to hide the base class isOn.
    /// Since the base class isOn is being hidden and not overwritten, other classes that uses KToggle with a type
    /// of Toggle and that adjusts the isOn will modify Toggle.isOn and not KToggle.isOn even though the instance object
    /// is of type KToggle.
    ///
    /// Attempt to downcast to KToggle and use KToggle.isOn in cases where KToggle.isOn is written to.
    /// </summary>
    [HarmonyPatch(typeof(ToggleGroup))]
    public class ToggleGroupKToggleFix
    {
        private static MethodBase KToggleOnValueChange;

        /// <summary>
        /// Called by Harmony before patching methods. Called once at the start with original == null, and once
        /// for each patch in class
        /// </summary>
        /// <param name="original">null first call, then the original method to be patched</param>
        /// <returns>if true will patch methods, otherwise will skip patching</returns>
        static bool Prepare(MethodBase original)
        {
            if (original == null)
            {
                KToggleOnValueChange = AccessTools.Method(typeof(KToggle), "OnValueChanged", new[] { typeof(bool) });
                if (KToggleOnValueChange == null)
                    Debug.LogWarning("[PlanningTool] KToggle.OnValueChanged not found, not patching KToggle.isOn fix");
            }
            var shouldPatch = !(KToggleOnValueChange == null);
            return shouldPatch;
        }

        [HarmonyPatch(nameof(ToggleGroup.NotifyToggleOn))]
        [HarmonyPostfix]
        public static void NotifyToggleOn_Patch(List<Toggle> ___m_Toggles, Toggle toggle, bool sendCallback)
        {
            if (KToggleOnValueChange == null)
                return;
            if (!sendCallback) return;
            foreach (var toggle2 in ___m_Toggles)
            {
                if (toggle2 == toggle) continue;
                if (toggle is KToggle kToggle)
                    KToggleOnValueChange.Invoke(kToggle, new object[] { false });
            }
        }

        [HarmonyPatch(nameof(ToggleGroup.SetAllTogglesOff))]
        [HarmonyPostfix]
        public static void SetAllTogglesOff_Patch(List<Toggle> ___m_Toggles, bool sendCallback)
        {
            if (KToggleOnValueChange == null)
                return;
            if (sendCallback)
            {
                foreach (var toggle in ___m_Toggles)
                {
                    if (toggle is KToggle kToggle)
                        KToggleOnValueChange.Invoke(kToggle, new object[] { false });
                }
            }
        }
    }
}
