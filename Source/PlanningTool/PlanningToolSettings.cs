using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlanningTool
{
    public enum PlanColor
    {
        Gray, Blue, Green, Red, Cyan, Magenta, Violet, Orange, Yellow, White, Black
    }

    public static class PlanColorExtension
    {
        private static readonly Dictionary<PlanColor, Color> ColorMap = new Dictionary<PlanColor, Color>();

        public static Color AsColor(this PlanColor planColor)
        {
            if (ColorMap.Count == 0)
            {
                ColorMap.Add(PlanColor.Gray, new Color(0.5f, 0.5f, 0.5f));
                ColorMap.Add(PlanColor.Blue, new Color(0.2f, 0.37f, 1f));
                ColorMap.Add(PlanColor.Green, new Color(0.2f, 1.0f, 0.2f));
                ColorMap.Add(PlanColor.Red, new Color(1.0f, 0.2f, 0.2f));
                ColorMap.Add(PlanColor.Cyan, new Color(0.2f, 1f, 0.96f));
                ColorMap.Add(PlanColor.Magenta, new Color(1.0f, 0.2f, 1.0f));
                ColorMap.Add(PlanColor.Violet, new Color(0.57f, 0.2f, 1f));
                ColorMap.Add(PlanColor.Orange, new Color(1f, 0.55f, 0.2f));
                ColorMap.Add(PlanColor.Yellow, new Color(1f, 0.97f, 0.2f));
                ColorMap.Add(PlanColor.White, new Color(1.0f, 1.0f, 1.0f));
                ColorMap.Add(PlanColor.Black, new Color(0.0f, 0.0f, 0.0f));
            }

            var colorExists = ColorMap.TryGetValue(planColor, out var color);
            if (colorExists)
            {
                return color;
            }

            Debug.LogWarning("[PlanningTool] Color with enum value " + planColor + " not recognized, returning default color");
            ColorMap.TryGetValue(PlanColor.Gray, out var gray);
            return gray;
        }
    }

    public enum PlanShape
    {
        Rectangle, Circle, Diamond
    }

    public class PlanningToolSettings
    {
        public static PlanningToolSettings Instance { get; private set; }

        private PlanColor _activeColor = PlanColor.Gray;

        public PlanColor ActiveColor
        {
            get => _activeColor;
            set
            {
                if (value == _activeColor) return;
                _activeColor = value;
                OnActiveColorChange.Signal(value);
            }
        }
        public event Action<PlanColor> OnActiveColorChange;
        private PlanShape _activeShape = PlanShape.Rectangle;

        public PlanShape ActiveShape
        {
            get => _activeShape;
            set
            {
                if (value == _activeShape) return;
                _activeShape = value;
                OnActiveShapeChange.Signal(value);
            }
        }
        public event Action<PlanShape> OnActiveShapeChange;

        private PlanningToolMode _planningMode = PlanningToolMode.DragPlan;

        public PlanningToolMode PlanningMode
        {
            get => _planningMode;
            set
            {
                if (value == _planningMode) return;
                _planningMode = value;
                OnPlanningToolModeChanged.Signal(value);
            }
        }

        public event Action<PlanningToolMode> OnPlanningToolModeChanged;

        public PlanningToolSettings()
        {
            Instance = this;
        }

        public static void DestroyInstance()
        {
            Instance = null;
        }

        public enum PlanningToolMode
        {
            DragPlan, CopyArea, CutArea, PlaceClipboard
        }
    }
}
