﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JOGUI
{
    public enum FlexDirection
    {
        Row,
        Column
    }

    public enum WrapMode
    {
        Wrap,
        NoWrap
    }
    
    public enum JustifyContent
    {
        Start,
        Center,
        End,
    }

    public enum AlignItems
    {
        Start,
        Center,
        End,
        Stretch
    }

    public enum AlignContent
    {
        Start,
        Center,
        End,
        Stretch,
        SpaceBetween
    }

    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class FlexContainer : UIBehaviour //reference: https://css-tricks.com/snippets/css/a-guide-to-flexbox/
    {
        class Line
        {
            public List<FlexElement> Items { get; set; } = new List<FlexElement>();
            public Vector2 Size { get; set; }
            public Vector2 UsedSpace { get; set; }

            public Vector2 Position { get; set; }
            public int MainAxis { get; }
            public int CrossAxis { get; }

            public Line(int mainAxis, int crossAxis, Vector2 position, Vector2 size)
            {
                MainAxis = mainAxis;
                CrossAxis = crossAxis;
                Position = position;
                Size = size;
            }

            public void Add(FlexElement element)
            {
                var usedSpace = UsedSpace;
                usedSpace[MainAxis] += element.RectTransform.rect.size[MainAxis];

                if (element.RectTransform.rect.size[CrossAxis] > usedSpace[CrossAxis])
                    usedSpace[CrossAxis] = element.RectTransform.rect.size[CrossAxis];

                UsedSpace = usedSpace;
                Items.Add(element);
            }
        }
        
        [SerializeField] protected FlexDirection flexDirection;
        public FlexDirection FlexDirection
        {
            get => flexDirection;
            set => SetProperty(ref flexDirection, value);
        }

        [SerializeField] protected WrapMode wrapMode = WrapMode.NoWrap;
        public WrapMode WrapMode
        {
            get => wrapMode;
            set => SetProperty(ref wrapMode, value);
        }

        [SerializeField] protected JustifyContent justifyContent;
        public JustifyContent JustifyContent
        {
            get => justifyContent;
            set => SetProperty(ref justifyContent, value);
        }
        
        [SerializeField] protected AlignContent alignContent;
        public AlignContent AlignContent
        {
            get => alignContent;
            set => SetProperty(ref alignContent, value);
        }
        
        [SerializeField] protected AlignItems alignItems;
        public AlignItems AlignItems
        {
            get => alignItems;
            set => SetProperty(ref alignItems, value);
        }

        [SerializeField] protected float _spacing;
        public float Spacing
        {
            get => _spacing;
            set => SetProperty(ref _spacing, value);
        }

        [SerializeField] protected RectOffset _padding;
        public RectOffset Padding
        {
            get => _padding;
            set => SetProperty(ref _padding, value);
        }
        
        [SerializeField] protected bool fitHorizontal;
        public bool FitHorizontal
        {
            get => fitHorizontal;
            set => SetProperty(ref fitHorizontal, value);
        }
        
        [SerializeField] protected bool fitVertical;
        public bool FitVertical
        {
            get => fitVertical;
            set => SetProperty(ref fitVertical, value);
        }

        private RectTransform _rectTransform;
        private RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }

        private bool _isDirty;

        private void LateUpdate()
        {
            if (!_isDirty) return;
            SetLayout();
        }

        private void SetLayout()
        {
            if (RectTransform.childCount == 0) return;
            
            var container = RectTransform;
            var mainAxis = FlexDirection == FlexDirection.Row ? 0 : 1;
            var crossAxis = mainAxis == 0 ? 1 : 0;
            var positionPointer = Vector2.zero;
            var currentLine = CreateNewLine(positionPointer);
            var lines = new List<Line> {currentLine};

            for (var i = 0; i < container.childCount; i++)
            {
                var child = (RectTransform) container.GetChild(i);
                if (!child.gameObject.activeInHierarchy) continue;
                var element = GetOrAddFlexElement(child);
                var elementRect = element.RectTransform;
                elementRect.anchorMin = Vector2.up;
                elementRect.anchorMax = Vector2.up;
                elementRect.sizeDelta = element.FlexBasis;
                var elementSize = element.FlexBasis;

                if (WrapMode == WrapMode.Wrap && Mathf.Abs(positionPointer[mainAxis]) + elementSize[mainAxis] > container.rect.size[mainAxis])
                {
                    positionPointer[mainAxis] = 0;
                    currentLine = CreateNewLine(positionPointer);
                    lines.Add(currentLine);
                }

                // position flex element
                var pivot = elementRect.pivot;
                elementRect.anchoredPosition = new Vector2(positionPointer.x + elementSize.x * pivot.x + (mainAxis == 0 ? Padding.left : 0), positionPointer.y - elementSize.y * (1f - pivot.y));

                // move position pointer
                positionPointer[mainAxis] += (elementSize[mainAxis] + Spacing) * (mainAxis == 0 ? 1 : -1);
                currentLine.Add(element);
            }

            Line CreateNewLine(Vector2 position)
            {
                var size = container.rect.size;
                size[crossAxis] = 0;
                return new Line(mainAxis, crossAxis, position, size);
            }
            
            foreach (var line in lines)
            {
                ApplyFlexGrow(mainAxis, line);
            }

            ApplyJustifyContent(mainAxis, container.rect.size, lines);
            ApplyAlignContent(crossAxis, container.rect.size, lines);
            ApplyAlignItems(crossAxis, lines);
            if(FitHorizontal) ApplyFitContent(0, container, lines);
            if(FitVertical) ApplyFitContent(1, container, lines);
            _isDirty = false;
        }
        
        private FlexElement GetOrAddFlexElement(RectTransform child)
        {
            if (child.TryGetComponent(out FlexElement element))
            {
                return element;
            }

            element = child.gameObject.AddComponent<FlexElement>();
            element.FlexBasis = child.rect.size;
            return element;
        }

        private void ApplyFlexGrow(int mainAxis, Line line)
        {
            var itemsWithGrow = line.Items.Where(i => i.FlexGrow > 0).ToArray();
            if (itemsWithGrow.Length == 0) return;
            var spacing = (line.Items.Count - 1) * Spacing;
            var availableSpace = line.Size[mainAxis] - line.UsedSpace[mainAxis] - spacing;
            if (availableSpace < 0.05f) return;
            var sections = itemsWithGrow.Sum(i => i.FlexGrow);
            var spacePerSection = availableSpace / sections;

            foreach (var item in itemsWithGrow)
            {
                var size = item.FlexBasis;
                size[mainAxis] += spacePerSection * item.FlexGrow;
                item.RectTransform.sizeDelta = size;
            }

            var positionPointer = line.Position;
            foreach (var item in line.Items)
            {
                var itemRect = item.RectTransform;
                var elementSize = itemRect.rect.size;
                var pivot = itemRect.pivot;
                itemRect.anchoredPosition = new Vector2(positionPointer.x + elementSize.x * pivot.x, positionPointer.y - elementSize.y * (1f - pivot.y));
                positionPointer[mainAxis] += (elementSize[mainAxis] + Spacing) * (mainAxis == 0 ? 1 : -1);
            }

            var usedSpace = line.UsedSpace;
            usedSpace[mainAxis] = line.Size[mainAxis] - spacing;
            line.UsedSpace = usedSpace;
        }

        private void ApplyJustifyContent(int mainAxis, Vector2 containerSize, List<Line> lines)
        {
            if (lines.Count == 0) return;

            var offset = 0f;

            var largestLine = lines.OrderByDescending(l => l.UsedSpace[mainAxis]).First();
            var max = largestLine.UsedSpace[mainAxis] + (largestLine.Items.Count - 1) * Spacing;

            switch (JustifyContent)
            {
                case JustifyContent.Start:
                    break;
                case JustifyContent.Center:
                    offset = (containerSize[mainAxis] - max) * 0.5f;
                    break;
                case JustifyContent.End:
                    offset = containerSize[mainAxis] - max;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (mainAxis == 1)
                offset *= -1;

            foreach (var item in lines.SelectMany(l => l.Items))
            {
                var delta = Vector2.zero;
                delta[mainAxis] = offset;
                item.RectTransform.anchoredPosition += delta;
            }
        }

        private void ApplyAlignContent(int crossAxis, Vector2 containerSize, List<Line> lines)
        {
            switch (AlignContent)
            {
                case AlignContent.SpaceBetween:
                    ApplyAlignContentSpaceBetween(crossAxis, containerSize, lines);
                    return;
            }
            
            var positionPointer = Vector2.zero;
            var contentSize = Vector2.zero;
            var offsetMultiplier = 0f;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.Items.Count == 0) continue;
                var lineSize = 0f;
                switch (AlignContent)
                {
                    case AlignContent.Start:
                        lineSize = line.UsedSpace[crossAxis];
                        offsetMultiplier = 0f;
                        break;
                    case AlignContent.Center:
                        lineSize = line.UsedSpace[crossAxis];
                        offsetMultiplier = 0.5f;
                        break;
                    case AlignContent.End:
                        lineSize = line.UsedSpace[crossAxis];
                        offsetMultiplier = 1f;
                        break;
                    case AlignContent.Stretch:
                        lineSize = containerSize[crossAxis] / lines.Count - (lines.Count - 1) * Spacing / lines.Count;
                        break;
                }
                
                var size = line.Size;
                size[crossAxis] = lineSize;
                line.Size = size;
                contentSize[crossAxis] += lineSize;

                positionPointer[crossAxis] += i > 0 ? lines[i - 1].Size[crossAxis] + Spacing : 0;
                line.Position = positionPointer;
            }

            var offset = (containerSize[crossAxis] - (contentSize[crossAxis] + (lines.Count - 1) * Spacing)) * offsetMultiplier;
            
            foreach (var line in lines)
            {
                var position = line.Position;
                position[crossAxis] += offset;
                line.Position = position;
            }
        }

        private void ApplyAlignContentSpaceBetween(int crossAxis, Vector2 containerSize, List<Line> lines)
        {
            var usedSpace = lines.Sum(l => l.UsedSpace[crossAxis]);
            var availableSpace = containerSize[crossAxis] - usedSpace;
            var offset = availableSpace / (lines.Count - 1);
            var positionPointer = Vector2.zero;
            
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                var size = line.Size;
                size[crossAxis] = line.UsedSpace[crossAxis];
                line.Size = size;

                if (i > 0)
                {
                    positionPointer[crossAxis] += lines[i - 1].Size[crossAxis] + offset;
                }

                line.Position = positionPointer;
            }
        }

        private void ApplyAlignItems(int crossAxis, List<Line> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                foreach (var item in line.Items)
                {
                    var size = item.RectTransform.sizeDelta;
                    var offsetMultiplier = 0f;
                    switch (AlignItems)
                    {
                        case AlignItems.Start:
                            offsetMultiplier = 0f;
                            break;
                        case AlignItems.Center:
                            offsetMultiplier = 0.5f;
                            break;
                        case AlignItems.End:
                            offsetMultiplier = 1.0f;
                            break;
                        case AlignItems.Stretch:
                            size[crossAxis] = line.Size[crossAxis];// - (i > 0 ? Spacing : 0);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    item.RectTransform.sizeDelta = size;
                    var offset = (line.Size[crossAxis] - size[crossAxis]) * offsetMultiplier;
                    var pivot = item.RectTransform.pivot;
                    var pivotVal = crossAxis == 0 ? pivot[crossAxis] : (1f - pivot[crossAxis]);
                    var anchoredPosition = item.RectTransform.anchoredPosition;
                    anchoredPosition[crossAxis] = (line.Position[crossAxis] + offset + size[crossAxis] * pivotVal) * (crossAxis == 0 ? 1 : -1);
                    item.RectTransform.anchoredPosition = anchoredPosition;
                }
            }
        }

        private void ApplyFitContent(int axis, RectTransform container, List<Line> lines)
        {
            if (lines.Count == 0) return;

            var requiredSize = axis == lines[0].MainAxis 
                ? lines.Max(l => l.UsedSpace[axis] + Padding.left + Padding.right +(l.Items.Count - 1) * Spacing) 
                : lines.Sum(l => l.UsedSpace[axis]) + Padding.top + Padding.bottom + (lines.Count - 1) * Spacing;

            if (container.TryGetComponent<FlexElement>(out var flexElement))
            {
                var basis = flexElement.FlexBasis;
                basis[axis] = requiredSize;
                flexElement.FlexBasis = basis;
            }
            else if (container.TryGetComponent<LayoutElement>(out var layoutElement))
            {
                if (axis == 0)
                    layoutElement.preferredWidth = requiredSize;
                else
                    layoutElement.preferredHeight = requiredSize;
            }
            else
            {
                container.SetSizeWithCurrentAnchors(axis == 0 ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical, requiredSize);
            }
        }

        public void SetDirty()
        {
            _isDirty = true;
        }

        public void ForceRebuildImmediate() => SetLayout();
        
        private void SetProperty<T>(ref T currentValue, T newValue)
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue))) return;
            currentValue = newValue;
            SetDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetDirty();
        }

        protected virtual void OnTransformChildrenChanged()
        {
            SetDirty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }

        protected override void Reset()
        {
            SetDirty();
        }
#endif
    }
}