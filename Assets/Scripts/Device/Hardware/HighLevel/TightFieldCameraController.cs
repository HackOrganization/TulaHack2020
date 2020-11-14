using UnityEngine;

namespace Device.Hardware.HighLevel
{
    /// <summary>
    /// Контроллер управления железом узкопольной камеры
    /// </summary>
    public class TightFieldCameraController: CameraBaseController
    {
        /// <summary>
        /// Текущая позиция устройства
        /// </summary>
        public override Vector2 CurrentPosition => _currentPosition;
        
        private Vector2 _currentPosition;
        
        /// <summary>
        /// Получение команды движгаться в определнную координату 
        /// </summary>
        protected override void OnNewPositionCaptured(object[] args)
        {
            var value = (Vector2) args[0];
            if(value == PassiveHuntingFlag)
                return;
            
            ActiveHunting(value);
        }

        /// <summary>
        /// Режим активного наведения 
        /// </summary>
        protected override void ActiveHunting(Vector2 value)
        {
            LastHandledPosition = new Vector2(value.x, 0);
            //ToDo: rotate
        }
    }
}