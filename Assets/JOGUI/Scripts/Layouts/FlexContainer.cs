using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            public Vector2 Position { get; }
            public int MainAxis { get; }
            public int CrossAxis { get; }

            public Line(int mainAxis, int crossAxis, Vector2 position)
            {
                MainAxis = mainAxis;
                CrossAxis = crossAxis;
                Position = position;
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

        [SerializeField] protected Alignment justifyContent;
        public Alignment JustifyContent
        {
            get => justifyContent;
            set => SetProperty(ref justifyContent, value);
        }
        
        [SerializeField] protected Alignment alignContent;
        public Alignment AlignContent
        {
            get => alignContent;
            set => SetProperty(ref alignContent, value);
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
            var items = new List<RectTransform>();
            var currentLineSize = Vector2.zero;
            var currentLine = CreateNewLine(positionPointer);
            var lines = new List<Line> {currentLine};

            for (var i = 0; i < container.childCount; i++)
            {
                var element = GetOrAddFlexElement((RectTransform)container.GetChild(i));
                var elementRect = element.RectTransform;
                items.Add(elementRect);
                elementRect.anchorMin = Vector2.up;
                elementRect.anchorMax = Vector2.up;
                elementRect.sizeDelta = element.FlexBasis;
                var elementSize = element.FlexBasis;

                if (WrapMode == WrapMode.Wrap && Mathf.Abs(positionPointer[mainAxis]) + elementSize[mainAxis] > currentLineSize[mainAxis])
                {
                    var offset = currentLineSize[crossAxis] + Spacing;
                    positionPointer[crossAxis] += offset * (FlexDirection == FlexDirection.Row ? -1 : 1);
                    positionPointer[mainAxis] = 0;
                    currentLine.Size = currentLineSize;
                    currentLine = CreateNewLine(positionPointer);
                    lines.Add(currentLine);
                }
                
                if (elementSize[crossAxis] > currentLineSize[crossAxis]) 
                    currentLineSize[crossAxis] = elementSize[crossAxis];

                // position flex element
                var pivot = elementRect.pivot;
                elementRect.anchoredPosition = new Vector2(positionPointer.x + elementSize.x * pivot.x, positionPointer.y - elementSize.y * (1f - pivot.y));

                // move position pointer
                positionPointer[mainAxis] += (elementSize[mainAxis] + Spacing) * (FlexDirection == FlexDirection.Row ? 1 : -1);
                currentLine.Add(element);
            }

            Line CreateNewLine(Vector2 position)
            {
                currentLineSize[mainAxis] = container.rect.size[mainAxis];
                currentLineSize[crossAxis] = 0;
                return new Line(mainAxis, crossAxis, position);
            }
            
            currentLine.Size = currentLineSize;

            foreach (var line in lines)
            {
                ApplyFlexGrow(mainAxis, line);
            }
            
            ApplyJustifyContent(mainAxis, container.rect.size, lines);
            ApplyAlignContent(crossAxis, container.rect.size, lines);
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

        private void AlignContentAlongAxis(int axis, Vector2 containerSize, List<Line> lines, Alignment alignment)
        {
            if (lines.Count == 0) return;

            var isMainAxis = axis == lines[0].MainAxis;
            var max = 0f;
            if (isMainAxis)
            {
                var largestLine = lines.OrderByDescending(l => l.UsedSpace[axis]).First();
                max = largestLine.UsedSpace[axis] + (largestLine.Items.Count - 1) * Spacing;
            }
            else
            {
                max = lines.Sum(l => l.UsedSpace[axis]) + (lines.Count - 1) * Spacing;
            }
            
            float offset = 0;
            switch (alignment)
            {
                case Alignment.Start:
                    break;
                case Alignment.Center:
                    offset = (containerSize[axis] - max) * 0.5f;
                    break;
                case Alignment.End:
                    offset = containerSize[axis] - max;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (axis == 1)
                offset *= -1;

            foreach (var item in lines.SelectMany(l => l.Items))
            {
                var delta = Vector2.zero;
                delta[axis] = offset;
                item.RectTransform.anchoredPosition += delta;
            }
        }

        private void ApplyJustifyContent(int mainAxis, Vector2 containerSize, List<Line> lines)
        {
            AlignContentAlongAxis(mainAxis, containerSize, lines, JustifyContent);
        }
        
        private void ApplyAlignContent(int crossAxis, Vector2 containerSize, List<Line> lines)
        {
            AlignContentAlongAxis(crossAxis, containerSize, lines, AlignContent);
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