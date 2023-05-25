using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

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

        /// <summary>
        /// Make sure the origin/pivot of the clipboard is within the bounding box of all plans.
        /// </summary>
        public void AdjustOffsetsToElements()
        {
            if (_clipboardContent.Count == 0) return;
            // Find the min and max of x and y offsets
            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var minY = int.MaxValue;
            var maxY = int.MinValue;
            foreach (var element in _clipboardContent)
            {
                if (element.OffsetX < minX) minX = element.OffsetX;
                if (element.OffsetX > maxX) maxX = element.OffsetX;
                if (element.OffsetY < minY) minY = element.OffsetY;
                if (element.OffsetY > maxY) maxY = element.OffsetY;
            }

            // Check if the origin is within the bounding box
            if (minX <= 0 && maxX >= 0 && minY <= 0 && maxY >= 0) return; // No need to adjust

            // Calculate the delta to move the origin
            var deltaX = 0;
            var deltaY = 0;
            if (minX > 0) deltaX = -minX; // Move left
            else if (maxX < 0) deltaX = -maxX; // Move right
            if (minY > 0) deltaY = -minY; // Move down
            else if (maxY < 0) deltaY = -maxY; // Move up

            // Adjust the offsets by the delta
            foreach (var element in _clipboardContent)
            {
                element.OffsetX += deltaX;
                element.OffsetY += deltaY;
            }
        }

        public void ExportToSystemClipboard()
        {
            var jsonContent = JsonConvert.SerializeObject(_clipboardContent);
            GUIUtility.systemCopyBuffer = jsonContent;
        }

        public void ImportFromSystemClipboard()
        {
            var jsonContent = GUIUtility.systemCopyBuffer;
            var errors = new List<string>();
            var output = JsonConvert.DeserializeObject<List<PlanClipboardElement>>(jsonContent, new JsonSerializerSettings()
            {
                Error = (sender, args) =>
                {
                    errors.Add(args.ErrorContext.Error.Message);
                    args.ErrorContext.Handled = true;
                }
            });
            if (errors.Count != 0)
            {
                throw new DeserializationException("Deserialization failed", errors, jsonContent);
            }
            _clipboardContent.Clear();
            _clipboardContent.AddRange(output);
        }
    }

    public class PlanClipboardElement
    {
        public int OffsetX;
        public int OffsetY;
        [JsonConverter(typeof(StringEnumConverter))]
        public PlanShape Shape;
        [JsonConverter(typeof(StringEnumConverter))]
        public PlanColor Color;
    }

    public class DeserializationException : Exception
    {
        public List<string> Errors { get; }
        public string StringToDeserialize { get; }

        public DeserializationException(string message, List<string> errors, string stringToSerialize) : base(message)
        {
            Errors = errors;
            StringToDeserialize = stringToSerialize;
        }
    }
}
