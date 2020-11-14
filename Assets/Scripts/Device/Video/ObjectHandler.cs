using Core;
using Device.Utils;
using UnityEngine;
using EventType = Core.EventType;

namespace Device.Video
{
    /// <summary>
    /// Наводит объект захвата на изображении
    /// </summary>
    public class ObjectHandler: MonoBehaviour
    {
        private static readonly Vector2Int TransformVector = new Vector2Int(1, -1); 
        
        /// <summary>
        /// Recttransfrom ObjectHandler'a
        /// </summary>
        [SerializeField]
        private RectTransform handlerRectTransform;
        
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
        
        public void Initialize(CameraTypes cameraType, RectTransform container, Vector2Int resolution)
        {
            _cameraType = cameraType;
            _container = container;
            _resolution = resolution;
            
            _containerResolutionRatio = _container.rect.size / _resolution;
            
            EventManager.AddHandler(EventType.CameraDrawObject, OnObjectCaptured);
        }

        private void OnDestroy()
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

            var size = AutoSizedVector(args[2]);
            handlerRectTransform.anchoredPosition = AutoSizedVector(args[1]) * TransformVector;
            
            handlerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x); 
            handlerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y); 
        }

        /// <summary>
        /// Преобразует переданные данные типа Vector2Int в Vector2 и скалирует по соотноешнию _containerResolutionRatio
        /// </summary>
        private Vector2 AutoSizedVector(object arg)
        {
            var value = (Vector2) (Vector2Int) arg;
            value.Scale(_containerResolutionRatio);
            return value;
        }
    }
}