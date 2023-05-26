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

        public static LocString AsLocString(this PlanColor planColor)
        {
            if (planColor == PlanColor.Gray)
                return PTStrings.COLORS.GRAY;
            if (planColor == PlanColor.Blue)
                return PTStrings.COLORS.BLUE;
            if (planColor == PlanColor.Green)
                return PTStrings.COLORS.GREEN;
            if (planColor == PlanColor.Red)
                return PTStrings.COLORS.RED;
            if (planColor == PlanColor.Cyan)
                return PTStrings.COLORS.CYAN;
            if (planColor == PlanColor.Magenta)
                return PTStrings.COLORS.MAGENTA;
            if (planColor == PlanColor.Violet)
                return PTStrings.COLORS.VIOLET;
            if (planColor == PlanColor.Orange)
                return PTStrings.COLORS.ORANGE;
            if (planColor == PlanColor.Yellow)
                return PTStrings.COLORS.YELLOW;
            if (planColor == PlanColor.White)
                return PTStrings.COLORS.WHITE;
            if (planColor == PlanColor.Black)
                return PTStrings.COLORS.BLACK;
            Debug.LogWarning("[PlanningTool] Color with enum value " + planColor + " not recognized, returning default text");
            return PTStrings.COLORS.GRAY;
        }
    }

    public enum PlanShape
    {
        Rectangle, Circle, Diamond
    }

    public static class PlanShapeExtension
    {
        public static LocString AsLocString(this PlanShape planShape)
        {
            if (planShape == PlanShape.Rectangle)
                return PTStrings.SHAPES.RECTANGLE;
            if (planShape == PlanShape.Circle)
                return PTStrings.SHAPES.CIRCLE;
            if (planShape == PlanShape.Diamond)
                return PTStrings.SHAPES.DIAMOND;
            Debug.LogWarning("[PlanningTool] Shape with enum value " + planShape + " not recognized, returning default text");
            return PTStrings.SHAPES.RECTANGLE;
        }
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
            DragPlan, CopyArea, CutArea, SamplePlan, PlaceClipboard
        }
    }
}
