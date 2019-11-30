using UnityEngine;

namespace JOGUI
{
    public class Vector3Evaluator : IEvaluateType<Vector3>
    {
        public Vector3 Evaluate(EaseType easeType, Vector3 startValue, Vector3 endValue, float t)
        {
            Vector3 result = Vector3.zero;
            result.x = Ease.Evaluate(easeType, startValue.x, endValue.x, t);
            result.y = Ease.Evaluate(easeType, startValue.y, endValue.y, t);
            result.z = Ease.Evaluate(easeType, startValue.z, endValue.z, t);
            return result;
        }
    }
}
