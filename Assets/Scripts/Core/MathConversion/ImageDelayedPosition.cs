using Core.MathConversion.Utils;
using UnityEngine;

namespace Core.MathConversion
{
    public static class MathConversions
    {
        /// <summary>
        /// Получает позицию объекта на новом изображении по координатам объекта на старом изображении (в пикселях) 
        /// </summary>
        public static Vector2Int DelayedImageHorizontalPosition(this Vector2Int objectPosition,
            in Vector2Int currentPosition, in Vector2Int cashedPosition)
        {
            var deltaPosition = Mathf.FloorToInt(
                (currentPosition.x - cashedPosition.x) / WideFieldParams.AngleToSteps * WideFieldParams.AngleToPixels);
            
            return new Vector2Int(objectPosition.x + deltaPosition, objectPosition.y);
        }

        /// <summary>
        /// Получает новую оринетацию центра камеры в шагах 
        /// </summary>
        public static Vector2Int HorizontalPosition(this Vector2Int objectPosition, in Vector2Int cashedPosition)
        {
            var deltaPosition = Mathf.FloorToInt(
                objectPosition.x / WideFieldParams.AngleToPixels * WideFieldParams.AngleToSteps);
            
            return new Vector2Int(cashedPosition.x + deltaPosition, cashedPosition.y);
        }
    }
}