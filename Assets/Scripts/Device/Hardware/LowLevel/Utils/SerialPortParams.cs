﻿using System.IO.Ports;
using Device.Hardware.LowLevel.Controllers;

namespace Device.Hardware.LowLevel.Utils
{
    public static class SerialPortParams
    {
        /// <summary>
        /// Бит в секунду
        /// </summary>
        public const int BAUD_RATE = 115200;

        /// <summary>
        /// Четность
        /// </summary>
        public const Parity PARITY = Parity.None;

        /// <summary>
        /// Биты данных
        /// </summary>
        public const int DATA_BITS = 8;

        /// <summary>
        /// Стоповый бит
        /// </summary>
        public const StopBits STOP_BIT = StopBits.One;

        /// <summary>
        /// время ожидания на чтение запись (милисекунды)
        /// </summary>
        public const int TIMEOUT = 50;

        /// <summary>
        /// Создает новый компонент SerialPortController с дефолтными настройками
        /// </summary>
        public static SerialPortController NewSerialPort(string name)
        {
            var serialPort = new SerialPort(name, BAUD_RATE, PARITY, DATA_BITS, STOP_BIT)
            {
                ReadTimeout = TIMEOUT, WriteTimeout = TIMEOUT
            };

            return new SerialPortController(serialPort);
        }
    }
}