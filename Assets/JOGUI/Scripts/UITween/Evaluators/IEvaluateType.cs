
namespace JOGUI
{
    public interface IEvaluateType<T>
    {
        T GetChangeValue(T startValue, T endValue);
        T Evaluate(T startValue, T changeValue, float t);
    }
}