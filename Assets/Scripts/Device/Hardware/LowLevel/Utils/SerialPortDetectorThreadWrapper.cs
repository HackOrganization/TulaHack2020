using System;
using System.Threading;
using Device.Hardware.LowLevel.Utils.Communication;
using UnityAsyncHelper.Wrappers;

namespace Device.Hardware.LowLevel.Utils
{
    /// <summary>
    /// Оболочка потока обноружения порта, к которому подключено устройство
    /// </summary>
    public class SerialPortDetectorThreadWrapper: ThreadWrapperBase, IDisposable
    {
        /// <summary>
        /// Событие заверешения работы по идентификации устройства
        /// </summary>
        public event EventHandler<SerialPortDetectorEventArgs> onCompleted = (sender, args) => { };
        
        private const int AwaitResponseTime = 15;

        /// <summary>
        /// Запрос прерван
        /// </summary>
        public bool RequestCanceled { get; private set; }

        private bool _result;
        private int _currentResponseTime;
        private SerialPortController _serialPortController;
        
        public SerialPortDetectorThreadWrapper(string portName, bool sendOnCompletedToMainThread = true) : base(sendOnCompletedToMainThread)
        {
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
            onCompleted(this, new SerialPortDetectorEventArgs(_result, _serialPortController));
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
                    onCompleted = null;
                    _serialPortController.onDetected -= Detect;
                    if (_result)
                        _serialPortController = null;
                    else
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