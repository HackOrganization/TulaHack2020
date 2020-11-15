using System;
using System.Collections;
using Core.GameEvents;
using Device.Data;
using Device.Utils;
using Device.Video.Utils;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

using EventType = Core.GameEvents.EventType;
using VideoTightFieldParams = Device.Video.Utils.TightFieldParams;
using VideoWideFieldParams = Device.Video.Utils.WideFieldParams;

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

        [Header("Object handler")] 
        [SerializeField, CanBeNull] protected ObjectHandler objectHandler;
        
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

        /// <summary>
        /// Включение автозахвата изображени п ри проигрывании видео
        /// </summary>
        private bool AutoCapture => _cameraType == CameraTypes.WideField;

        private bool _initialized;
        private CameraTypes _cameraType;
        private Vector2Int _webCamResolution;
        private WebCamTexture _webCamTexture;
        private WebCamDevice _webCamDevice;
        private WaitForSeconds _webCamFrameRateWait;

        public void Initialize(CameraTypes cameraType)
        {
            SetSubscription();
            _cameraType = cameraType;
            _webCamFrameRateWait = new WaitForSeconds(1f / Params.WEB_CAM_FPS);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        /// <summary>
        /// Устанавливает необхолдимые настройки арботы с изображением после включения камеры
        /// (необходим экземпляр изображения для работы)
        /// </summary>
        private void SetUpOnPlay()
        {
            if(_initialized)
                return;
            
            SetUpSendFrame();
            SetUpDestination();
            if(objectHandler != null)
                objectHandler.Initialize(_cameraType, destination.rectTransform, _webCamResolution);
            _initialized = true;
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
            
            SetUpOnPlay();
            
            if(AutoCapture)
                StartCapture();
            
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
            EndCapture();
            
            return true;
        }

        /// <summary>
        /// Включение корутины автозахвата изображения
        /// </summary>
        public void StartCapture()
        {
            StartCoroutine(ECapture());
        }
        
        /// <summary>
        /// Захватывает изображение с камеры для дальнейшей передачи
        /// Захват изображение происходит с частотой отрисовки камеры.
        /// Изображение сохраняется и указывает сохранить текущую позицию устройства
        /// </summary>
        public IEnumerator ECapture()
        {
            yield return new WaitForSeconds(1);
            
            while (Status == VideoStatuses.Play)
            {
                yield return new WaitForEndOfFrame();
                try
                {
                    var pixelsArray = _webCamTexture.GetPixels32();
                    SendFrame.SetPixels32(pixelsArray);
                    EventManager.RaiseEvent(EventType.DeviceHandlePosition, _cameraType);
                }
                catch (Exception ex)
                {        
                    Debug.LogError($"Camera [{_cameraType} : {_webCamTexture.deviceName}]: {ex}");
                }
                yield return _webCamFrameRateWait;
            }
        }

        /// <summary>
        /// Выключение корутины автозахвата изображения
        /// </summary>
        public void EndCapture()
        {
            StopCoroutine(ECapture());
        }

        public void Dispose()
        {
            Stop();
            Status = VideoStatuses.Null;
            
            if(objectHandler != null)
                objectHandler.Disable();
            
            ResetSubscription();
        }
        
        #region GAMEEVENTS
        
        /// <summary>
        /// Устанавливает подписки на глоабльные события
        /// </summary>
        private void SetSubscription()
        {
            EventManager.AddHandler(EventType.CameraAuthorized, OnCameraAuthorized);
        }
        
        /// <summary>
        /// Отписывается от рассылки глоабльных событий
        /// </summary>
        private void ResetSubscription()
        {
            EventManager.RemoveHandler(EventType.CameraAuthorized, OnCameraAuthorized);
        }
        
        /// <summary>
        /// Вызывается при авторизации камер 
        /// </summary>
        protected void OnCameraAuthorized(object[] args)
        {
            var idName = cameraIdentificationSettings.GetName(_cameraType);
            _webCamDevice = WebCamTexture.devices.GetByIdentificationName(idName);
            _webCamTexture = _cameraType == CameraTypes.WideField
                ? new WebCamTexture(_webCamDevice.name, VideoWideFieldParams.WIDTH, VideoWideFieldParams.HEIGHT)
                : new WebCamTexture(_webCamDevice.name, VideoTightFieldParams.WIDTH, VideoTightFieldParams.HEIGHT);
                
            Status = VideoStatuses.Authorized;
            Play();
        }
        
        #endregion
    }
}
