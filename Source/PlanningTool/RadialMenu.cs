using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace PlanningTool
{
    public class RadialMenu : MonoBehaviour
    {
        public bool IsStarted { get; private set; }
        public Color DefaultColor { get; set; } = new Color(0.5f, 0.5f, 0.5f);
        public Color DefaultHighlightColor { get; set; } = new Color(0.8f, 0.8f, 0.8f);
        public float WorldPositionZ = Grid.GetLayerZ(Grid.SceneLayer.SceneMAX) - 20f;

        private List<RadialMenuOption> _options = new List<RadialMenuOption>();

        private List<RadialMenuHelper.OptionComponents> _optionComponentsList =
            new List<RadialMenuHelper.OptionComponents>();
        private Action<int> _callback;
        private Camera _mainCamera;
        private Vector3 _mousePosOnStart;
        private int _currentHoverIndex;

        public void StartMenu(List<RadialMenuOption> options, Action<int> callback, bool usePreviousPosition = false)
        {
            if (options == null || options.Count == 0)
            {
                throw new ArgumentException(options == null ? "options was null" : "options was empty", nameof(options));
            }

            if (IsStarted)
                Debug.LogWarning("[PlanningTool] RadialMenu.StartMenu called while already having menu open, this is probably an error!");

            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning("[PlanningTool] Camera.main not defined but RadialMenu.StartMenu called.");
                return;
            }

            _mainCamera = mainCamera;
            if (!usePreviousPosition)
            {
                _mousePosOnStart = KInputManager.GetMousePos();
            }
            var mousePos = _mainCamera.ScreenToWorldPoint(_mousePosOnStart);
            mousePos.z = WorldPositionZ;
            transform.position = mousePos;
            IsStarted = true;
            _optionComponentsList.Clear();
            _options.Clear();
            _options.AddRange(options);
            _callback = callback;
            _createMenu();
            _currentHoverIndex = 0;
            _setHighlightColor(_currentHoverIndex, true);
        }

        public void CancelMenu()
        {
            KMonoBehaviour.PlaySound(GlobalAssets.GetSound("hud_click_deselect"));
            IsStarted = false;
            _destroyMenu();
        }

        public void MakeSelection()
        {
            KMonoBehaviour.PlaySound(GlobalAssets.GetSound("hud_click"));
            _callback?.Invoke(_currentHoverIndex);
            IsStarted = false;
            _destroyMenu();
        }

        private void _createMenu()
        {
            _optionComponentsList = RadialMenuHelper.GenerateOptionsFor(gameObject, _options, DefaultColor);
        }

        private void _destroyMenu()
        {
            _options.Clear();
            _callback = null;
            _currentHoverIndex = 0;
            foreach (var optionComponent in _optionComponentsList)
            {
                Destroy(optionComponent.GameObject);
                Destroy(optionComponent.IconOrText);
            }
            _optionComponentsList.Clear();
        }

        private void Update()
        {
            if (!IsStarted)
                return;

            // update transform and scale to match camera
            var zoom = _mainCamera.orthographicSize * 0.25f;
            transform.localScale = new Vector3(zoom, zoom, zoom);
            var positionRelativeToCamera = _mainCamera.ScreenToWorldPoint(_mousePosOnStart);
            positionRelativeToCamera.z = WorldPositionZ;
            transform.position = positionRelativeToCamera;

            var mousePos = _mainCamera.ScreenToWorldPoint(KInputManager.GetMousePos());
            mousePos.z = WorldPositionZ;
            var posToMouse = mousePos - gameObject.transform.position;
            if (posToMouse.magnitude < 0.02f)
                return;
            var toMouseAngle = _calculateToMouseAngle(posToMouse);
            var anglePerOption = 360f / _options.Count;
            var currentIndex = Mathf.FloorToInt(toMouseAngle / anglePerOption);
            if (currentIndex != _currentHoverIndex)
            {
                _setHighlightColor(_currentHoverIndex, false);
                _setHighlightColor(currentIndex, true);

                _currentHoverIndex = currentIndex;
                KMonoBehaviour.PlaySound(GlobalAssets.GetSound("hud_mouseover"));
            }
        }

        private void _setHighlightColor(int optionIndex, bool setHighlight)
        {
            if (setHighlight)
            {
                if (_options[optionIndex].HighlightColor.HasValue)
                    _optionComponentsList[optionIndex].MeshRenderer.material.color =
                        _options[optionIndex].HighlightColor.Value;
                else
                    _optionComponentsList[optionIndex].MeshRenderer.material.color =
                        DefaultHighlightColor;
            }
            else
            {
                if (_options[optionIndex].Color.HasValue)
                    _optionComponentsList[optionIndex].MeshRenderer.material.color =
                        _options[optionIndex].Color.Value;
                else
                    _optionComponentsList[optionIndex].MeshRenderer.material.color =
                        DefaultColor;
            }
        }

        private static float _calculateToMouseAngle(Vector3 centerToMouse)
        {
            var angle = Mathf.Atan2(centerToMouse.x, centerToMouse.y); // notice order of params, angle w.r.t. y axis
            angle = Mathf.Clamp(angle * Mathf.Rad2Deg, -180f, 180f);
            if (angle < 0f)
                angle += 360f;

            return angle;
        }
    }

    public class RadialMenuOption
    {
        [CanBeNull] public string Label;
        public Color? Color;
        public Color? HighlightColor;
        [CanBeNull] public Sprite Icon;
        public bool WasSelected;
    }
}
