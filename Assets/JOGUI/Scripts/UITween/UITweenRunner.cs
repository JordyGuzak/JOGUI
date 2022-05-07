using System.Collections.Generic;
using UnityEngine;

namespace JOGUI
{
    public class UITweenRunner : MonoBehaviour
    {
        private static UITweenRunner _instance;
        public static UITweenRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UITweenRunner>();

                    if (_instance == null)
                        _instance = new GameObject("UITweenRunner").AddComponent<UITweenRunner>();
                }

                return _instance;
            }
        }

        public bool IsPlaying => _runningTweens.Count > 0;

        private List<Tween> _runningTweens = new List<Tween>();

        private void Update()
        {
            if (!IsPlaying)
                return;

            foreach (var t in _runningTweens)
                t.Tick();
        }

        public void Play(params Tween[] tweens)
        {
            foreach (var tween in tweens)
            {
                if (_runningTweens.Contains(tween))
                {
                    tween.PlayFromStart();
                }
                else
                {
                    tween.Play();
                    tween.OnAnimationFinished += Remove;
                    tween.OnAnimationKilled += Remove;
                    _runningTweens.Add(tween);
                }
            }
        }

        public void Pause()
        {
            for (int i = 0; i < _runningTweens.Count; i++)
            {
                _runningTweens[i].Pause();
            }
        }

        public void Stop()
        {
            for (int i = _runningTweens.Count - 1; i >= 0; i--)
            {
                _runningTweens[i].Stop();
            }
        }

        private void Remove(Tween tween)
        {
            tween.OnAnimationFinished -= Remove;
            tween.OnAnimationKilled -= Remove;
            _runningTweens.Remove(tween);
        }
    }
}
