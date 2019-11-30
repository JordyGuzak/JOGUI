using UnityEngine;

namespace JOGUI
{
    public class Vector2Evaluator : IEvaluateType<Vector2>
    {
        public Vector2 Evaluate(EaseType easeType, Vector2 startValue, Vector2 endValue, float t)
        {
            Vector2 result = Vector2.zero;
            result.x = Ease.Evaluate(easeType, startValue.x, endValue.x, t);
            result.y = Ease.Evaluate(easeType, startValue.y, endValue.y, t);
            return result;
        }
    }
}
