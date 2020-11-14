using Core.GameEvents;
using Core.MathConversion;
using Device.Hardware.HighLevel.Utils;
using Device.Hardware.LowLevel;
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
        /// Тип камеры
        /// </summary>
        public override CameraTypes CameraType => CameraTypes.WideField;

        /// <summary>
        /// Текущая позиция устройства (в шагах)
        /// </summary>
        public override Vector2Int CurrentPosition => new Vector2Int(_currentPosition, 0);

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
            if (!((Vector2) position).IsNullPosition())
            {
                var positionOnImage = position.DelayedImageHorizontalPosition(CurrentPosition, CashedDevicePosition);
                position = position.HorizontalPosition(CashedDevicePosition);
                
                EventManager.RaiseEvent(EventType.CameraDrawObject, CameraTypes.WideField, positionOnImage, args[2]);
            }
            
            //ToDo: uncomment to navigate
            //LastHandledPosition.SetUp(position);
        }
    }
}