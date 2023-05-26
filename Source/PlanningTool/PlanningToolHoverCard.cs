using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlanningTool
{
    public class PlanningToolHoverCard : HoverTextConfiguration
    {

        public void UpdateToolName()
        {
            switch (PlanningToolSettings.Instance.PlanningMode)
            {
                case PlanningToolSettings.PlanningToolMode.DragPlan:
                    ToolName = PTStrings.PLANNING_TOOL_NAME;
                    ActionName = PTStrings.HOVER_CARD.PLANNING_ACTION_NAME;
                    break;
                case PlanningToolSettings.PlanningToolMode.CopyArea:
                    ToolName = PTStrings.BRUSH_MENU.COPY_BUTTON_LABEL;
                    ActionName = PTStrings.HOVER_CARD.COPY_ACTION_NAME;
                    break;
                case PlanningToolSettings.PlanningToolMode.CutArea:
                    ToolName = PTStrings.BRUSH_MENU.CUT_BUTTON_LABEL;
                    ActionName = PTStrings.HOVER_CARD.CUT_ACTION_NAME;
                    break;
                case PlanningToolSettings.PlanningToolMode.PlaceClipboard:
                    ToolName = PTStrings.BRUSH_MENU.PASTE_BUTTON_LABEL;
                    ActionName = PTStrings.HOVER_CARD.PASTE_ACTION_NAME;
                    break;
                case PlanningToolSettings.PlanningToolMode.SamplePlan:
                    ToolName = PTStrings.BRUSH_MENU.SAMPLE_BUTTON_LABEL;
                    ActionName = PTStrings.HOVER_CARD.SAMPLE_ACTION_NAME;
                    break;
            }
        }

        public override void UpdateHoverElements(List<KSelectable> selected)
        {
            // from DigToolHoverTextCard.UpdateHoverElements
            UpdateToolName();
            HoverTextScreen instance = HoverTextScreen.Instance;
            HoverTextDrawer drawer = instance.BeginDrawing();
            int cell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
            if (!Grid.IsValidCell(cell) || Grid.WorldIdx[cell] != ClusterManager.Instance.activeWorldId)
            {
                drawer.EndDrawing();
            }
            else
            {
                drawer.BeginShadowBar();
                if (Grid.IsVisible(cell))
                {
                    DrawTitle(instance, drawer);
                    DrawInstructions(HoverTextScreen.Instance, drawer);
                    DrawToolHotkeys(drawer);
                }
                else
                {
                    drawer.DrawIcon(instance.GetSprite("iconWarning"));
                    drawer.DrawText(STRINGS.UI.TOOLS.GENERIC.UNKNOWN, Styles_BodyText.Standard);
                }

                drawer.EndShadowBar();
                drawer.EndDrawing();
            }
        }

        public void DrawToolHotkeys(HoverTextDrawer drawer)
        {
            var standard = Styles_Instruction.Standard;
            if (PlanningToolSettings.Instance.PlanningMode == PlanningToolSettings.PlanningToolMode.DragPlan)
            {
                drawer.NewLine(34);
                drawer.DrawText(GameUtil.ReplaceHotkeyString(PTStrings.HOVER_CARD.COPY_HOTKEY, ToolKeyBindings.CopyPlanAction.GetKAction()), standard);
                drawer.NewLine();
                drawer.DrawText(GameUtil.ReplaceHotkeyString(PTStrings.HOVER_CARD.CUT_HOTKEY, ToolKeyBindings.CutPlanAction.GetKAction()), standard);
                drawer.NewLine();
                drawer.DrawText(GameUtil.ReplaceHotkeyString(PTStrings.HOVER_CARD.PASTE_HOTKEY, ToolKeyBindings.PastePlanAction.GetKAction()), standard);
                drawer.NewLine();
                drawer.DrawText(GameUtil.ReplaceHotkeyString(PTStrings.HOVER_CARD.SAMPLE_HOTKEY, ToolKeyBindings.SampleToolAction.GetKAction()), standard);
                drawer.NewLine();
                drawer.DrawText(GameUtil.ReplaceHotkeyString(PTStrings.HOVER_CARD.SWITCH_SHAPE_HOTKEY, ToolKeyBindings.SwitchShapeAction.GetKAction()), standard);
                drawer.NewLine();
                drawer.DrawText(GameUtil.ReplaceHotkeyString(PTStrings.HOVER_CARD.SWITCH_COLOR_HOTKEY, ToolKeyBindings.SwitchColorAction.GetKAction()), standard);
            } else if (PlanningToolSettings.Instance.PlanningMode ==
                       PlanningToolSettings.PlanningToolMode.PlaceClipboard)
            {
                drawer.NewLine(34);
                drawer.DrawText(GameUtil.ReplaceHotkeyString(PTStrings.HOVER_CARD.ROTATE_LEFT_HOTKEY, ToolKeyBindings.ClipBoardRotateCCWAction.GetKAction()), standard);
                drawer.NewLine();
                drawer.DrawText(GameUtil.ReplaceHotkeyString(PTStrings.HOVER_CARD.ROTATE_RIGHT_HOTKEY, ToolKeyBindings.ClipBoardRotateCWAction.GetKAction()), standard);
                drawer.NewLine();
                drawer.DrawText(GameUtil.ReplaceHotkeyString(PTStrings.HOVER_CARD.FLIP_HORIZONTALLY_HOTKEY, ToolKeyBindings.ClipBoardFlipAction.GetKAction()), standard);
            }
        }
    }
}
