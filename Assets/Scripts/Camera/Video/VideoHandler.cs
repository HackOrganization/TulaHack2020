using Camera.Data;
using Camera.Utils;
using Core;
using Core.OrderStart;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;
using EventType = Core.EventType;

namespace Camera.Video
{
    /// <summary>
    /// Захватывает видео с камеры.
    /// Вызывается перед тем, как будет подвтержден запрос на авторизацию камер,
    /// чтобы событие авторизации пришло позже того, как мы на него подпишемся
    /// </summary>
    public class VideoHandler : MonoBehaviour, IStarter
    {
        [Header("Test")] 
        [SerializeField] private bool execute;
        
        [Header("Camera info")] 
        [SerializeField]
        private CameraTypes cameraType;

        [Header("UI")] 
        [SerializeField] private RawImage destination;
        
        [Header("Data")] 
        [SerializeField] 
        private CameraIdentificationSettings cameraIdentificationSettings;


        private Texture2D _sendFrame;
        private Vector2Int _webCamResolution;
        private WebCamTexture _webCamTexture;
        private WebCamDevice _webCamDevice;
        
        public void OnStart()
        {
            if(!execute)
                return;
            
            EventManager.AddHandler(EventType.DeviceAuthorized, OnCameraAuthorized);
        }

        private void OnDestroy()
        {
            EventManager.RemoveHandler(EventType.DeviceAuthorized, OnCameraAuthorized);
        }

        /// <summary>
        /// Вызывается при авторизации камер 
        /// </summary>
        private void OnCameraAuthorized(object[] args)
        {
            SetUpWebCam();
            SetUpDestination();
            Play();   
        }

        /// <summary>
        /// Настройка камеры
        /// </summary>
        private void SetUpWebCam()
        {
            var idName = cameraIdentificationSettings.GetName(cameraType);
            _webCamDevice = WebCamTexture.devices.GetByIdentificationName(idName);
            _webCamTexture = new WebCamTexture(_webCamDevice.name);

            _webCamResolution = new Vector2Int(_webCamTexture.width, _webCamTexture.height);
            _sendFrame = new Texture2D(_webCamResolution.x, _webCamResolution.y);
        }

        /// <summary>
        /// Настройка элемента UI вывода с камеры  
        /// </summary>
        private void SetUpDestination()
        {
            destination.texture = _webCamTexture;
            var ratio = _webCamResolution.x / (float) _webCamResolution.y;
            var destinationRect = destination.rectTransform;
            destinationRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, destinationRect.rect.width / ratio);
        }

        /// <summary>
        /// Чтение с камеры
        /// </summary>
        public void Play()
        {
            if (_webCamTexture != null)
                _webCamTexture.Play();
        }

        /// <summary>
        /// Остановка четния с камеры
        /// </summary>
        public void Stop()
        {
            if (_webCamTexture != null)
                _webCamTexture.Stop();
        }
    }
}
