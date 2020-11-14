using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using Core;
using Device.Hardware.HighLevel;
using Device.Hardware.LowLevel.Utils;
using Device.Hardware.LowLevel.Utils.Communication;
using Device.Utils;
using UnityEngine;
using EventType = Core.EventType;

namespace Device.Hardware.LowLevel
{
    /// <summary>
    /// Контроллер управления железом.
    /// Отвечает за передачу команд навигации на порт устройства.
    /// </summary>
    public class HardwareController: MonoBehaviour
    {
        [Header("Camera controllers")] 
        [SerializeField] private CameraBaseController wideFieldController;
        [SerializeField] private CameraBaseController tightFieldController;
        
        /// <summary>
        /// Порт определен, устройства откалиброваны
        /// </summary>
        public bool IsInitialized { get; private set; }

        public WideFieldCameraController WideFieldCameraController => (WideFieldCameraController)wideFieldController;
        public TightFieldCameraController TightFieldCameraController => (TightFieldCameraController)wideFieldController;

        private int _wrappersInvokedCount;
        private readonly List<SerialPortDetectorThreadWrapper> _threadWrappers = new List<SerialPortDetectorThreadWrapper>();
        private SerialPortController _serialPortController;

        public void Initialize()
        {
            StartSearchingDevice();
        }

        /// <summary>
        /// Начинает поиск устройства по COM-портам
        /// </summary>
        private void StartSearchingDevice()
        {
            EventManager.AddHandler(EventType.HardwareSerialPortDetected, OnPortDetected);
            
            foreach (var portName in SerialPort.GetPortNames())
            {
                var wrapper = new SerialPortDetectorThreadWrapper(portName);
                _threadWrappers.Add(wrapper);
                wrapper.Start();
            }
        }

        /// <summary>
        /// Перехватывает событие HardwareSerialPortDetected
        /// <param name="args">[0] - Result, [1] - PortName</param>>
        /// </summary>
        private void OnPortDetected(object[] args)
        {
            if(_wrappersInvokedCount++ > CommunicationParams.DEVICES_COUNT)
                return;
            
            var result = (bool) args[0];
            var portName = (string) args[1];

            if (result)
            {
                _serialPortController = SerialPortParams.NewSerialPort(portName);
                foreach (var wrapper in _threadWrappers.Where(wr => wr.PortName != portName))
                    wrapper.CancelRequest();

                IsInitialized = true;
                return;
            }
            
            //ToDo: Invoke wrappers Dispose
            //ToDo: Handle HardwareSerialPortDetectionCanceled event
        }
    }
}