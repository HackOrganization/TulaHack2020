using UnityEngine;

namespace Device.Video.Utils
{
    public class WideFieldParams
    {
        /// <summary>
        /// Запрашиваемая ширина изображения
        /// </summary>
        public const int WIDTH = 640;
        
        /// <summary>
        /// Запрашивваемая высота изображения
        /// </summary>
        public const int HEIGHT = 480;

        /// <summary>
        /// Преобразовывает координаты точки, привязанной к левому верхнему углку, к координатам точки, привзяанным к центру
        /// </summary>
        public static Vector2Int UpLeftToCenterPoint(in Vector2Int input)
            => new Vector2Int(input.x - (WIDTH / 2), input.y - (HEIGHT / 2));
    }

    public class TightFiledParams
    {
        /// <summary>
        /// Запрашиваемая ширина изображения
        /// </summary>
        public const int WIDTH = 640;
        
        /// <summary>
        /// Запрашивваемая высота изображения
        /// </summary>
        public const int HEIGHT = 480;
        
        /// <summary>
        /// Преобразовывает координаты точки, привязанной к левому верхнему углку, к координатам точки, привзяанным к центру
        /// </summary>
        public static Vector2Int UpLeftToCenterPoint(in Vector2Int input)
            => new Vector2Int(input.x - (WIDTH / 2), input.y - (HEIGHT / 2));
    }
}