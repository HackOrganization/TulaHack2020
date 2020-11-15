using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using Device.Hardware.HighLevel;
using Device.Hardware.LowLevel.Controllers;
using Device.Hardware.LowLevel.Utils.Communication;
using Device.Hardware.LowLevel.Utils.Communication.Infos;
using UnityEngine;
using Utils;

using LowLevelUtils = Device.Hardware.LowLevel.Utils;

namespace Device.Hardware.LowLevel
{
    /// <summary>
    /// Контроллер управления железом.
    /// Отвечает за передачу команд навигации на порт устройства.
    /// </summary>
    public class HardwareController: Singleton<HardwareController>
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
        protected CameraBaseController[] CameraBaseControllers => new[]
        {
            wideFieldController, tightFieldController
        };
        
        /// <summary>
        /// Широкопольная наводящаяся камера
        /// </summary>
        public WideFieldHighLevelController WideFieldHighLevelController => (WideFieldHighLevelController)wideFieldController;
        
        /// <summary>
        /// Узкопольная наводящаяся камера
        /// </summary>
        public TightFieldHighLevelController TightFieldHighLevelController => (TightFieldHighLevelController)tightFieldController;

        protected bool _isDisabled;
        protected bool _calibrateDone;
        protected bool _positionRequested;
        private int _wrappersInvokedCount;
        protected SerialPortController _serialPortController;
        protected readonly TrackModeController _trackModeController = new TrackModeController();
        private readonly List<LowLevelUtils.SerialPortDetectorThreadWrapper> _threadWrappers = new List<LowLevelUtils.SerialPortDetectorThreadWrapper>();

        protected WaitUntil _untilPortOpened;
        protected WaitForSeconds _loopWait;
        
        #region INITIALIZATION

        public void Initialize()
        {
            _untilPortOpened = new WaitUntil(() => _serialPortController != null && _serialPortController.IsOpened);
            _loopWait = new WaitForSeconds(1f / LowLevelUtils.SerialPortParams.TIMEOUT);

            StartSearchingDevice();
            StartCoroutine(EWork());
        }
        
        /// <summary>
        /// Начинает поиск устройства по COM-портам
        /// </summary>
        private void StartSearchingDevice()
        {
            foreach (var portName in SerialPort.GetPortNames())
            {
                Debug.Log($"");
                var wrapper = new LowLevelUtils.SerialPortDetectorThreadWrapper(portName);
                wrapper.onCompleted += OnDetectionCompleted;
                _threadWrappers.Add(wrapper);
                wrapper.Start();
            }
        }

        /// <summary>
        /// Перехватывает событие SerialPortDetectorThreadWrapper.onCompleted
        /// Это событие как определяет порт нужного устройства, так и фиксирует порты других устройств
        /// </summary>
        private void OnDetectionCompleted(object sender, LowLevelUtils.SerialPortDetectorEventArgs args)
        {
            if (args.Result)
            {
                Debug.Log($"Found! {args.Controller}");

                _serialPortController = args.Controller;
                _serialPortController.onMessageReceived += OnMessageReceived;
                foreach (var wrapper in _threadWrappers.Where(wr => !wr.RequestCanceled))
                    wrapper.CancelRequest();

                foreach (var cameraBaseController in CameraBaseControllers)
                    cameraBaseController.Initialize();
            }

            ((LowLevelUtils.SerialPortDetectorThreadWrapper) sender).onCompleted -= OnDetectionCompleted;

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
        protected virtual IEnumerator EWork()
        {
            yield return _untilPortOpened;

            _serialPortController.Send(CommunicationParams.GetCalibrationMessage());
            yield return new WaitUntil(()=> _calibrateDone);
            
            while (!_isDisabled)
            {
                var success = false;
                var moveInfosArray = new MoveInfo[LowLevelUtils.Params.DEVICES_COUNT];
                var index = 0;
                foreach (var controller in CameraBaseControllers)
                {
                    var enumMoveInfos = controller.PositionController.TowardsInfos(ref success);
                    foreach (var mi in enumMoveInfos)
                        moveInfosArray[index++] = mi;
                }
                
                if (success)
                {
                    //Если какая-то из координат была обновлена, тогда отправляем команду наведения
                    var moveMessage = CommunicationParams.GetMoveMessage(moveInfosArray);
                    _serialPortController.Send(moveMessage);
                    _trackModeController.Reset();
                }
                else if (!_trackModeController.SetUp())
                {
                    //если режим слежения не нужно запускать, то запрашивает текущие координаты для уточнения
                    _positionRequested = true;
                    _serialPortController.Send(CommunicationParams.GetPositionRequestMessage());    
                }
                
                foreach (var controller in CameraBaseControllers)
                    controller.updateCurrentPosition = success;
                
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
            if (message.Equals(CommunicationParams.CALIBRATION_RESPONSE))
            {
                _calibrateDone = true;
            }
            else if (message.StartsWith(CommunicationParams.POSITION_FLAG.ToString()))
            {
                var newPositions = CommunicationParams.ParsePositionResponse(message);
                SetUpNewPositions(in newPositions);
                _positionRequested = false;
            }
        }

        /// <summary>
        /// Устанавливает новые позиции камер (в шагах) на основании ответа от контроллера 
        /// </summary>
        protected virtual void SetUpNewPositions(in Vector2Int[] newPositions)
        {
            var cameraControllers = CameraBaseControllers;
            for (var i = 0; i < cameraControllers.Length; i++)
                cameraControllers[i].CurrentPosition = newPositions[i];
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
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
        
        private void OnDisable()
        {
            Dispose();
        }
    }
}