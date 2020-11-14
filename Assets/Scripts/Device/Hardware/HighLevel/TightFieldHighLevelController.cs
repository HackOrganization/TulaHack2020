using Core.MathConversion;
using Device.Hardware.HighLevel.Utils;
using Device.Utils;
using UnityEngine;

using LowLevelTightFieldParams = Device.Hardware.LowLevel.Utils.TightFieldParams;

namespace Device.Hardware.HighLevel
{
    /// <summary>
    /// Высокоуровневое управление устройством поворота узкопольной камеры
    /// </summary>
    public class TightFieldHighLevelController: CameraBaseController
    {
        /// <summary>
        /// Минимально допустимое значение позиции шаговика (в шагах)
        /// </summary>
        protected static readonly Vector2Int MinStepValue = new Vector2Int(LowLevelTightFieldParams.TIGHTFIELD_MIN_STEPS_X, LowLevelTightFieldParams.TIGHTFIELD_MIN_STEPS_Y);
        
        /// <summary>
        /// Максимально допустимое значение позиции шаговика (в шагах)
        /// </summary>
        protected static readonly Vector2Int MaxStepValue = new Vector2Int(LowLevelTightFieldParams.TIGHTFIELD_MAX_STEPS_X, LowLevelTightFieldParams.TIGHTFIELD_MAX_STEPS_Y);
        
        /// <summary>
        /// Тип камеры
        /// </summary>
        public override CameraTypes CameraType => CameraTypes.TightField;

        /// <summary>
        /// Текущая позиция устройства (в шагах)
        /// </summary>
        public override Vector2Int CurrentPosition { get; set; }

        public override void Initialize()
        {
            PositionController = new TightFieldPositionController();
            base.Initialize();
        }
        
        /// <summary>
        /// Обновление координаты наведения
        /// </summary>
        protected override void OnNewPositionCaptured(object[] args)
        {
            var cameraType = (CameraTypes) args[0];
            if(CameraType != cameraType)
                return;

            var objectImagePosition = (Vector2Int) args[1];
            var wideFieldAzimuthStep = (int) args[2];
            
            var tightAzimuthStep = wideFieldAzimuthStep.AzimuthTightFieldCameraStep();
            var elevationStep = objectImagePosition.ElevationTightFieldCameraStep();

            var newPosition = new Vector2Int(tightAzimuthStep, elevationStep);
            newPosition.Clamp(MinStepValue, MaxStepValue);
            PositionController.SetUp(newPosition);
        }
    }
}