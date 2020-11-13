using UnityEngine;

namespace Device.Hardware
{
    /// <summary>
    /// Контроллер управления железом широкоугольной камеры
    /// </summary>
    public class WideFieldCameraHardwareController: HardwareController
    {
        /// <summary>
        /// Текущая позиция устройства
        /// </summary>
        public override Vector2 CurrentPosition => new Vector2(_currentPosition, 0);

        /// <summary>
        /// По часовой стрелке
        /// </summary>
        private bool _isClockwise;
        private float _currentPosition;
        
        /// <summary>
        /// Получение команды движгаться в определнную координату 
        /// </summary>
        protected override void OnNewPositionCaptured(object[] args)
        {
            var value = (Vector2) args[0];
            if(value == PassiveHuntingFlag)
                PassiveHunting();
            else
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
        
        /// <summary>
        /// Режим пасивной слежки
        /// </summary>
        private void PassiveHunting()
        {
            var sign = _isClockwise ? 1f : -1f;
            //ToDo: Rotate -> _currentPosition
            //ToDo: handle min/max position to change rotation
        }
    }
}