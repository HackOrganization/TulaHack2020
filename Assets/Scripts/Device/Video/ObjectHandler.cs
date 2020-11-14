using Core.GameEvents;
using Device.Utils;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;
using EventType = Core.GameEvents.EventType;

namespace Device.Video
{
    /// <summary>
    /// Наводит объект захвата на изображении
    /// </summary>
    public class ObjectHandler: MonoBehaviour
    {
        [Header("Square settings")]
        [SerializeField] private RectTransform handlerRectTransform;

        [Header("Status colors", order = 1)] 
        [SerializeField] private RawImage handlerImage;
        [SerializeField] private Color detectedColor;
        [SerializeField] private Color lossColor;
        
        
        /// <summary>
        /// Тип камеры
        /// </summary>
        private CameraTypes _cameraType;
        
        /// <summary>
        /// Родительский конйтенер, где бегает объект
        /// </summary>
        private RectTransform _container;

        /// <summary>
        /// Размер изображения
        /// </summary>
        private Vector2Int _resolution;

        /// <summary>
        /// Соотношение преобразование размера объекта к размеру изображения (rectSize/imageSize)
        /// </summary>
        private Vector2 _containerResolutionRatio;

        /// <summary>
        /// Результат обнаружения в последнем кадре
        /// </summary>
        private bool _detectionResult;
        
        public void Initialize(CameraTypes cameraType, RectTransform container, Vector2Int resolution)
        {
            _cameraType = cameraType;
            _container = container;
            _resolution = resolution;
            
            _containerResolutionRatio = _container.rect.size / _resolution;
            
            SetSubscription();
        }

        private void OnDestroy()
        {
            ResetSubscription();
        }
        
        #region GAMEEVENTS
        
        /// <summary>
        /// Устанавливает подписки на глоабльные события
        /// </summary>
        private void SetSubscription()
        {
            EventManager.AddHandler(EventType.CameraDrawObject, OnObjectCaptured);
        }
        
        /// <summary>
        /// Отписывается от рассылки глоабльных событий
        /// </summary>
        private void ResetSubscription()
        {
            EventManager.RemoveHandler(EventType.CameraDrawObject, OnObjectCaptured);
        }
        
        /// <summary>
        /// Перехват сообщения  
        /// </summary>
        private void OnObjectCaptured(object[] args)
        {
            if(_cameraType != (CameraTypes)args[0])
                return;

            var isDetected = (bool) args[1];
            if (_detectionResult != isDetected)
            {
                handlerImage.color = isDetected
                    ? detectedColor
                    : lossColor;
                _detectionResult = isDetected;
            }
            
            if(!_detectionResult)
                return;
            
            handlerRectTransform.SetHandlerPosition(args[2], in _containerResolutionRatio);
            handlerRectTransform.SetHandlerSize(args[3], in _containerResolutionRatio);
        }
        
        #endregion
    }
}