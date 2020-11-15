using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils.Extensions
{
    public static class VectorExtensions
    {
        private static readonly float[] NullVector = {0f, 0f};
        private static readonly int[] NullVectorInt = {0, 0};
        
        /// <summary>
        /// Проверяет, является ли теущая позиция нулевой 
        /// </summary>
        public static bool IsNullPosition(this Vector2 position)
            => NullVector.Where((t, i) => position[i] < t).Any(); /// <summary>
        
        /// Проверяет, является ли теущая позиция нулевой 
        /// </summary>
        public static bool IsNullPosition(this Vector2Int position)
            => NullVectorInt.Where((t, i) => position[i] < t).Any(); 
        
        /// <summary>
        /// Проверяет, является ли текущий размер нулевым 
        /// </summary>
        public static bool IsNullSize(this Vector2Int position)
            => NullVectorInt.Where((t, i) => position[i] == t).Any();

        /// <summary>
        /// Усредненное значение вектора 
        /// </summary>
        public static Vector2 Average(this IEnumerable<Vector2> list)
        {
            var count = 0;
            var buffer = new Vector2();
            foreach (var element in list)
            {
                buffer += element;
                ++count;
            }

            return buffer / count;
        }
    }
}