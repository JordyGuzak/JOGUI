using UnityEngine;

namespace JOGUI
{
    public class Vector3Evaluator : IEvaluateType<Vector3>
    {
        public Vector3 GetChangeValue(Vector3 startValue, Vector3 endValue)
        {
            return endValue - startValue;
        }

        public Vector3 Evaluate(Vector3 startValue, Vector3 changeValue, float t)
        {
            return startValue + changeValue * t;
        }
    }
}
