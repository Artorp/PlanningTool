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
                    ToolName = "Planning";
                    ActionName = "Place plan";
                    break;
                case PlanningToolSettings.PlanningToolMode.CopyArea:
                    ToolName = "Copy";
                    ActionName = "Copy plans";
                    break;
                case PlanningToolSettings.PlanningToolMode.CutArea:
                    ToolName = "Cut";
                    ActionName = "Cut plans";
                    break;
                case PlanningToolSettings.PlanningToolMode.PlaceClipboard:
                    ToolName = "Paste";
                    ActionName = "Paste plans";
                    break;
                case PlanningToolSettings.PlanningToolMode.SamplePlan:
                    ToolName = "Sample";
                    ActionName = "Sample plan";
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
                drawer.DrawText(GameUtil.ReplaceHotkeyString("{Hotkey} - Copy", ToolKeyBindings.CopyPlanAction.GetKAction()), standard);
                drawer.NewLine();
                drawer.DrawText(GameUtil.ReplaceHotkeyString("{Hotkey} - Cut", ToolKeyBindings.CutPlanAction.GetKAction()), standard);
                drawer.NewLine();
                drawer.DrawText(GameUtil.ReplaceHotkeyString("{Hotkey} - Paste", ToolKeyBindings.PastePlanAction.GetKAction()), standard);
                drawer.NewLine();
                drawer.DrawText(GameUtil.ReplaceHotkeyString("{Hotkey} - Sample", ToolKeyBindings.SampleToolAction.GetKAction()), standard);
                drawer.NewLine();
                drawer.DrawText(GameUtil.ReplaceHotkeyString("{Hotkey} - Switch shape", ToolKeyBindings.SwitchShapeAction.GetKAction()), standard);
                drawer.NewLine();
                drawer.DrawText(GameUtil.ReplaceHotkeyString("{Hotkey} - Switch color", ToolKeyBindings.SwitchColorAction.GetKAction()), standard);
            } else if (PlanningToolSettings.Instance.PlanningMode ==
                       PlanningToolSettings.PlanningToolMode.PlaceClipboard)
            {
                drawer.NewLine(34);
                drawer.DrawText(GameUtil.ReplaceHotkeyString("{Hotkey} - Rotate left", ToolKeyBindings.ClipBoardRotateCCWAction.GetKAction()), standard);
                drawer.NewLine();
                drawer.DrawText(GameUtil.ReplaceHotkeyString("{Hotkey} - Rotate right", ToolKeyBindings.ClipBoardRotateCWAction.GetKAction()), standard);
                drawer.NewLine();
                drawer.DrawText(GameUtil.ReplaceHotkeyString("{Hotkey} - Flip horizontally", ToolKeyBindings.ClipBoardFlipAction.GetKAction()), standard);
            }
        }
    }
}
