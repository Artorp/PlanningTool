using System.Collections.Generic;

namespace PlanningTool
{
    /// <summary>
    /// Representation of a collection of plan entities, with color and shape, and an offset to the origin
    /// </summary>
    public class PlanClipboard
    {
        private List<PlanClipboardElement> _clipboardContent = new List<PlanClipboardElement>();

        public void AddPlan(SaveLoadPlans.PlanData planData, int originX, int originY)
        {
            Grid.CellToXY(planData.Cell, out var x, out var y);
            var offsetX = x - originX;
            var offsetY = y - originY;
            var plan = new PlanClipboardElement
            {
                OffsetX = offsetX,
                OffsetY = offsetY,
                Shape = planData.Shape,
                Color = planData.Color
            };
            _clipboardContent.Add(plan);
        }

        public void Clear()
        {
            _clipboardContent.Clear();
        }

        public bool HasData()
        {
            return _clipboardContent.Count != 0;
        }

        public IEnumerable<PlanClipboardElement> Elements()
        {
            foreach (var element in _clipboardContent)
            {
                yield return element;
            }
        }

        public void Rotate(bool clockwise)
        {
            // modify offsetX and offsetY of each element to rotate
            var rotated = new List<PlanClipboardElement>(_clipboardContent.Count);

            foreach (var element in _clipboardContent)
            {
                var newElement = new PlanClipboardElement()
                {
                    Color = element.Color,
                    Shape = element.Shape
                };
                if (clockwise)
                {
                    newElement.OffsetX = element.OffsetY;
                    newElement.OffsetY = -element.OffsetX;
                }
                else
                {
                    newElement.OffsetX = -element.OffsetY;
                    newElement.OffsetY = element.OffsetX;
                }
                rotated.Add(newElement);
            }

            _clipboardContent.Clear();
            _clipboardContent.AddRange(rotated);
        }

        public void Flip(bool horizontally)
        {
            var flipped = new List<PlanClipboardElement>(_clipboardContent.Count);

            foreach (var element in _clipboardContent)
            {
                var newElement = new PlanClipboardElement()
                {
                    Color = element.Color,
                    Shape = element.Shape
                };
                if (horizontally)
                {
                    newElement.OffsetX = -element.OffsetX;
                    newElement.OffsetY = element.OffsetY;
                }
                else
                {
                    newElement.OffsetX = element.OffsetX;
                    newElement.OffsetY = -element.OffsetY;
                }
                flipped.Add(newElement);
            }

            _clipboardContent.Clear();
            _clipboardContent.AddRange(flipped);
        }
    }

    public class PlanClipboardElement
    {
        public int OffsetX;
        public int OffsetY;
        public PlanShape Shape;
        public PlanColor Color;
    }
}
