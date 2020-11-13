using UnityEngine;

namespace Device.Hardware
{
    public abstract class HardwareController: MonoBehaviour
    {
        protected static readonly Vector2 PassiveHuntingFlag = new Vector2(-1f, -1f);

        /// <summary>
        /// Текущая позиция устройства
        /// </summary>
        public abstract Vector2 CurrentPosition { get; }
        protected Vector2 LastHandledPosition;

        public void Initialize()
        {
            //ToDo: Hadle from device current position
            Debug.Log("Hardware inited");
        }

        /// <summary>
        /// Получение команды движгаться в определнную координату 
        /// </summary>
        protected abstract void OnNewPositionCaptured(object[] args);

        /// <summary>
        /// Режим активного наведения 
        /// </summary>
        protected abstract void ActiveHunting(Vector2 value);
    }
}