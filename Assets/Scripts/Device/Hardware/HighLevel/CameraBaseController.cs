using System;
using System.Collections.Generic;
using Core;
using Core.GameEvents;
using Device.Hardware.HighLevel.Utils;
using Device.Hardware.LowLevel;
using Device.Utils;
using UnityEngine;
using Utils.Extensions;
using EventType = Core.GameEvents.EventType;

namespace Device.Hardware.HighLevel
{
    /// <summary>
    /// Базовый класс высокоуровневого управления устройством поворота
    /// </summary>
    public abstract class CameraBaseController: MonoBehaviour
    {
        /// <summary>
        /// Неопределенные координты (передаются в том случае, если нужно находиться либо в пассивном режиме, либо никуда не нужно двигаться)
        /// </summary>
        protected static readonly Vector2Int PassiveHuntingFlag = new Vector2Int(-1, -1);

        /// <summary>
        /// Тип камеры
        /// </summary>
        public abstract CameraTypes CameraType { get; }
        
        /// <summary>
        /// Готово ли устройство к работе
        /// </summary>
        public bool IsInitialized { get; private set; }
        
        /// <summary>
        /// Текущая позиция устройства (в шагах)
        /// </summary>
        public abstract Vector2Int CurrentPosition { get; }
        
        /// <summary>
        /// Последняя переданная позиция наведения (в пикселях)
        /// </summary>
        public ILastHandledPosition LastHandledPosition { get; protected set; }

        private SerialPortController _serialPortController;
        protected Vector2Int CashedDevicePosition;

        public virtual void Initialize(SerialPortController serialPortController)
        {
            _serialPortController = serialPortController;
            SetSubscription();

            IsInitialized = true;
        }

        /// <summary>
        /// Фиксирует текущую позицию устройства, возвращает ключ в словаре 
        /// </summary>
        public void FixPosition()
        {
            CashedDevicePosition = CurrentPosition;
        }
        
        #region GAMEEVENTS
        
        /// <summary>
        /// Устанавливает подписки на глоабльные события
        /// </summary>
        private void SetSubscription()
        {
            EventManager.AddHandler(EventType.DeviceGoPosition, OnNewPositionCaptured);
        }
        
        /// <summary>
        /// Отписывается от рассылки глоабльных событий
        /// </summary>
        private void ResetSubscription()
        {
            EventManager.RemoveHandler(EventType.DeviceGoPosition, OnNewPositionCaptured);
        }
        
        /// <summary>
        /// Получение команды двигаться в определенную координату 
        /// </summary>
        protected abstract void OnNewPositionCaptured(object[] args);
        
        #endregion
        
        #region DISPOSE

        /// <summary>
        /// Флаг окончания работы.
        /// </summary>
        public bool IsDisposed { get; protected set; }

        protected virtual void Dispose(bool disposing, bool sayGoodbye = true)
        {
            if (!IsDisposed)
            {
                EventManager.RaiseEvent(EventType.EndWork, true);
                IsDisposed = true;
                
                if (disposing)
                {
                    //Это ссылка на объект HardwareController._serialPortController
                    //Вызов _serialPortController.Dispose происходит в HardwareController.OnDisable()
                    ResetSubscription();
                    _serialPortController = null;
                }
            }
        }
        
        /// <summary>
        /// Методы вызова очистки данных класса с оповещением сервера 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}