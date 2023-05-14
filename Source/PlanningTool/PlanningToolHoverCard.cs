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
                    Element element = Grid.Element[cell];
                    if (Grid.Solid[cell] && Diggable.IsDiggable(cell))
                    {
                        // should probably remove or adjust this
                        drawer.NewLine();
                        drawer.DrawText(element.nameUpperCase, Styles_Title.Standard);
                        drawer.NewLine();
                        drawer.DrawIcon(instance.GetSprite("dash"));
                        drawer.DrawText(element.GetMaterialCategoryTag().ProperName(), Styles_BodyText.Standard);
                        drawer.NewLine();
                        drawer.DrawIcon(instance.GetSprite("dash"));
                        string[] strArray = HoverTextHelper.MassStringsReadOnly(cell);
                        drawer.DrawText(strArray[0], Styles_Values.Property.Standard);
                        drawer.DrawText(strArray[1], Styles_Values.Property_Decimal.Standard);
                        drawer.DrawText(strArray[2], Styles_Values.Property.Standard);
                        drawer.DrawText(strArray[3], Styles_Values.Property.Standard);
                        drawer.NewLine();
                        drawer.DrawIcon(instance.GetSprite("dash"));
                        drawer.DrawText(GameUtil.GetHardnessString(Grid.Element[cell]), Styles_BodyText.Standard);
                    }
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
