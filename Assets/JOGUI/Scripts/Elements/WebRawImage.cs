using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace JOGUI
{
    public class WebRawImage : RawImage
    {
        [SerializeField] private string _url;

        private Coroutine _textureLoadingRoutine;
        
        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                LoadTextureFromUrl(_url);
            }
        }

        protected override void Start()
        {
            base.Start();
            LoadTextureFromUrl(_url);
        }

        private void LoadTextureFromUrl(string url)
        {
            if (_textureLoadingRoutine != null)
                StopCoroutine(_textureLoadingRoutine);

            if (string.IsNullOrWhiteSpace(url))
            {
                texture = null;
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
                texture = null;
            }
            else
            {
                texture = downloadHandler.texture;
            }

            _textureLoadingRoutine = null;
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            //LoadTextureFromUrl(_url);
        }
#endif
    }
}