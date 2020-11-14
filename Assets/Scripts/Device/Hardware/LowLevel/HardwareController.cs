using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using Device.Hardware.HighLevel;
using Device.Hardware.LowLevel.Utils;
using Device.Hardware.LowLevel.Utils.Communication;
using Device.Hardware.LowLevel.Utils.Communication.Infos;
using UnityEngine;

namespace Device.Hardware.LowLevel
{
    /// <summary>
    /// Контроллер управления железом.
    /// Отвечает за передачу команд навигации на порт устройства.
    /// </summary>
    public class HardwareController: MonoBehaviour
    {
        [Header("Camera controllers")] 
        [SerializeField] protected CameraBaseController wideFieldController;
        [SerializeField] protected CameraBaseController tightFieldController;

        /// <summary>
        /// Порт определен, устройства откалиброваны. 
        /// </summary>
        public bool IsInitialized => CameraBaseControllers.All(c => c.IsInitialized);

        /// <summary>
        /// Перечисление наводящихся камер
        /// </summary>
        private IEnumerable<CameraBaseController> CameraBaseControllers => new[]
        {
            wideFieldController, tightFieldController
        };
        
        /// <summary>
        /// Широкопольная наводящаяся камера
        /// </summary>
        public WideFieldCameraController WideFieldCameraController => (WideFieldCameraController)wideFieldController;
        
        /// <summary>
        /// Узкопольная наводящаяся камера
        /// </summary>
        public TightFieldCameraController TightFieldCameraController => (TightFieldCameraController)tightFieldController;

        private bool _isDisabled;
        private bool _calibrateDone;
        private int _wrappersInvokedCount;
        private SerialPortController _serialPortController;
        private readonly List<SerialPortDetectorThreadWrapper> _threadWrappers = new List<SerialPortDetectorThreadWrapper>();

        private WaitUntil _untilPortOpened;
        private WaitForSeconds _loopWait;
        
        #region INITIALIZATION

        public void Initialize()
        {
            _untilPortOpened = new WaitUntil(() => _serialPortController != null && _serialPortController.IsOpened);
            _loopWait = new WaitForSeconds(1f / SerialPortParams.TIMEOUT);
            
            StartSearchingDevice();

            StartCoroutine(CorSendPosition());
        }

        /// <summary>
        /// Начинает поиск устройства по COM-портам
        /// </summary>
        private void StartSearchingDevice()
        {
            foreach (var portName in SerialPort.GetPortNames())
            {
                var wrapper = new SerialPortDetectorThreadWrapper(portName);
                wrapper.onCompleted += OnDetectionCompleted;
                _threadWrappers.Add(wrapper);
                wrapper.Start();
            }
        }

        /// <summary>
        /// Перехватывает событие SerialPortDetectorThreadWrapper.onCompleted
        /// </summary>
        private void OnDetectionCompleted(object sender, SerialPortDetectorEventArgs args)
        {
            if (args.Result)
            {
                Debug.Log($"Found! {args.PortName}");
                
                _serialPortController = SerialPortParams.NewSerialPort(args.PortName);
                _serialPortController.onMessageReceived += OnMessageReceived;
                foreach (var wrapper in _threadWrappers.Where(wr => wr.PortName != args.PortName))
                    wrapper.CancelRequest();

                foreach (var cameraBaseController in CameraBaseControllers)
                    cameraBaseController.Initialize(_serialPortController);
            }

            ((SerialPortDetectorThreadWrapper) sender).onCompleted -= OnDetectionCompleted;

            if (++_wrappersInvokedCount == _threadWrappers.Count)
            {
                foreach (var wrapper in _threadWrappers)
                    wrapper.Dispose();
                
                _threadWrappers.Clear();

                _serialPortController?.Start();
            }
        }
        #endregion

        /// <summary>
        /// Отправляет с периодичностью
        /// </summary>
        private IEnumerator CorSendPosition()
        {
            yield return _untilPortOpened;//new WaitUntil(() => _serialPortController != null && _serialPortController.IsOpened);

            Debug.Log("Start calibration....");
            //_serialPortController.Send(CommunicationParams.GetDefaultSetupMessage());
            _serialPortController.Send(CommunicationParams.GetCalibrationMessage());
            yield return new WaitUntil(()=> _calibrateDone);
            
            //ToDo: не реализован режим пассивного слежения
            while (!_isDisabled)
            {
                var success = false;
                var moveInfosArray = new MoveInfo[CommunicationParams.DEVICES_COUNT];
                var index = 0;
                foreach (var controller in CameraBaseControllers)
                {
                    var enumMoveInfos = controller.LastHandledPosition.ToMoveInfos(ref success);
                    foreach (var mi in enumMoveInfos)
                        moveInfosArray[index++] = mi;
                }
                
                //Если какая-то из координат была обновлена, тогда отправляем команду наведения
                if (success)
                {
                    var moveMessage = CommunicationParams.GetMoveMessage(moveInfosArray);
                    //Debug.Log($"Input detected. Sending command \"{moveMessage}\"");
                    _serialPortController.Send(moveMessage);
                }
                yield return _loopWait;
            }
        }

        /// <summary>
        /// Обработка полученного сообщения 
        /// </summary>
        private void OnMessageReceived(string message)
        {
            switch (message)
            {
                case CommunicationParams.CALIBRATION_RESPONSE:
                    _calibrateDone = true;
                    break;
            }
        }

        private void OnDisable()
        {
            _isDisabled = true;
            StopAllCoroutines();
            
            foreach (var cameraController in CameraBaseControllers)
                cameraController.Dispose();

            foreach (var wrapper in _threadWrappers)
            {
                wrapper.onCompleted -= OnDetectionCompleted;
                wrapper.CancelRequest();
                wrapper.Dispose();
            }
            _threadWrappers.Clear();
            
            _serialPortController?.Dispose();
        }
    }
}