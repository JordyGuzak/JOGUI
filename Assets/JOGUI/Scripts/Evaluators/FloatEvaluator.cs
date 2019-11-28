namespace JOGUI
{
    public class FloatEvaluator : IEvaluateType<float>
    {
        public float Evaluate(EaseType easeType, float startValue, float endValue, float t)
        {
            return Ease.Evaluate(easeType, startValue, endValue, t);
        }
    }
}
