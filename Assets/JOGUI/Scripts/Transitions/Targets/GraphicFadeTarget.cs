using UnityEngine;
using UnityEngine.UI;

namespace JOGUI
{
    public class GraphicFadeTarget : IFadeTarget
    {
        private Graphic _target;

        public GraphicFadeTarget(Graphic graphic)
        {
            _target = graphic;
        }

        public void SetAlpha(float alpha)
        {
            var color = _target.color;
            color.a = alpha;
            _target.color = color;
        }

        public Object GetOnDestroyLink() => _target;
    }
}
