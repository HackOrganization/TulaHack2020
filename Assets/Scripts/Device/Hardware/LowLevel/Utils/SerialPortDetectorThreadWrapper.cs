using System;
using System.IO.Ports;
using System.Threading;
using Core;
using Device.Hardware.LowLevel.Utils.Communication;
using UnityAsyncHelper.Wrappers;

namespace Device.Hardware.LowLevel.Utils
{
    /// <summary>
    /// Оболочка потока обноружения порта, к которому подключено устройство
    /// </summary>
    public class SerialPortDetectorThreadWrapper: ThreadWrapperBase, IDisposable
    {
        private const int AwaitResponseTime = 15;

        /// <summary>
        /// Запрос прерван
        /// </summary>
        public bool RequestCanceled { get; private set; }

        /// <summary>
        /// Имя порта
        /// </summary>
        public readonly string PortName;

        private bool _result;
        private int _currentResponseTime;
        private SerialPortController _serialPortController;
        
        public SerialPortDetectorThreadWrapper(string portName, bool sendOnCompletedToMainThread = true) : base(sendOnCompletedToMainThread)
        {
            PortName = portName;
            _serialPortController = SerialPortParams.NewSerialPort(portName);
            _serialPortController.onDetected += Detect;
        }

        /// <summary>
        /// Выполнение асинхронной задачи
        /// </summary>
        protected override void DoTask()
        {
            _serialPortController.Start();
            _serialPortController.Send(CommunicationParams.HELLO_REQUEST);
            
            while (_currentResponseTime++ < AwaitResponseTime)
            {
                if(RequestCanceled)
                    return;
                
                Thread.Sleep(1000);
            }

            _result = false;
        }

        /// <summary>
        /// Заверешение выполнения асинхронной задачи
        /// </summary>
        protected override void OnCompleted()
        {
            if (RequestCanceled && !_result)
            {
                EventManager.RaiseEvent(EventType.HardwareSerialPortDetectionCanceled);
                return;
            }
            
            EventManager.RaiseEvent(EventType.HardwareSerialPortDetected, _result, _serialPortController.PortName);
        }

        /// <summary>
        /// Прерывание выполнение асинхронной задачи 
        /// </summary>
        public void CancelRequest(bool result = false)
        {
            RequestCanceled = true;
            _result = result;
        }

        /// <summary>
        /// Перехватывает событие обнаружения устройства и прерывает выполнение потока
        /// </summary>
        private void Detect()
        {
            CancelRequest(true);
        }
        
        #region DISPOSE

        /// <summary>
        /// Флаг окончания работы.
        /// </summary>
        private bool _isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _serialPortController.onDetected -= Detect;
                    _serialPortController.Dispose();
                }
                _isDisposed = true;
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