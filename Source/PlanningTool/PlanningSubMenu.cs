using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace PlanningTool
{
    public class PlanningSubMenu
    {
        public static PlanningSubMenu Instance { private set; get; }

        public List<ToolMenu.ToolCollection> PlanTools = new List<ToolMenu.ToolCollection>();

        public PlanningSubMenu()
        {
            // TODO: set up through init function? onspawn or something
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("[PlanningTool] PlanningSubMenu instantiated but singleton instance already exists.");
            }
        }

        public static void DestroyInstance()
        {
            Instance = null;
        }

        public void CreateSubmenuTools()
        {
            PlanTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.DIG.NAME, "icon_action_dig", Action.Dig, "DigTool", STRINGS.UI.TOOLTIPS.DIGBUTTON, false));
            PlanTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.CANCEL.NAME, "icon_action_cancel", Action.BuildingCancel, "CancelTool", STRINGS.UI.TOOLTIPS.CANCELBUTTON, false));
        }

        public void InstantiateCollectionsUI()
        {
            // Same as InstantiateCollectionsUI(IList<ToolMenu.ToolCollection> collections) but without the active tool tracking
            // as we want to have the planning tool active while using the sub menu
            var toolRow = Util.KInstantiateUI(ToolMenu.Instance.prefabToolRow, ToolMenu.Instance.gameObject, true);
            var toolSet = Util.KInstantiateUI(ToolMenu.Instance.sandboxToolSet, toolRow, true);
            foreach (var tc in PlanTools)
            {
                tc.toggle = Util.KInstantiateUI(tc.tools.Count > 1 ? ToolMenu.Instance.collectionIconPrefab : ToolMenu.Instance.sandboxToolIconPrefab, toolSet, true);
                KToggle component = tc.toggle.GetComponent<KToggle>();
                component.soundPlayer.Enabled = false;
                component.onClick += () =>
                {
                    Debug.Log("A digging tool submenu toggle component was clicked on.");
                    /*
                     * if (this.currentlySelectedCollection == tc && tc.tools.Count >= 1)
                         KMonoBehaviour.PlaySound(GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound()));
                       this.ChooseCollection(tc);
                     *
                     */
                    if (ToolMenu.Instance.currentlySelectedCollection == tc && tc.tools.Count >= 1)
                        KMonoBehaviour.PlaySound(GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound()));
                    // ToolMenu.Instance.ChooseCollection(tc);
                    // func signature (ToolMenu.ToolCollection collection, bool autoSelectTool = true)
                    var chooseCollectionMethod = AccessTools.Method(typeof(ToolMenu), "ChooseCollection", new []{typeof(ToolMenu.ToolCollection), typeof(bool)});
                    chooseCollectionMethod.Invoke(ToolMenu.Instance, new object[] { tc, Type.Missing });

                };
                var go = Util.KInstantiateUI(ToolMenu.Instance.Prefab_collectionContainerWindow, toolSet, true);
                go.transform.localScale = Vector3.one;
                go.GetComponentInChildren<LocText>().SetText(tc.text.ToUpper());
                tc.MaskContainer = go.GetComponentInChildren<GridLayoutGroup>().gameObject;
                go.SetActive(false);
                tc.UIMenuDisplay = go;
                foreach (var ti in tc.tools)
                {
                    var gameObject5 = Util.KInstantiateUI(ToolMenu.Instance.sandboxToolIconPrefab, tc.MaskContainer, true);
                    gameObject5.name = ti.text;
                    ti.toggle = gameObject5.GetComponent<KToggle>();
                    if (ti.collection.tools.Count > 1)
                    {
                        RectTransform rectTransform = ti.toggle.gameObject.GetComponentInChildren<SetTextStyleSetting>().rectTransform();
                        if (gameObject5.name.Length > 12)
                        {
                            rectTransform.GetComponent<SetTextStyleSetting>().SetStyle(ToolMenu.Instance.CategoryLabelTextStyle_LeftAlign);
                            rectTransform.anchoredPosition = new Vector2(16f, rectTransform.anchoredPosition.y);
                        }
                    }
                    // ti.toggle.onClick += (System.Action) (() => this.ChooseTool(ti));
                    // ti.toggle.onClick += () => Debug.Log("Tool got onClick event!");
                    var chooseToolMethod = AccessTools.Method(typeof(ToolMenu), "ChooseTool", new []{typeof(ToolMenu.ToolInfo)});
                    ti.toggle.onClick += () => chooseToolMethod.Invoke(ToolMenu.Instance, new object[]{ ti });
                    tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapse(s =>
                    {
                        // private void SetToggleState(KToggle toggle, bool state)
                        var toggle = tc.toggle.GetComponent<KToggle>();
                        toggle.Deselect();
                        toggle.isOn = false;
                        tc.UIMenuDisplay.SetActive(false);
                    });
                }
            }
        }
    }
}
