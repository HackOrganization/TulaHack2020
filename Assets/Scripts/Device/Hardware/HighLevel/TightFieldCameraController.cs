using Device.Hardware.HighLevel.Utils;
using Device.Hardware.LowLevel;
using Device.Utils;
using UnityEngine;

namespace Device.Hardware.HighLevel
{
    /// <summary>
    /// Высокоуровневое управление устройством поворота узкопольной камеры
    /// </summary>
    public class TightFieldCameraController: CameraBaseController
    {
        /// <summary>
        /// Тип камеры
        /// </summary>
        public override CameraTypes CameraType => CameraTypes.TightField;
        
        /// <summary>
        /// Текущая позиция устройства (в шагах)
        /// </summary>
        public override Vector2Int CurrentPosition => _currentPosition;
        
        private Vector2Int _currentPosition;
        
        public override void Initialize(SerialPortController serialPortController)
        {
            LastHandledPosition = new TightFieldLastHandledPosition();
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

            LastHandledPosition.SetUp((Vector2Int) args[2]);
        }
    }
}