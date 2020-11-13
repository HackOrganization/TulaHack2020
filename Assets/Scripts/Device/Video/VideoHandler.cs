using System.Collections;
using Core;
using Device.Data;
using Device.Utils;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;
using EventType = Core.EventType;

namespace Device.Video
{
    /// <summary>
    /// Захватывает видео с камеры.
    /// Вызывается перед тем, как будет подвтержден запрос на авторизацию камер,
    /// чтобы событие авторизации пришло позже того, как мы на него подпишемся
    /// </summary>
    public class VideoHandler : MonoBehaviour
    {
        [Header("UI")] 
        [SerializeField] private RawImage destination;
        
        [Header("Data")] 
        [SerializeField] 
        private CameraIdentificationSettings cameraIdentificationSettings;

        /// <summary>
        /// Текущий статус контроллера 
        /// </summary>
        public VideoStatuses Status { get; private set; } = VideoStatuses.Null;

        /// <summary>
        /// Флаг, что необхдимые насктройки камеры выполнены
        /// </summary>
        public bool IsAuthorized => Status >= VideoStatuses.Authorized;

        /// <summary>
        /// Картинка, подлежащая отправке
        /// </summary>
        public Texture2D SendFrame { get; private set; }

        private bool _initialized;
        private CameraTypes _cameraType;
        private Vector2Int _webCamResolution;
        private WebCamTexture _webCamTexture;
        private WebCamDevice _webCamDevice;

        public void Initialize(CameraTypes cameraType)
        {
            EventManager.AddHandler(EventType.DeviceAuthorized, OnCameraAuthorized);
            _cameraType = cameraType;
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
            var idName = cameraIdentificationSettings.GetName(_cameraType);
            _webCamDevice = WebCamTexture.devices.GetByIdentificationName(idName);
            _webCamTexture = new WebCamTexture(_webCamDevice.name);

            Status = VideoStatuses.Authorized;
            if(_cameraType == CameraTypes.WideField)
                Play();
        }

        /// <summary>
        /// Настраиваем объект, который будем отправлять
        /// </summary>
        private void SetUpSendFrame()
        {
            _webCamResolution = new Vector2Int(_webCamTexture.width, _webCamTexture.height);
            SendFrame = new Texture2D(_webCamResolution.x, _webCamResolution.y);
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
        public bool Play()
        {
            if(Status < VideoStatuses.Authorized)
                return false;
            
            if (_webCamTexture != null)
                _webCamTexture.Play();

            Status = VideoStatuses.Play;
            
            if(_initialized)
                return true;
            
            SetUpSendFrame();
            SetUpDestination();
            _initialized = true;

            return true;
        }

        /// <summary>
        /// Остановка четния с камеры
        /// </summary>
        public bool Stop()
        {
            if(Status < VideoStatuses.Authorized)
                return false;
            
            if (_webCamTexture != null)
                _webCamTexture.Stop();

            Status = VideoStatuses.Pause;

            return true;
        }

        /// <summary>
        /// Захватывает изображение с камеры для дальнейшей передачи
        /// </summary>
        public IEnumerator Capture()
        {
            yield return new WaitForEndOfFrame();
            SendFrame.SetPixels32(_webCamTexture.GetPixels32());
        }
    }
}
