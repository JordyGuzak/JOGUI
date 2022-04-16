using System;
using UnityEngine;

namespace JOGUI
{
    public enum EaseType
    {
        EaseInQuad = 0,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc,
        Linear,
        Spring,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
    }

    public static class Ease
    {
        public static float Evaluate(EaseType easeType, float time, float duration, float overshootOrAmplitude, float period)
        {
            switch (easeType)
            {
                case EaseType.EaseInQuad:
                    return (time /= duration) * time;
                case EaseType.EaseOutQuad:
                    return (float) (-(double) (time /= duration) * ((double) time - 2.0));
                case EaseType.EaseInOutQuad:
                    return (double) (time /= duration * 0.5f) < 1.0 ? 0.5f * time * time : (float) (-0.5 * ((double) --time * ((double) time - 2.0) - 1.0));
                case EaseType.EaseInCubic:
                    return (time /= duration) * time * time;
                case EaseType.EaseOutCubic:
                    return (float) ((double) (time = (float) ((double) time / (double) duration - 1.0)) * (double) time * (double) time + 1.0);
                case EaseType.EaseInOutCubic:
                    return (double) (time /= duration * 0.5f) < 1.0 ? 0.5f * time * time * time : (float) (0.5 * ((double) (time -= 2f) * (double) time * (double) time + 2.0));
                case EaseType.EaseInQuart:
                    return (time /= duration) * time * time * time;
                case EaseType.EaseOutQuart:
                    return (float) -((double) (time = (float) ((double) time / (double) duration - 1.0)) * (double) time * (double) time * (double) time - 1.0);
                case EaseType.EaseInOutQuart:
                    return (double) (time /= duration * 0.5f) < 1.0 ? 0.5f * time * time * time * time : (float) (-0.5 * ((double) (time -= 2f) * (double) time * (double) time * (double) time - 2.0));
                case EaseType.EaseInQuint:
                    return (time /= duration) * time * time * time * time;
                case EaseType.EaseOutQuint:
                    return (float) ((double) (time = (float) ((double) time / (double) duration - 1.0)) * (double) time * (double) time * (double) time * (double) time + 1.0);
                case EaseType.EaseInOutQuint:
                    return (double) (time /= duration * 0.5f) < 1.0 ? 0.5f * time * time * time * time * time : (float) (0.5 * ((double) (time -= 2f) * (double) time * (double) time * (double) time * (double) time + 2.0));
                case EaseType.EaseInSine:
                    return (float) (-Math.Cos((double) time / (double) duration * 1.57079637050629) + 1.0);
                case EaseType.EaseOutSine:
                    return (float) Math.Sin((double) time / (double) duration * 1.57079637050629);
                case EaseType.EaseInOutSine:
                    return (float) (-0.5 * (Math.Cos(3.14159274101257 * (double) time / (double) duration) - 1.0));
                case EaseType.EaseInExpo:
                    return (double) time != 0.0 ? (float) Math.Pow(2.0, 10.0 * ((double) time / (double) duration - 1.0)) : 0.0f;
                case EaseType.EaseOutExpo:
                    return (double) time == (double) duration ? 1f : (float) (-Math.Pow(2.0, -10.0 * (double) time / (double) duration) + 1.0);
                case EaseType.EaseInOutExpo:
                    if ((double) time == 0.0)
                        return 0.0f;
                    if ((double) time == (double) duration)
                        return 1f;
                    return (double) (time /= duration * 0.5f) < 1.0 ? 0.5f * (float) Math.Pow(2.0, 10.0 * ((double) time - 1.0)) : (float) (0.5 * (-Math.Pow(2.0, -10.0 * (double) --time) + 2.0));
                case EaseType.EaseInCirc:
                    return (float) -(Math.Sqrt(1.0 - (double) (time /= duration) * (double) time) - 1.0);
                case EaseType.EaseOutCirc:
                    return (float) Math.Sqrt(1.0 - (double) (time = (float) ((double) time / (double) duration - 1.0)) * (double) time);
                case EaseType.EaseInOutCirc:
                    return (double) (time /= duration * 0.5f) < 1.0 ? (float) (-0.5 * (Math.Sqrt(1.0 - (double) time * (double) time) - 1.0)) : (float) (0.5 * (Math.Sqrt(1.0 - (double) (time -= 2f) * (double) time) + 1.0));
                case EaseType.Linear:
                    return time / duration;
                case EaseType.EaseInBack:
                    return (float) ((double) (time /= duration) * (double) time * (((double) overshootOrAmplitude + 1.0) * (double) time - (double) overshootOrAmplitude));
                case EaseType.EaseOutBack:
                    return (float) ((double) (time = (float) ((double) time / (double) duration - 1.0)) * (double) time * (((double) overshootOrAmplitude + 1.0) * (double) time + (double) overshootOrAmplitude) + 1.0);
                case EaseType.EaseInOutBack:
                    return (double) (time /= duration * 0.5f) < 1.0 ? (float) (0.5 * ((double) time * (double) time * (((double) (overshootOrAmplitude *= 1.525f) + 1.0) * (double) time - (double) overshootOrAmplitude))) : (float) (0.5 * ((double) (time -= 2f) * (double) time * (((double) (overshootOrAmplitude *= 1.525f) + 1.0) * (double) time + (double) overshootOrAmplitude) + 2.0));
                case EaseType.EaseInElastic:
                    if ((double) time == 0.0)
                        return 0.0f;
                    if ((double) (time /= duration) == 1.0)
                        return 1f;
                    if ((double) period == 0.0)
                        period = duration * 0.3f;
                    float num1;
                    if ((double) overshootOrAmplitude < 1.0)
                    {
                        overshootOrAmplitude = 1f;
                        num1 = period / 4f;
                    }
                    else
                        num1 = period / 6.283185f * (float) Math.Asin(1.0 / (double) overshootOrAmplitude);
                    return (float) -((double) overshootOrAmplitude * Math.Pow(2.0, 10.0 * (double) --time) * Math.Sin(((double) time * (double) duration - (double) num1) * 6.28318548202515 / (double) period));
                case EaseType.EaseOutElastic:
                    if ((double) time == 0.0)
                        return 0.0f;
                    if ((double) (time /= duration) == 1.0)
                        return 1f;
                    if ((double) period == 0.0)
                        period = duration * 0.3f;
                    float num2;
                    if ((double) overshootOrAmplitude < 1.0)
                    {
                        overshootOrAmplitude = 1f;
                        num2 = period / 4f;
                    }
                    else
                        num2 = period / 6.283185f * (float) Math.Asin(1.0 / (double) overshootOrAmplitude);
                    return (float) ((double) overshootOrAmplitude * Math.Pow(2.0, -10.0 * (double) time) * Math.Sin(((double) time * (double) duration - (double) num2) * 6.28318548202515 / (double) period) + 1.0);
                case EaseType.EaseInOutElastic:
                    if ((double) time == 0.0)
                        return 0.0f;
                    if ((double) (time /= duration * 0.5f) == 2.0)
                        return 1f;
                    if ((double) period == 0.0)
                        period = duration * 0.45f;
                    float num3;
                    if ((double) overshootOrAmplitude < 1.0)
                    {
                        overshootOrAmplitude = 1f;
                        num3 = period / 4f;
                    }
                    else
                        num3 = period / 6.283185f * (float) Math.Asin(1.0 / (double) overshootOrAmplitude);
                    return (double) time < 1.0 ? (float) (-0.5 * ((double) overshootOrAmplitude * Math.Pow(2.0, 10.0 * (double) --time) * Math.Sin(((double) time * (double) duration - (double) num3) * 6.28318548202515 / (double) period))) : (float) ((double) overshootOrAmplitude * Math.Pow(2.0, -10.0 * (double) --time) * Math.Sin(((double) time * (double) duration - (double) num3) * 6.28318548202515 / (double) period) * 0.5 + 1.0);
                default:
                    return time / duration;
            }
        }


        public static float Evaluate(EaseType easeType, float startValue, float endValue, float t)
        {
            switch (easeType)
            {
                case EaseType.Linear:
                    return Linear(startValue, endValue, t);
                case EaseType.Spring:
                    return Spring(startValue, endValue, t);
                case EaseType.EaseInQuad:
                    return EaseInQuad(startValue, endValue, t);
                case EaseType.EaseOutQuad:
                    return EaseOutQuad(startValue, endValue, t);
                case EaseType.EaseInOutQuad:
                    return EaseInOutQuad(startValue, endValue, t);
                case EaseType.EaseInCubic:
                    return EaseInCubic(startValue, endValue, t);
                case EaseType.EaseOutCubic:
                    return EaseOutCubic(startValue, endValue, t);
                case EaseType.EaseInOutCubic:
                    return EaseInOutCubic(startValue, endValue, t);
                case EaseType.EaseInBounce:
                    return EaseInBounce(startValue, endValue, t);
                case EaseType.EaseOutBounce:
                    return EaseOutBounce(startValue, endValue, t);
                case EaseType.EaseInElastic:
                    return EaseInElastic(startValue, endValue, t);
                case EaseType.EaseOutElastic:
                    return EaseOutElastic(startValue, endValue, t);
                case EaseType.EaseInOutElastic:
                    return EaseInOutElastic(startValue, endValue, t);
                case EaseType.EaseInOutBounce:
                    return EaseInOutBounce(startValue, endValue, t);
                case EaseType.EaseInBack:
                    return EaseInBack(startValue, endValue, t);
                case EaseType.EaseOutBack:
                    return EaseOutBack(startValue, endValue, t);
                case EaseType.EaseInOutBack:
                    return EaseInOutBack(startValue, endValue, t);
                default:
                    return Linear(startValue, endValue, t);
            }
        }

        public static float Linear(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, value);
        }

        public static float Spring(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
            return start + (end - start) * value;
        }

        public static float EaseInQuad(float start, float end, float value)
        {
            end -= start;
            return end * value * value + start;
        }

        public static float EaseOutQuad(float start, float end, float value)
        {
            end -= start;
            return -end * value * (value - 2) + start;
        }

        public static float EaseInOutQuad(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * value * value + start;
            value--;
            return -end * 0.5f * (value * (value - 2) - 1) + start;
        }

        public static float EaseInCubic(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value + start;
        }

        public static float EaseOutCubic(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value + 1) + start;
        }

        public static float EaseInOutCubic(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * value * value * value + start;
            value -= 2;
            return end * 0.5f * (value * value * value + 2) + start;
        }

        public static float EaseInQuart(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value + start;
        }

        public static float EaseOutQuart(float start, float end, float value)
        {
            value--;
            end -= start;
            return -end * (value * value * value * value - 1) + start;
        }

        public static float EaseInOutQuart(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * value * value * value * value + start;
            value -= 2;
            return -end * 0.5f * (value * value * value * value - 2) + start;
        }

        public static float EaseInQuint(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value * value + start;
        }

        public static float EaseOutQuint(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value * value * value + 1) + start;
        }

        public static float EaseInOutQuint(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * value * value * value * value * value + start;
            value -= 2;
            return end * 0.5f * (value * value * value * value * value + 2) + start;
        }

        public static float EaseInSine(float start, float end, float value)
        {
            end -= start;
            return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
        }

        public static float EaseOutSine(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
        }

        public static float EaseInOutSine(float start, float end, float value)
        {
            end -= start;
            return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
        }

        public static float EaseInExpo(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (value - 1)) + start;
        }

        public static float EaseOutExpo(float start, float end, float value)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * value) + 1) + start;
        }

        public static float EaseInOutExpo(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
            value--;
            return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
        }

        public static float EaseInCirc(float start, float end, float value)
        {
            end -= start;
            return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
        }

        public static float EaseOutCirc(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * Mathf.Sqrt(1 - value * value) + start;
        }

        public static float EaseInOutCirc(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
            value -= 2;
            return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
        }

        public static float EaseInBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            return end - EaseOutBounce(0, end, d - value) + start;
        }

        public static float EaseOutBounce(float start, float end, float value)
        {
            value /= 1f;
            end -= start;
            if (value < (1 / 2.75f))
            {
                return end * (7.5625f * value * value) + start;
            }
            else if (value < (2 / 2.75f))
            {
                value -= (1.5f / 2.75f);
                return end * (7.5625f * (value) * value + .75f) + start;
            }
            else if (value < (2.5 / 2.75))
            {
                value -= (2.25f / 2.75f);
                return end * (7.5625f * (value) * value + .9375f) + start;
            }
            else
            {
                value -= (2.625f / 2.75f);
                return end * (7.5625f * (value) * value + .984375f) + start;
            }
        }

        public static float EaseInOutBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            if (value < d * 0.5f) return EaseInBounce(0, end, value * 2) * 0.5f + start;
            else return EaseOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
        }

        public static float EaseInBack(float start, float end, float value)
        {
            end -= start;
            value /= 1;
            float s = 1.70158f;
            return end * (value) * value * ((s + 1) * value - s) + start;
        }

        public static float EaseOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value = (value) - 1;
            return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
        }

        public static float EaseInOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value /= .5f;
            if ((value) < 1)
            {
                s *= (1.525f);
                return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
            }

            value -= 2;
            s *= (1.525f);
            return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
        }

        public static float EaseInElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d) == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
        }

        public static float EaseOutElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d) == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p * 0.25f;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
        }

        public static float EaseInOutElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d * 0.5f) == 2) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
            return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
        }
    }
}

