using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.GameEvents;
using Core.OrderStart;
using Device.Hardware.LowLevel;
using Device.Utils;
using Device.Video;
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
        [Header("Контроллеры устройств")] 
        [SerializeField] private WideFieldDeviceController wideFieldDeviceController;
        [SerializeField] private TightFieldDeviceController tightFieldDeviceController;
        
        [Header("Hardware info")] 
        [SerializeField] private HardwareController hardwareController;

        /// <summary>
        /// Возвращает массив контроллеров управления устройствами
        /// </summary>
        private IEnumerable<DeviceController> DeviceControllers => new DeviceController[]
        {
            wideFieldDeviceController,
            tightFieldDeviceController
        };

        private bool _isDisposed;
        private bool _captureLocked;
        private bool _debugInitialized;
        private WaitUntil _untilAllReady;
        private WaitUntil _notCaptureLocked;
        private DebugController _debugController;
        
        /// <summary>
        /// Функция последовательной инициализации
        /// </summary>
        public void OnStart()
        {
            _debugController = FindObjectOfType<DebugController>();
            _debugInitialized = _debugController != null;
            
            Application.targetFrameRate = 300;
            SetSubscription();
            
            _untilAllReady = new WaitUntil(ComponentsAreReady);
            _notCaptureLocked = new WaitUntil(()=> !_captureLocked);

            foreach (var deviceController in DeviceControllers)
                deviceController.Initialize();
            hardwareController.Initialize();

            StartCoroutine(EWideFiledRun());
        }
        
        private void OnDisable()
        {
            ResetSubscription();
            StopCoroutine(EWideFiledRun());
            _isDisposed = true;
        }

        /// <summary>
        /// Все компоненты инициализированы и не разрушены 
        /// </summary>
        private bool ComponentsAreReady()
        {
            return DeviceControllers.All(d => d.IsReady) && hardwareController.IsReady;
        }
        
        /// <summary>
        /// По готовности Нейронки отправляет последний захваченный кадр
        /// </summary>
        private IEnumerator EWideFiledRun()
        {
            yield return _untilAllReady;
            
            //КОСТЫЛЬ КОСТЫЛЕЙ
            yield return new WaitForSeconds(1);

            while (!_isDisposed)
            {
                _captureLocked = true;
                
                hardwareController.WideFieldHighLevelController.CashPosition();
                wideFieldDeviceController.OnSendImageRequest();
                
                if(_debugInitialized)
                    _debugController.Log();
                yield return _notCaptureLocked;

                if (!ComponentsAreReady())
                {
                    Dispose();
                    yield break;
                }   
            }
        }

        private void Dispose()
        {
            EventManager.RaiseEvent(EventType.EndWork, true);
            hardwareController.Dispose();
            foreach (var deviceController in DeviceControllers)
                deviceController.Dispose();

            ResetSubscription();
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