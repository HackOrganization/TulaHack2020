using System;
using System.IO.Ports;
using System.Threading;
using Device.Hardware.LowLevel.Utils.Communication;
using UnityAsyncHelper.Core;
using UnityEngine;

namespace Device.Hardware.LowLevel
{
    /// <summary>
    /// Контроллер обмена данными через COM-порт
    /// </summary>
    public class SerialPortController: IDisposable
    {
        /// <summary>
        /// Событие обнаружения устройства (при получении ответа на HELLO-Respose)
        /// </summary>
        public event Action onDetected = () => { }; 
        
        /// <summary>
        /// Открыт ли порт для работы с устройством
        /// </summary>
        public bool IsOpened { get; private set; }
        
        /// <summary>
        /// Имя порта
        /// </summary>
        public string PortName => _serialPort.PortName;
        
        private StringComparer _stringComparer = StringComparer.OrdinalIgnoreCase;
        private readonly SerialPort _serialPort;
        private readonly Thread _readThread;

        public SerialPortController(SerialPort serialPort)
        {
            _serialPort = serialPort;
            _readThread = new Thread(Read)
            {
                IsBackground = true
            };
        }

        /// <summary>
        /// Запускает общение с устройством через порт
        /// </summary>
        public void Start()
        {
            _serialPort.Open();
            _readThread.Start();

            IsOpened = true;
        }

        /// <summary>
        /// Отправка сообщения через порт (в основном потоке)
        /// </summary>
        public void Send(string message)
        {
            _serialPort.WriteLine(message);
        }

        /// <summary>
        /// Чтение данных с порта в фоновом потоке
        /// </summary>
        private void Read()
        {
            while (!_isDisposed)
            {
                try
                {
                    var message = _serialPort.ReadLine();
                    ThreadManager.ExecuteOnMainThread(() => OnMessageRead(message));
                }
                catch (TimeoutException toEx)
                {

                }
                catch (ThreadAbortException thaEx)
                {
                    Debug.Log($"SerialPort ReadThread {Thread.CurrentThread.ManagedThreadId} aborted");
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        /// <summary>
        /// Обрабатывает прочитанное в основном потоке сообщение (обработка происходит в основном потоке) 
        /// </summary>
        private void OnMessageRead(string message)
        {
            if (_stringComparer.Equals(CommunicationParams.HELLO_RESPONSE, message))
            {
                onDetected();
                return;
            }
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
                    IsOpened = false;
                    onDetected = null;
                    
                    _stringComparer = null;
                    _readThread.Abort();
                    _serialPort.Close();
                    _serialPort.Dispose();
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