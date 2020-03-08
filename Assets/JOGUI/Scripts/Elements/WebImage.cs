using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace JOGUI
{
    public class WebImage : Image
    {
        [SerializeField] private string _url;

        private Coroutine _textureLoadingRoutine;
        
        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                LoadSpriteFromUrl(_url);
            }
        }

        protected override void Start()
        {
            base.Start();
            LoadSpriteFromUrl(_url);
        }

        private void LoadSpriteFromUrl(string url)
        {
            if (_textureLoadingRoutine != null)
                StopCoroutine(_textureLoadingRoutine);

            if (string.IsNullOrWhiteSpace(url))
            {
                sprite = null;
                return;
            }
            
            _textureLoadingRoutine = StartCoroutine(LoadSpriteFromUrlRoutine(url));
        }

        private IEnumerator LoadSpriteFromUrlRoutine(string url)
        {
            var request = new UnityWebRequest(url);
            var downloadHandler = new DownloadHandlerTexture(true);
            request.downloadHandler = downloadHandler;
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
                sprite = null;
            }
            else
            {
                var texture = downloadHandler.texture;
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 100f);
            }

            _textureLoadingRoutine = null;
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            //LoadSpriteFromUrl(_url);
        }
#endif
    }
}