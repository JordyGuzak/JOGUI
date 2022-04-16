namespace JOGUI
{
    public class IntEvaluator : IEvaluateType<int>
    {
        public int GetChangeValue(int startValue, int endValue)
        {
            return endValue - startValue;
        }

        public int Evaluate(int startValue, int changeValue, float t)
        {
            return startValue + (int)(changeValue * t);
        }
    }
}
