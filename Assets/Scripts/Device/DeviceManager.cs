using System.Collections;
using System.Linq;
using System.Net;
using Core.GameEvents;
using Core.OrderStart;
using Device.Hardware.LowLevel;
using Device.Utils;
using UI;
using UnityEngine;
using Utils;
using EventType = Core.GameEvents.EventType;
using NetworkingParams = Networking.Utils.Params;

namespace Device
{
    /// <summary>
    /// Менеджер управления устройствами
    /// </summary>
    public class DeviceManager: Singleton<DeviceManager>, IStarter
    {
        [Header("Settings")] 
        [SerializeField] private bool autoRun;
        
        [Header("Контроллеры устройств")]
        [SerializeField] private DeviceController[] deviceControllers;
        
        [Header("Hardware info")] 
        [SerializeField] private HardwareController hardwareController;

        /// <summary>
        /// Точки подключения устройств для обмена сообщениями
        /// </summary>
        private static IPEndPoint[] DeviceConnectionPoints => new[]
        {
            NetworkingParams.WideFieldEndPoint
        };

        /// <summary>
        /// Получает котроллер управления широкоугольной камеры
        /// </summary>
        private DeviceController WideFieldDevice 
            => deviceControllers.First(d => d.cameraType == CameraTypes.WideField);

        private bool _isDisposed;
        private bool _captureLocked;
        private WaitUntil _untilAllReady;
        private WaitUntil _notCaptureLocked;
        private DebugController _debugController;
        
        /// <summary>
        /// Функция последовательной инициализации
        /// </summary>
        public void OnStart()
        {
            _debugController = FindObjectOfType<DebugController>();
            
            Application.targetFrameRate = 300;
            SetSubscription();
            
            _untilAllReady = new WaitUntil(ComponentsAreReady);
            _notCaptureLocked = new WaitUntil(()=> !_captureLocked);
            
            var deviceConnectionPoints = DeviceConnectionPoints;
            for (var i = 0; i < deviceControllers.Length; i++)
                deviceControllers[i].Initialize(deviceConnectionPoints[i]);
            
            hardwareController.Initialize();

            if (autoRun)
                Run();
        }
        
        private void OnDisable()
        {
            ResetSubscription();
            StopCoroutine(CorRun());
            _isDisposed = true;
        }

        /// <summary>
        /// Все компоненты инициализированы и не разрушены 
        /// </summary>
        private bool ComponentsAreReady()
        {
            return deviceControllers.All(d => d.IsReady) && hardwareController.IsReady;
        }

        /// <summary>
        /// Запускает цикличную работу устройств
        /// </summary>
        public void Run()
        {
            StartCoroutine(CorRun());
        }
        
        /// <summary>
        /// С периодичностью Params.CAPTURE_PER_SECOND запускает работу устройств
        /// </summary>
        private IEnumerator CorRun()
        {
            yield return _untilAllReady;
            
            //КОСТЫЛЬ КОСТЫЛЕЙ
            yield return new WaitForSeconds(1);

            while (!_isDisposed)
            {
                _captureLocked = true;
                
                hardwareController.WideFieldCameraController.CashPosition();
                WideFieldDevice.OnSendImageRequest();
                
                _debugController.Log();
                Debug.Log("Image sent");
                yield return _notCaptureLocked;
                
                if(!ComponentsAreReady())
                    //ToDo: dispose all controllers (Neural, AsyncClient, Hardware...)
                    yield break;
            }
        }

        #region GAMEEVENTS

        /// <summary>
        /// Устанавливает подписки на глоабльные события
        /// </summary>
        private void SetSubscription()
        {
            EventManager.AddHandler(EventType.EndWork, OnEndWork);
            EventManager.AddHandler(EventType.CaptureNewImage, OnWideFieldMessageResponse);
        }

        
        /// <summary>
        /// Отписывается от рассылки глоабльных событий
        /// </summary>
        private void ResetSubscription()
        {
            EventManager.RemoveHandler(EventType.EndWork, OnEndWork);
            EventManager.RemoveHandler(EventType.CaptureNewImage, OnWideFieldMessageResponse);
        }
        
        /// <summary>
        /// Перехватывает событие получения ответа на отправленный кадр ШПК 
        /// </summary>
        private void OnWideFieldMessageResponse(object[] args)
        {
            if((CameraTypes)args[0] != CameraTypes.WideField)
                return;
            
            _captureLocked = false;
        }
        
        /// <summary>
        /// перехватывает сообщение об окончании работы 
        /// </summary>
        private void OnEndWork(params object[] args)
        {
            _isDisposed = (bool) args[0];
        }
        
        #endregion
    }
}