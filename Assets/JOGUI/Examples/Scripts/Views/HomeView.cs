using System;
using System.Collections.Generic;
using UnityEngine;

namespace JOGUI.Examples
{
    public class HomeView : View
    {
        [SerializeField] private RectTransform _drawer;
        [SerializeField] private Blocker _blocker;
        [SerializeField] private RectTransform _image;
        
        private Transition _enterTransition;
        private Transition _exitTransition;
        private Transition _drawerTransition;

        private Vector3 _imageWorldPosition;

        private void Awake()
        {
            Debug.Log("Awake => " + _image.position);
            Debug.Log("Awake => " + _image.anchoredPosition);
        }

        private void OnEnable()
        {
            Debug.Log("OnEnable => " + _image.position);
            Debug.Log("OnEnable => " + _image.anchoredPosition);
        }

        private void Start()
        {
            
            Debug.Log("Start => " + _image.position);
            Debug.Log("Start => " + _image.anchoredPosition);
        }

        public override void Initialize(ViewGroup viewGroup)
        {
            base.Initialize(viewGroup);

            _drawerTransition = new TransitionSet(TransitionMode.PARALLEL)
                .Add(new Slide(_drawer.position, SlideMode.IN, Direction.RIGHT)
                    .AddTarget(_drawer)
                    .SetEaseType(EaseType.EaseInOutCubic))
                .Add(new Fade(0, 0.66f)
                    .AddTarget(_blocker)
                    .SetEaseType(EaseType.EaseInOutCubic));

            _enterTransition = new Slide(RectTransform.position, SlideMode.IN, Direction.RIGHT).AddTarget(RectTransform);
            _exitTransition = new Fade(1, 0).AddTarget(this);
            
            _imageWorldPosition = _image.position;

            Debug.Log("Initialize => " + _imageWorldPosition);
            Debug.Log("Initialize => " + _image.anchoredPosition);
        }

        public override Transition GetEnterTransition()
        {
            return _enterTransition;
        }

        public override Transition GetExitTransition()
        {
            return _exitTransition;
        }

        public void GoToProfile()
        {
            ViewGroup.Navigate(typeof(ProfileView));
        }

        public void OpenDrawerMenu()
        {
            _drawer.gameObject.SetActive(true);
            _blocker.gameObject.SetActive(true);

            _drawerTransition
                .SetOnComplete(() =>
                {
                    _blocker.onPointerClick.AddListener(CloseDrawerMenu);
                })
                .Run();
        }

        public void CloseDrawerMenu()
        {
            _blocker.onPointerClick.RemoveListener(CloseDrawerMenu);

            _drawerTransition
                .Reversed()
                .SetOnComplete(() =>
                {
                    _blocker.gameObject.SetActive(false);
                    _drawer.gameObject.SetActive(false);
                })
                .Run();
        }

        public void SlideInTest()
        {
            Debug.Log(_image.position);
            Debug.Log(_imageWorldPosition);
            var slide = new Slide(_imageWorldPosition, SlideMode.IN, Direction.RIGHT).AddTarget(_image);
            slide.Run();
        }
        
        public void SlideOutTest()
        {
            var slide = new Slide(_imageWorldPosition, SlideMode.OUT, Direction.RIGHT).AddTarget(_image);
            slide.Run();
        }
    }
}
