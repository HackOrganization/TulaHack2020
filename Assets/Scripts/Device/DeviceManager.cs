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
        private WaitForEndOfFrame _endOfFrame;
        
        public void OnStart()
        {
            EventManager.AddHandler(EventType.ReceivedMessage, OnMessageReceived);
            
            _untilAllReady = new WaitUntil(()=> deviceControllers.All(d => d.IsReady));
            _loopAwait = new WaitForSeconds(Params.CAPTURE_PER_SECOND);
            _endOfFrame = new WaitForEndOfFrame();
            
            var deviceConnectionPoints = DeviceConnectionPoints;
            for (var i = 0; i < deviceControllers.Length; i++)
                deviceControllers[i].Initialize(deviceConnectionPoints[i]);

            if (autoRun)
                Run();
        }

        private void OnDisable()
        {
            EventManager.RemoveHandler(EventType.ReceivedMessage, OnMessageReceived);
            
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

            while (!_isDisposed)
            {
                yield return _endOfFrame;
                
                WideFieldDevice.OnSendImageRequest();
                yield return _loopAwait;
                
                //ToDo: поставлено в целях теста отправки одного изображения
                yield break;
            }
        }
        
        /// <summary>
        /// Перехватывает событие получения сообщения
        /// <param name="args">MessageType, IMessage, AsynchronousClient</param>
        /// </summary>
        private static void OnMessageReceived(object[] args)
        {
            var messageType = (MessageType) args[0]; 
            if(!messageType.GetMessageDestination().IsClientSupported())
                return;

            //ToDo: Check client equality by index
            var client = (AsynchronousClient) args[2];
            
            switch (messageType)
            {
                case MessageType.CloseConnection:
                    client.SafeDispose();
                    break;
                
                case MessageType.WideFieldPosition:
                    OnWideFieldPositionCaught(client, (WideFieldPositionMessage) args[1]);
                    break;
                
                case MessageType.TightFieldPosition:
                    Debug.Log("TightFieldPosition caught");
                    break;
                
                default: return;
            }
        }
        
        #region SERVER RESPONSES

        private static void OnWideFieldPositionCaught(AsynchronousClient client, WideFieldPositionMessage message)
        {
            Debug.Log($"[Client] WideField[{message.PacketId}]: take position \"{message.Position}\"");
        }
        
        #endregion
    }
}