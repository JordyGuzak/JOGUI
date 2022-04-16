namespace JOGUI
{
    public class FloatEvaluator : IEvaluateType<float>
    {
        public float GetChangeValue(float startValue, float endValue)
        {
            return endValue - startValue;
        }

        public float Evaluate(float startValue, float changeValue, float t)
        {
            return startValue + changeValue * t;
        }
    }
}
