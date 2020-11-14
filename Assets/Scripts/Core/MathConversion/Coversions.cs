using System;
using Core.MathConversion.Utils;
using UnityEngine;

using VideoWideFiledParams = Device.Video.Utils.WideFieldParams;

namespace Core.MathConversion
{
    public static class Conversions
    {
        /// <summary>
        /// Получает позицию объекта на новом изображении по координатам объекта на старом изображении (в пикселях) 
        /// </summary>
        public static Vector2Int DelayedWideImageObjectPosition(this Vector2Int objectPosition,
            in Vector2Int currentPosition, in Vector2Int cashedPosition)
        {
            var deltaPosition = Mathf.FloorToInt(
                (currentPosition.x - cashedPosition.x) / WideFieldParams.AngleToSteps * WideFieldParams.AngleToPixels);
            
            return new Vector2Int(objectPosition.x + deltaPosition, objectPosition.y);
        }

        /// <summary>
        /// Получает новую оринетацию центра ШПК в шагах 
        /// </summary>
        public static int AzimuthWideFieldCameraStep(this Vector2Int objectPosition, in Vector2Int cashedPosition)
        {
            var deltaPosition = Mathf.FloorToInt(
                VideoWideFiledParams.UpLeftToCenterPoint(objectPosition).x / WideFieldParams.AngleToPixels * WideFieldParams.AngleToSteps);

            return cashedPosition.x + deltaPosition;
        }

        /// <summary>
        /// Получает новую горицонтальную ориентацию центра УПК в шагах
        /// Из шага наведения ШПК вычитает смещение начал ШПК и УПК (в шагах)
        /// </summary>
        public static int AzimuthTightFieldCameraStep(this int wideFieldStepPosition)
            => wideFieldStepPosition - TightFieldParams.DELTA_START_STEP_X;

        /// <summary>
        /// Получает новую ориентацию УПК в шагах 
        /// </summary>
        public static int ElevationTightFieldCameraStep(this Vector2Int objectPosition)
        {
            var angleValue = objectPosition.y / TightFieldParams.VerticalAnglesToPixels;
            var stepValue = TightFieldParams.DELTA_START_STEP_Y - (int)(angleValue * TightFieldParams.VerticalAngleToSteps);

            return stepValue;
        }
    }
}