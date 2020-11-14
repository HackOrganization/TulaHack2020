using UnityEngine;

namespace Utils.Extensions
{
    public static class ObjectHandlerExtensions
    {
        private static readonly Vector2Int TransformVector = new Vector2Int(1, -1); 
        
        /// <summary>
        /// Устанавливает новую позицию объекта захвата, если это необходимо (координаты не нулевые)
        /// </summary>
        public static void SetHandlerPosition(this RectTransform rectTransform, object arg, in Vector2 containerResolutionRatio)
        {
            var newPosition = arg.AutoSizedVector(in containerResolutionRatio);
            if (newPosition.IsNullPosition())
                return;

            rectTransform.anchoredPosition = newPosition * TransformVector; 
        }

        /// <summary>
        /// Устанавливает новый размер объекта, если это необходимо (если размер не 0, 0) 
        /// </summary>
        public static void SetHandlerSize(this RectTransform rectTransform, object arg,
            in Vector2 containerResolutionRatio)
        {
            if (((Vector2Int) arg).IsNullSize())
                return;
            
            var size = arg.AutoSizedVector(in containerResolutionRatio);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x); 
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y); 
        }
        
        /// <summary>
        /// Преобразует переданные данные типа Vector2Int в Vector2 и скалирует по соотноешнию _containerResolutionRatio
        /// </summary>
        private static Vector2 AutoSizedVector(this object arg, in Vector2 containerResolutionRatio)
        {
            var value = (Vector2) (Vector2Int) arg;
            value.Scale(containerResolutionRatio);
            return value;
        }
    }
}