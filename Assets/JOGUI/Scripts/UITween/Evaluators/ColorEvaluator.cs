using UnityEngine;

namespace JOGUI
{
    public class ColorEvaluator : IEvaluateType<Color>
    {
        public Color GetChangeValue(Color startValue, Color endValue)
        {
            return endValue - startValue;
        }

        public Color Evaluate(Color startValue, Color changeValue, float t)
        {
            return startValue + changeValue * t;
        }
    }
}
