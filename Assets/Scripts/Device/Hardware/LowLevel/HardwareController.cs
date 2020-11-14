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
        /// Порт определен, устройства откалиброваны, объект не разрушен 
        /// </summary>
        public bool IsReady => CameraBaseControllers.All(c => c.IsInitialized && !c.IsDisposed);
        /// <summary>
        /// Перечисление наводящихся камер
        /// </summary>
        private CameraBaseController[] CameraBaseControllers => new[]
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
        private bool _positionRequested;
        private int _wrappersInvokedCount;
        private SerialPortController _serialPortController;
        private TrackModeController _trackModeController = new TrackModeController();
        private readonly List<SerialPortDetectorThreadWrapper> _threadWrappers = new List<SerialPortDetectorThreadWrapper>();

        private WaitUntil _untilPortOpened;
        private WaitForSeconds _loopWait;
        
        #region INITIALIZATION

        public void Initialize()
        {
            _untilPortOpened = new WaitUntil(() => _serialPortController != null && _serialPortController.IsOpened);
            _loopWait = new WaitForSeconds(1f / SerialPortParams.TIMEOUT);

            //ToDo: Comment TEST_LockSearchingDevice, uncomment StartSearchingDevice
            //TEST_LockSearchingDevice();
            StartSearchingDevice();

            StartCoroutine(CorSendPosition());
        }

        private void TEST_LockSearchingDevice()
        {
            
            _serialPortController = SerialPortParams.NewSerialPort(SerialPort.GetPortNames()[0]);
            _serialPortController.onMessageReceived += OnMessageReceived;
            
            foreach (var cameraBaseController in CameraBaseControllers)
                cameraBaseController.Initialize(_serialPortController);
            
            _serialPortController.Start();
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
        /// Это событие как определяет порт нужного устройства, так и фиксирует порты других устройств
        /// </summary>
        private void OnDetectionCompleted(object sender, SerialPortDetectorEventArgs args)
        {
            if (args.Result)
            {
                Debug.Log($"Found! {args.Controller}");

                _serialPortController = args.Controller;
                _serialPortController.onMessageReceived += OnMessageReceived;
                foreach (var wrapper in _threadWrappers.Where(wr => !wr.RequestCanceled))
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
            }
        }
        #endregion

        /// <summary>
        /// Основной цикл работы устройства наведения
        /// дожидается инициалиации порта, калбировки устройства и отправляет комманды наведения, если таковые имеются
        /// </summary>
        private IEnumerator CorSendPosition()
        {
            yield return _untilPortOpened;

            Debug.Log("Start calibration....");
            _serialPortController.Send(CommunicationParams.GetCalibrationMessage());
            yield return new WaitUntil(()=> _calibrateDone);
            
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
                    _serialPortController.Send(moveMessage);
                    _trackModeController.Reset();
                }
                else if (!_trackModeController.SetUp())
                {
                    _positionRequested = true;
                    _serialPortController.Send(CommunicationParams.GetPositionRequestMessage());    
                }
                yield return _loopWait;
                
                if(_positionRequested)
                    yield return new WaitUntil(()=> !_positionRequested);
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
            
            //ToDo: BEFORE TEST handle position response
        }

        /// <summary>
        /// Устанавливает новые позиции камер (в шагах) на основании ответа от контроллера 
        /// </summary>
        private void SetUpNewPositions(in Vector2Int[] newPositions)
        {
            var cameraControllers = CameraBaseControllers;
            for (var i = 0; i < cameraControllers.Length; i++)
                cameraControllers[i].CurrentPosition = newPositions[i];
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