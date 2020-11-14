using Core.GameEvents;
using Core.MathConversion;
using Core.MathConversion.Utils;
using Device.Hardware.HighLevel.Utils;
using Device.Hardware.LowLevel;
using Device.Hardware.LowLevel.Utils.Communication;
using Device.Utils;
using UnityEngine;
using Utils.Extensions;
using EventType = Core.GameEvents.EventType;

namespace Device.Hardware.HighLevel
{
    /// <summary>
    /// Высокоуровневое управление устройством поворота широкопольной камеры
    /// </summary>
    public class WideFieldCameraController: CameraBaseController
    {
        /// <summary>
        /// Максимально допустимый размер изображения
        /// </summary>
        private static readonly Vector2Int MaxImagePosition = new Vector2Int(WideFieldParams.WIDTH, WideFieldParams.HEIGHT);
        
        private static readonly Vector2Int MaxStepValue = new Vector2Int(CommunicationParams.WIDEFIELD_FULL_LOOP_STEPS, 0);
        
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

        public override void Initialize(SerialPortController serialPortController)
        {
            LastHandledPosition = new WideFieldLastHandledPosition();
            base.Initialize(serialPortController);
        }

        /// <summary>
        /// Обновление координаты наведения 
        /// </summary>
        protected override void OnNewPositionCaptured(object[] args)
        {
            var cameraType = (CameraTypes) args[0];
            if(CameraType != cameraType)
                return;

            var position = (Vector2Int) args[1];
            if (!position.IsNullPosition())
            {
                var positionOnImage = position.DelayedImageHorizontalPosition(CurrentPosition, CashedDevicePosition);
                positionOnImage.Clamp(MinImagePosition, MaxImagePosition);
                EventManager.RaiseEvent(EventType.CameraDrawObject, CameraTypes.WideField, positionOnImage, args[2]);
                
                position = position.HorizontalPosition(CashedDevicePosition);
                position.Clamp(MinStepValue, MaxStepValue);
            }
            
            LastHandledPosition.SetUp(position);
        }
    }
}