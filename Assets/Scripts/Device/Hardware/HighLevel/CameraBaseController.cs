using System;
using System.Collections;
using Core.GameEvents;
using Device.Hardware.HighLevel.Utils;
using Device.Utils;
using UnityEngine;
using EventType = Core.GameEvents.EventType;

namespace Device.Hardware.HighLevel
{
    /// <summary>
    /// Базовый класс высокоуровневого управления устройством поворота
    /// </summary>
    public abstract class CameraBaseController: MonoBehaviour
    {
        /// <summary>
        /// Минимально допустимая координата изображения
        /// </summary>
        protected static readonly Vector2Int MinImagePosition = Vector2Int.zero;
        
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
        public abstract Vector2Int CurrentPosition { get; set; }
        
        /// <summary>
        /// Последняя переданная позиция наведения (в пикселях)
        /// </summary>
        public IPositionController PositionController { get; protected set; }
        
        [HideInInspector]
        public bool updateCurrentPosition;
        
        protected Vector2Int CashedDevicePosition;
        private Vector2Int _handledPosition;

        public virtual void Initialize()
        {
            SetSubscription();
            StartCoroutine(EUpdateCurrentPosition());
            
            IsInitialized = true;
        }

        /// <summary>
        /// Фиксирует текущую позицию устройства, возвращает ключ в словаре 
        /// </summary>
        public void CashPosition()
        {
            CashedDevicePosition = _handledPosition;
        }

        /// <summary>
        /// Обновляет текущую позицию по расчетам 
        /// </summary>
        private IEnumerator EUpdateCurrentPosition()
        {
            while (!IsDisposed)
            {
                if (updateCurrentPosition)
                    CurrentPosition = PositionController.UpdateCurrentPosition(CurrentPosition, ref updateCurrentPosition);
                
                yield return null;
            }
        }
        
        #region GAMEEVENTS
        
        /// <summary>
        /// Устанавливает подписки на глоабльные события
        /// </summary>
        private void SetSubscription()
        {
            EventManager.AddHandler(EventType.DeviceGoPosition, OnNewPositionCaptured);
            EventManager.AddHandler(EventType.DeviceHandlePosition, OnHandlePosition);
        }
        
        /// <summary>
        /// Отписывается от рассылки глоабльных событий
        /// </summary>
        private void ResetSubscription()
        {
            EventManager.RemoveHandler(EventType.DeviceGoPosition, OnNewPositionCaptured);
            EventManager.RemoveHandler(EventType.DeviceHandlePosition, OnHandlePosition);
        }
        
        /// <summary>
        /// Получение команды двигаться в определенную координату 
        /// </summary>
        protected abstract void OnNewPositionCaptured(object[] args);

        /// <summary>
        /// Фиксирует позицию последнего сохраненного кадра 
        /// </summary>
        private void OnHandlePosition(object[] args)
        {
            if((CameraTypes) args[0] != CameraType)
                return;

            _handledPosition = CurrentPosition;
        }
        
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
                    ResetSubscription();
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