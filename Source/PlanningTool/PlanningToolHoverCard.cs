using System.Collections.Generic;
using UnityEngine;

namespace PlanningTool
{
    public class PlanningToolHoverCard : HoverTextConfiguration
    {

        public override void UpdateHoverElements(List<KSelectable> selected)
        {
            // from DigToolHoverTextCard.UpdateHoverElements
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
    }
}
