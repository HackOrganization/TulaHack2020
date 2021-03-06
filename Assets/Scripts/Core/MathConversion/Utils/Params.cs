﻿using VideoWideFieldParams = Device.Video.Utils.WideFieldParams;
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
        /// Разность начальной точки УПК и ШПК
        /// </summary>
        public const int DELTA_START_STEP_X = -490;

        /// <summary>
        /// Разность начальной точки УПК и верхнего положения кадра ШПК 
        /// </summary>
        public const int DELTA_START_STEP_Y = -190;
        
        /// <summary>
        /// Вертикальный угол захвата камеры
        /// </summary>
        public const float VERTICAL_ANGLES = 35f;
        
        /// <summary>
        /// Горизонтальных шагов на 1 деление круга (радус)
        /// </summary>
        public static readonly float AzimuthAngleToSteps = (float) LowLevelTightFiledParams.CYCLE_STEPS_X / Params.CYCLE_ANGLES;
        
        /// <summary>
        /// Вертикальных шагов на 1 деление круга (градус)
        /// </summary>
        public static readonly float ElevationAngleToSteps = (float) LowLevelTightFiledParams.CYCLE_STEPS_Y / Params.CYCLE_ANGLES;

        /// <summary>
        /// Пикселей изображения ШПК на 1 деление круга (градус)
        /// </summary>
        public static readonly float ElevationAnglesToPixels = VideoWideFieldParams.HEIGHT / VERTICAL_ANGLES;
    }
}