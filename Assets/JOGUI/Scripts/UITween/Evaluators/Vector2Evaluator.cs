using UnityEngine;

namespace JOGUI
{
    public class Vector2Evaluator : IEvaluateType<Vector2>
    {
        public Vector2 GetChangeValue(Vector2 startValue, Vector2 endValue)
        {
            return endValue - startValue;
        }

        public Vector2 Evaluate(Vector2 startValue, Vector2 changeValue, float t)
        {
            return startValue + changeValue * t;
        }
    }
}
