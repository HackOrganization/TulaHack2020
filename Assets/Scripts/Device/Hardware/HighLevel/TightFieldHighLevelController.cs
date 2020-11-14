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

            if ((SourceCommandType) args[1] == SourceCommandType.Auto)
                AutoSetUp(in args);
            else
                ManualSetUp((Vector2Int) args[2]);
        }

        private void AutoSetUp(in object[] args)
        {
            var objectImagePosition = (Vector2Int) args[2];
            var wideFieldAzimuthStep = (int) args[3];
            
            var tightAzimuthStep = wideFieldAzimuthStep.AzimuthTightFieldCameraStep();
            var elevationStep = objectImagePosition.ElevationTightFieldCameraStep();

            var newPosition = new Vector2Int(tightAzimuthStep, elevationStep);
            SetUpPosition(newPosition);
        }

        private void ManualSetUp(Vector2Int deltaPosition)
        {
            var newPosition = CurrentPosition + deltaPosition;
            SetUpPosition(newPosition);
        }

        private void SetUpPosition(Vector2Int newPosition)
        {
            newPosition.Clamp(MinStepValue, MaxStepValue);
            PositionController.SetUp(newPosition);
        }
    }
}