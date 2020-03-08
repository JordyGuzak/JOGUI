using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;
using JOGUI.Extensions;

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
        End
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
        Stretch
    }

    public enum Alignment
    {
        Start,
        Center,
        End
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

        [SerializeField] protected WrapMode wrapMode;
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
            var container = RectTransform;
            var mainAxis = FlexDirection == FlexDirection.Row ? 0 : 1;
            var crossAxis = mainAxis == 0 ? 1 : 0;
            var positionPointer = Vector2.zero;
            var currentLine = CreateNewLine(positionPointer);
            var lines = new List<Line> {currentLine};

            for (var i = 0; i < container.childCount; i++)
            {
                var element = GetOrAddFlexElement((RectTransform)container.GetChild(i));
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
                elementRect.anchoredPosition = new Vector2(positionPointer.x + elementSize.x * pivot.x, positionPointer.y - elementSize.y * (1f - pivot.y));

                // move position pointer
                positionPointer[mainAxis] += (elementSize[mainAxis] + Spacing) * (FlexDirection == FlexDirection.Row ? 1 : -1);
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

            ApplyAlignContent(crossAxis, container.rect.size, lines);
            ApplyJustifyContent(mainAxis, container.rect.size, lines);
            ApplyAlignItems(crossAxis, lines);
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
            var spacing = (line.Items.Count - 1) * Spacing;
            var availableSpace = line.Size[mainAxis] - line.UsedSpace[mainAxis] - spacing;
            if (availableSpace < 0.05f) return;
            var itemsWithGrow = line.Items.Where(i => i.FlexGrow > 0).ToArray();
            if (itemsWithGrow.Length == 0) return;
            var sections = itemsWithGrow.Sum(i => i.FlexGrow);
            var spacePerSection = availableSpace / sections;

            foreach (var item in itemsWithGrow)
            {
                if (item.FlexGrow <= 0) continue;
                
                var delta = spacePerSection * item.FlexGrow;
                var size = item.FlexBasis;
                size[mainAxis] += delta;
                item.RectTransform.sizeDelta = size;
            }

            var positionPointer = line.Position;
            foreach (var item in line.Items)
            {
                var itemRect = item.RectTransform;
                var elementSize = itemRect.rect.size;
                var pivot = itemRect.pivot;
                itemRect.anchoredPosition = new Vector2(positionPointer.x + elementSize.x * pivot.x, positionPointer.y - elementSize.y * (1f - pivot.y));
                positionPointer[mainAxis] += (elementSize[mainAxis] + Spacing) * (FlexDirection == FlexDirection.Row ? 1 : -1);
            }

            var lineSize = line.Size;
            lineSize[mainAxis] -= spacing;
            line.UsedSpace = lineSize;
        }

        private void ApplyJustifyContent(int mainAxis, Vector2 containerSize, List<Line> lines)
        {
            if (lines.Count == 0) return;
            
            var largestLine = lines.OrderByDescending(l => l.UsedSpace[mainAxis]).First();
            var max = largestLine.UsedSpace[mainAxis] + (largestLine.Items.Count - 1) * Spacing;
            
            float offset = 0;
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
            var positionPointer = Vector2.zero;
            var contentSize = Vector2.zero;
            var offsetMultiplier = 0f;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                var lineSize = 0f;
                switch (AlignContent)
                {
                    case AlignContent.Start:
                        lineSize = line.Items.Max(item => item.RectTransform.rect.size[crossAxis]);
                        offsetMultiplier = 0f;
                        break;
                    case AlignContent.Center:
                        lineSize = line.Items.Max(item => item.RectTransform.rect.size[crossAxis]);
                        offsetMultiplier = 0.5f;
                        break;
                    case AlignContent.End:
                        lineSize = line.Items.Max(item => item.RectTransform.rect.size[crossAxis]);
                        offsetMultiplier = 1f;
                        break;
                    case AlignContent.Stretch:
                        lineSize = containerSize[crossAxis] / lines.Count;
                        break;
                }
                
                var size = line.Size;
                size[crossAxis] = lineSize;
                line.Size = size;
                contentSize[crossAxis] += lineSize;
                
                positionPointer[crossAxis] += lineSize * i + (i > 0 ? Spacing : 0);
                line.Position = positionPointer;
            }

            var offset = (containerSize[crossAxis] - contentSize[crossAxis]) * offsetMultiplier;
            
            foreach (var line in lines)
            {
                var position = line.Position;
                position[crossAxis] += offset;
                line.Position = position;
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
                            size[crossAxis] = line.Size[crossAxis] - (i > 0 ? Spacing : 0);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    item.RectTransform.sizeDelta = size;
                    var offset = (line.Size[crossAxis] - size[crossAxis]) * offsetMultiplier;
                    var pivot = item.RectTransform.pivot;
                    var anchoredPosition = item.RectTransform.anchoredPosition;
                    anchoredPosition[crossAxis] = (line.Position[crossAxis] + size[crossAxis] * pivot[crossAxis]) * (crossAxis == 0 ? 1 : -1);
                    anchoredPosition[crossAxis] -= offset * (crossAxis == 1 ? 1 : -1);
                    item.RectTransform.anchoredPosition = anchoredPosition;
                }
            }
        }

        public void SetDirty()
        {
            _isDirty = true;
        }
        
        protected void SetProperty<T>(ref T currentValue, T newValue)
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
            StartCoroutine(DelayedSetDirty());
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
        
        IEnumerator DelayedSetDirty()
        {
            yield return null;
            SetDirty();
        }
    }
}