
namespace JOGUI
{
    public interface IEvaluateType<T>
    {
        T Evaluate(EaseType easeType, T startValue, T endValue, float t);
    }
}