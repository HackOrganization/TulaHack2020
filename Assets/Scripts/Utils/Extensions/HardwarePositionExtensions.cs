using Core.MathConversion.Utils;
using UnityEngine;

namespace Utils.Extensions
{
    public static class HardwarePositionExtensions
    {
        /// <summary>
        /// Нужно ли обновлять позицию широкопольного изображения
        /// </summary>
        public static bool IsWideFieldUpdatable(this int currentPosition, in Vector2Int newPosition)
        {
            return Mathf.Abs(newPosition.x - currentPosition) >= WideFieldParams.AngleToSteps;
        }

        /// <summary>
        /// Нужно ли обновлять позицию узкопольной камеры 
        /// </summary>
        public static bool IsTightFieldUpdatable(this Vector2Int currentPosition, in Vector2Int newPosition)
        {
            //ToDo: write checking for update need
            return true;
        }
    }
}