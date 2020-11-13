using System.Collections;
using System.Linq;
using System.Net;
using Core;
using Core.OrderStart;
using Device.Utils;
using Networking.Client;
using Networking.Message;
using Networking.Message.Utils;
using UnityEngine;
using Utils;
using Utils.Extensions;
using EventType = Core.EventType;
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
        
        public void OnStart()
        {
            _untilAllReady = new WaitUntil(()=> deviceControllers.All(d => d.IsReady));
            _loopAwait = new WaitForSeconds(Params.CAPTURE_PER_SECOND);
            
            var deviceConnectionPoints = DeviceConnectionPoints;
            for (var i = 0; i < deviceControllers.Length; i++)
                deviceControllers[i].Initialize(deviceConnectionPoints[i]);

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
            
            //ToDo: КОСТЫЛЬ КОСТЫЛЕЙ
            yield return new WaitForSeconds(1);

            var counter = 0;
            while (!_isDisposed)
            {
                yield return WideFieldDevice.OnSendImageRequest();
                yield return _loopAwait;
                
                //ToDo: поставлено в целях теста отправки одного изображения
                if(++counter == 5)
                    yield break;
            }
        }
    }
}