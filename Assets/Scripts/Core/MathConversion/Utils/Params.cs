using System;
using UnityEngine;
using VideoWideFieldParams = Device.Video.Utils.WideFieldParams;
using LowLevelWideFieldParams = Device.Hardware.LowLevel.Utils.WideFieldParams;
using LowLevelTightFiledParams = Device.Hardware.LowLevel.Utils.TightFieldParams;

namespace Core.MathConversion.Utils
{
    public static class Params
    {
        /// <summary>
        /// Количество делений полного круга (в данном случае, углов)
        /// </summary>
        public const int CYCLE_ANGLES = 360;
    }

    public static class WideFieldParams
    {
        /// <summary>
        /// Горизонтальный угол захвата камеры
        /// </summary>
        public const float HORISONTAL_ANGLES = 47f;
        
        /// <summary>
        /// Вертикальный угол захвата камеры
        /// </summary>
        public const float VERTICAL_ANGLES = HORISONTAL_ANGLES / 4 * 3;
        
        /// <summary>
        /// Дистанция захвата изображения
        /// </summary>
        public const float IMAGE_DISTANCE = 5;

        /// <summary>
        /// Пикселей на 1 деление круга (градус)
        /// </summary>
        public static readonly float AngleToPixels = VideoWideFieldParams.WIDTH / (HORISONTAL_ANGLES);
        
        /// <summary>
        /// Шагов на 1 деление круга (градус)
        /// </summary>
        public static readonly float AngleToSteps = (float) LowLevelWideFieldParams.CYCLE_STEPS / Params.CYCLE_ANGLES;
    }

    public static class TightFieldParams
    {
        /// <summary>
        /// Разница в высотах ШПК и УПК (в метрах)
        /// </summary>
        public const float DELTA_REAL_HEIGHT = 0.1f;
        
        /// <summary>
        /// Разница в глубине ШПК и УПК (в метрах)
        /// </summary>
        private const float DELTA_REAL_DEEP = 0.05f;

        /// <summary>
        /// Дистанция захвата изображения
        /// </summary>
        public const float IMAGE_DISTANCE = WideFieldParams.IMAGE_DISTANCE + DELTA_REAL_DEEP;

        /// <summary>
        /// Разность начальной точки УПК и ШПК
        /// </summary>
        public const int DELTA_START_STEP_X = -570;

        /// <summary>
        /// Разность начальной точки УПК и параллельного ШПК центра 
        /// </summary>
        public const int DELTA_STEP_Y = -195;
        
        //ToDo: BEFORE TEST SetUp correct
        /// <summary>
        /// Вертикальный угол захвата камеры
        /// </summary>
        public const float VERTICAL_ANGLES = 20;
        
        /// <summary>
        /// Горизонтальных шагов на 1 деление круга (радус)
        /// </summary>
        public static readonly float HorizontalAngleToSteps = (float) LowLevelTightFiledParams.CYCLE_STEPS_X / Params.CYCLE_ANGLES;
        
        /// <summary>
        /// Вертикальных шагов на 1 деление круга (градус)
        /// </summary>
        public static readonly float VerticalAngleToSteps = (float) LowLevelTightFiledParams.CYCLE_STEPS_Y / Params.CYCLE_ANGLES;
        
        /// <summary>
        /// Возвращает высоту изображения (в метрах)
        /// </summary>
        public static double GetImageRealHeight(float imageDistance)
            => 2 * Math.Tan(Mathf.Deg2Rad * WideFieldParams.VERTICAL_ANGLES / 2f) * imageDistance;

        /// <summary>
        /// Возвращает коэффициент преобразования пиксельного расстояния в реальное (м/pi) 
        /// </summary>
        public static double GetPixelToMeters(double imageRealHeight)
            => imageRealHeight / VideoWideFieldParams.HEIGHT;

        /// <summary>
        /// Коэффициент преобразования пиксельного расстояния в реальное (м/pi)
        /// (предустановлено на дистанцию захвата WideFieldParams.IMAGE_DISTANCE)   
        /// </summary>
        public static readonly double PixelToMeters =
            GetPixelToMeters(GetImageRealHeight(WideFieldParams.IMAGE_DISTANCE));
        
        
        /// <summary>
        /// Смещение центра УПК относительно ШПК в пикселях
        /// (предустановлено на дистанцию захвата WideFieldParams.IMAGE_DISTANCE)
        /// </summary>
        public static readonly int TightToWightCenterPixelPosition = (int)
            ((VideoWideFieldParams.HEIGHT / 2d) - (DELTA_REAL_HEIGHT / PixelToMeters));
    }
}