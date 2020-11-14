using Core.GameEvents;
using Core.MathConversion;
using Device.Hardware.HighLevel.Utils;
using Device.Utils;
using UnityEngine;
using Utils.Extensions;
using EventType = Core.GameEvents.EventType;

using GlobalWideFieldParams = Device.Utils.WideFieldParams;
using VideoWideFieldParams = Device.Video.Utils.WideFieldParams;
using LowLevelWideFieldParams = Device.Hardware.LowLevel.Utils.WideFieldParams;

namespace Device.Hardware.HighLevel
{
    /// <summary>
    /// Высокоуровневое управление устройством поворота широкопольной камеры
    /// </summary>
    public class WideFieldHighLevelController: CameraBaseController
    {
        /// <summary>
        /// Максимально допустимый размер изображения
        /// </summary>
        protected static readonly Vector2Int MaxImagePosition = new Vector2Int(VideoWideFieldParams.WIDTH, VideoWideFieldParams.HEIGHT);
        
        /// <summary>
        /// Тип камеры
        /// </summary>
        public override CameraTypes CameraType => CameraTypes.WideField;

        /// <summary>
        /// Текущая позиция устройства (в шагах)
        /// </summary>
        public override Vector2Int CurrentPosition
        {
            get => new Vector2Int(_currentPosition, 0);
            set => _currentPosition = value.x;
        } 

        private int _currentPosition;

        public override void Initialize()
        {
            PositionController = new WideFieldPositionController();
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
            if (objectImagePosition.IsNullPosition() || (byte) args[4] < Params.WIDEFIELD_DETECTION_PROBABILITY)
            {
                EventManager.RaiseEvent(EventType.CameraDrawObject, CameraTypes.WideField, false);
            }
            else
            {
                var newObjectImagePosition = objectImagePosition.DelayedWideImageObjectPosition(CurrentPosition, CashedDevicePosition);
                newObjectImagePosition.Clamp(MinImagePosition, MaxImagePosition);
                EventManager.RaiseEvent(EventType.CameraDrawObject, CameraTypes.WideField, true, newObjectImagePosition, args[3]);
                
                var azimuthStep = objectImagePosition.AzimuthWideFieldCameraStep(CashedDevicePosition);
                SetUpAzimuth(ref azimuthStep, SourceCommandType.Auto);
                
                EventManager.RaiseEvent(EventType.DeviceGoPosition,CameraTypes.TightField, SourceCommandType.Auto, objectImagePosition, azimuthStep);
            }
        }

        private void ManualSetUp(Vector2Int deltaPosition)
        {
            var azimuthStep = CurrentPosition.x + deltaPosition.x;
            SetUpAzimuth(ref azimuthStep, SourceCommandType.Manual);
        }

        private void SetUpAzimuth(ref int azimuthStep, in SourceCommandType commandSource)
        {
            azimuthStep = Mathf.Clamp(
                azimuthStep, 
                LowLevelWideFieldParams.WIDEFIELD_MIN_STEPS,
                LowLevelWideFieldParams.WIDEFIELD_MAX_STEPS);
            
            if(GlobalWideFieldParams.SourceCommandType != commandSource)
                return;
                
            PositionController.SetUp(new Vector2Int(azimuthStep, 0));
        }
    }
}