using UnityEngine;

namespace JOGUI
{
    public class ColorEvaluator : IEvaluateType<Color>
    {
        public Color Evaluate(EaseType easeType, Color startValue, Color endValue, float t)
        {
            var result = Color.black;
            result.r = Ease.Evaluate(easeType, startValue.r, endValue.r, t);
            result.g = Ease.Evaluate(easeType, startValue.g, endValue.g, t);
            result.b = Ease.Evaluate(easeType, startValue.b, endValue.b, t);
            result.a = Ease.Evaluate(easeType, startValue.a, endValue.a, t);
            return result;
        }
    }
}
