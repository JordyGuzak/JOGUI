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

        public bool IsPlaying => _runningTweens.Count > 0 && _runningTweens[0].IsPlaying();

        private List<ITween> _runningTweens = new List<ITween>();

        private void Update()
        {
            if (!IsPlaying)
                return;

            for (int i = 0; i < _runningTweens.Count; i++)
            {
                _runningTweens[i].Tick();
            }
        }

        public void Play(params ITween[] tweens)
        {
            for (int i = 0; i < tweens.Length; i++)
            {
                var tween = tweens[i];

                if (_runningTweens.Contains(tween))
                {
                    tween.PlayFromStart();
                }
                else
                {
                    tween.Play();
                    tween.OnAnimationFinished += OnAnimationFinished;
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

        private void OnAnimationFinished(ITween tween)
        {
            tween.OnAnimationFinished -= OnAnimationFinished;
            _runningTweens.Remove(tween);
        }
    }
}
