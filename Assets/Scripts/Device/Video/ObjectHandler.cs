using System.Linq;
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

            handlerRectTransform.SetHandlerPosition(args[1], in _containerResolutionRatio);
            
            var size = args[2].AutoSizedVector(in _containerResolutionRatio);
            handlerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x); 
            handlerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y); 
        }
    }
    
    public static class ObjectHandlerExtensions
    {
        private static readonly float[] NullVector = {0f, 0f};
        private static readonly Vector2Int TransformVector = new Vector2Int(1, -1); 
        
        /// <summary>
        /// Устанавливает новую позицию объекта захвата, если это необходимо (координаты не нулевые)
        /// </summary>
        public static void SetHandlerPosition(this RectTransform rectTransform, object arg, in Vector2 containerResolutionRatio)
        {
            var newPosition = arg.AutoSizedVector(in containerResolutionRatio);
            if (IsNullPosition(newPosition))
            {
                Debug.Log("Not set!");
                return;
            }

            rectTransform.anchoredPosition = newPosition * TransformVector; 
        }
        
        /// <summary>
        /// Проверяет, является ли теущая позиция нулевой 
        /// </summary>
        private static bool IsNullPosition(Vector2 position)
            => NullVector.Where((t, i) => position[i] < t).Any();
        
        /// <summary>
        /// Преобразует переданные данные типа Vector2Int в Vector2 и скалирует по соотноешнию _containerResolutionRatio
        /// </summary>
        public static Vector2 AutoSizedVector(this object arg, in Vector2 containerResolutionRatio)
        {
            var value = (Vector2) (Vector2Int) arg;
            value.Scale(containerResolutionRatio);
            return value;
        }
    }
}