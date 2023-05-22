using System;
using System.Collections.Generic;
using PeterHan.PLib.Actions;

namespace PlanningTool
{
    public static class ToolKeyBindings
    {
        public static PAction PlanningToolAction  { get; private set; }
        public static PAction ClipBoardRotateCCWAction { get; private set; }
        public static PAction ClipBoardRotateCWAction { get; private set; }
        public static PAction ClipBoardFlipAction { get; private set; }
        public static PAction CopyPlanAction { get; private set; }
        public static PAction CutPlanAction { get; private set; }
        public static PAction PastePlanAction { get; private set; }
        public static PAction SampleToolAction { get; private set; }
        public static PAction SwitchShapeAction { get; private set; }
        public static PAction SwitchColorAction { get; private set; }

        private static HashSet<Action> _actionsShouldIgnoreBindingConflicts = new HashSet<Action>();
        private static List<string[]> _stringsToAdd = new List<string[]>();

        /// <summary>
        /// Must be called during UserMod2.OnLoad
        /// </summary>
        public static void SetupPActionsOnLoad(PActionManager actionManager)
        {
            PlanningToolAction = actionManager.CreateAction("planningtool.use_planning_tool", PTStrings.ACTION_PLANNING_TOOL_NAME);
            ClipBoardRotateCCWAction = _createNonConflict(actionManager, "planningtool.clipboard_rotate_ccw",
                PTStrings.PLANNING_TOOL_ACTIVE_BINDINGS.ROTATE_CLIPBOARD_CCW, new PKeyBinding(KKeyCode.Q));
            ClipBoardRotateCWAction = _createNonConflict(actionManager, "planningtool.clipboard_rotate_cw",
                PTStrings.PLANNING_TOOL_ACTIVE_BINDINGS.ROTATE_CLIPBOARD_CW, new PKeyBinding(KKeyCode.E));
            ClipBoardFlipAction = _createNonConflict(actionManager, "planningtool.clipboard_flip",
                PTStrings.PLANNING_TOOL_ACTIVE_BINDINGS.FLIP_CLIPBOARD, new PKeyBinding(KKeyCode.F));
            CopyPlanAction = _createNonConflict(actionManager, "planningtool.copy_plan",
                PTStrings.PLANNING_TOOL_ACTIVE_BINDINGS.COPY_PLAN, new PKeyBinding(KKeyCode.R));
            CutPlanAction = _createNonConflict(actionManager, "planningtool.cut_plan",
                PTStrings.PLANNING_TOOL_ACTIVE_BINDINGS.CUT_PLAN, new PKeyBinding(KKeyCode.T));
            PastePlanAction = _createNonConflict(actionManager, "planningtool.paste_plan",
                PTStrings.PLANNING_TOOL_ACTIVE_BINDINGS.PASTE_PLAN, new PKeyBinding(KKeyCode.V));
            SampleToolAction = _createNonConflict(actionManager, "planningtool.use_sample_tool",
                PTStrings.PLANNING_TOOL_ACTIVE_BINDINGS.SAMPLE_TOOL, new PKeyBinding(KKeyCode.B));
            SwitchShapeAction = _createNonConflict(actionManager, "planningtool.switch_shape",
                PTStrings.PLANNING_TOOL_ACTIVE_BINDINGS.SWITCH_SHAPE, new PKeyBinding(KKeyCode.Alpha1));
            SwitchColorAction = _createNonConflict(actionManager, "planningtool.switch_color",
                PTStrings.PLANNING_TOOL_ACTIVE_BINDINGS.SWITCH_COLOR, new PKeyBinding(KKeyCode.Alpha2));
        }

        private static PAction _createNonConflict(PActionManager actionManager, string identifier, LocString title, PKeyBinding binding = null)
        {
            var pAction = actionManager.CreateAction(identifier, title, binding);
            _actionsShouldIgnoreBindingConflicts.Add(pAction.GetKAction());
            _stringsToAdd.Add(new []{"STRINGS.INPUT_BINDINGS.PLANNING_TOOL_ACTIVE." + pAction.GetKAction(), (string) title});
            return pAction;
        }

        /// <summary>
        /// Sets BindingEntry.mIgnoreRootConflics so that tool specific keybindings can be set by user without conflict.
        /// These keybindings will always take priority when planning tool is active from PlanningToolBrushMenu.
        ///
        /// Should be called after keybindings have been initialized, e.g. after Db.Initialize()
        /// </summary>
        public static void SetActionsIgnoreConflicts()
        {
            // KeyBindings is array of structs, so don't copy to local variable
            for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
            {
                if (_actionsShouldIgnoreBindingConflicts.Contains(GameInputMapping.KeyBindings[i].mAction))
                {
                    GameInputMapping.KeyBindings[i].mIgnoreRootConflics = true;
                    GameInputMapping.KeyBindings[i].mGroup = "PLANNING_TOOL_ACTIVE";
                }
            }
            for (int i = 0; i < GameInputMapping.DefaultBindings.Length; i++)
            {
                if (_actionsShouldIgnoreBindingConflicts.Contains(GameInputMapping.DefaultBindings[i].mAction))
                {
                    GameInputMapping.DefaultBindings[i].mIgnoreRootConflics = true;
                    GameInputMapping.DefaultBindings[i].mGroup = "PLANNING_TOOL_ACTIVE";
                }
            }

        }

        /// <summary>
        /// mIgnoreRootConflics is set to allow the user to use usually conflicting keybindings while having the
        /// planning tool selected and active. However to ensure it won't conflict with other mods (or the planning
        /// tool hotkey itself) it is assigned its own group. This requires some additional strings to be set.
        /// </summary>
        public static void AddStrings()
        {
            Strings.Add("STRINGS.INPUT_BINDINGS.PLANNING_TOOL_ACTIVE.NAME", PTStrings.PLANNING_TOOL_ACTIVE_BINDINGS.GROUP_NAME);
            foreach (var stringToAdd in _stringsToAdd)
            {
                Strings.Add(stringToAdd);
            }
        }
    }
}
