using System.Collections;
using System.Linq;
using System.Net;
using Core.OrderStart;
using Device.Hardware.LowLevel;
using Device.Utils;
using UnityEngine;
using Utils;
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
        private WaitUntil _untilAllReady;
        private WaitForSeconds _loopAwait;
        
        /// <summary>
        /// Функция последовательной инициализации
        /// </summary>
        public void OnStart()
        {
            _untilAllReady = new WaitUntil(() => 
                deviceControllers.All(d => d.IsReady) 
                && hardwareController.IsInitialized);
            _loopAwait = new WaitForSeconds(Params.CAPTURE_PER_SECOND);
            
            var deviceConnectionPoints = DeviceConnectionPoints;
            for (var i = 0; i < deviceControllers.Length; i++)
                deviceControllers[i].Initialize(deviceConnectionPoints[i]);
            
            hardwareController.Initialize();

            if (autoRun)
                Run();
        }

        private void OnDisable()
        {
            StopCoroutine(CorRun());
            _isDisposed = true;
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

            var counter = 0;
            while (!_isDisposed)
            {
                yield return new WaitForEndOfFrame();

                var packetId = hardwareController.WideFieldCameraController.FixPosition();
                WideFieldDevice.OnSendImageRequest(packetId);
                yield return _loopAwait;
                
                //ToDo: поставлено в целях теста отправки одного изображения
                if(++counter == 5)
                    yield break;
            }
        }
    }
}