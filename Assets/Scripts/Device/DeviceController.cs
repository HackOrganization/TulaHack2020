using System.Collections;
using System.Collections.Generic;
using System.Net;
using Core;
using Device.Hardware;
using Device.Utils;
using Device.Video;
using Networking.Client;
using Networking.Message;
using Networking.Message.Utils;
using UnityEngine;
using UnityEngine.Windows;
using Utils.Extensions;
using EventType = Core.EventType;

namespace Device
{
    /// <summary>
    /// Контроллер управления устройством 
    /// </summary>
    public class DeviceController : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool execute;
        
        [Header("Camera info")] 
        public CameraTypes cameraType;
        [SerializeField] private VideoHandler videoHandler;

        [Header("Hardware info")] 
        [SerializeField] private HardwareController hardwareController;

        /// <summary>
        /// Фиксирует, что устройство готово к работе.
        /// Для этого необходиом, чтобы было создано соединение с сервером и камера подключилась
        /// </summary>
        public bool IsReady => _clientConnected && videoHandler.IsAuthorized;
        
        private bool _clientConnected;
        private AsynchronousClient _client;
        private readonly Dictionary<ushort, Vector2> _framePositionMap = new Dictionary<ushort, Vector2>(); 
        
        /// <summary>
        /// Инициализация все компонентов устройства 
        /// </summary>
        public void Initialize(IPEndPoint endPoint)
        {
            if (!execute)
                return;
            
            EventManager.AddHandler(EventType.ClientConnected, OnClientConnected);
            
            videoHandler.Initialize(cameraType);
            hardwareController.Initialize();
            _client = AsynchronousClient.Connect(endPoint);
        }
        
        /// <summary>
        /// При подключении клиента 
        /// </summary>
        private void OnClientConnected(object[] args)
        {
            if(!(bool) args[0])
                return;

            var localPoint = (IPEndPoint) args[1];
            var remotePoint = (IPEndPoint) args[2];
            
            if(!((IPEndPoint)_client.Socket.LocalEndPoint).Equals(localPoint))
                return;
            
            Debug.Log($"[Client] Connected: {localPoint} -> {remotePoint}");
            _client.Receive();
            _clientConnected = true;
        }

        /// <summary>
        /// Перехватывает команду отправки изображения
        /// </summary>
        public IEnumerator OnSendImageRequest()
        {
            if(videoHandler.Status != VideoStatuses.Play)
                if(!videoHandler.Play())
                    yield return null;
            
            yield return videoHandler.Capture();
            var newIndex = _framePositionMap.Keys.GetNextFree();
            _framePositionMap.Add(newIndex, hardwareController.CurrentPosition);

            var newMessage = new ImageMessage(
                cameraType == CameraTypes.WideField
                    ? MessageType.WideFieldImage
                    : MessageType.TightFieldImage)
            {
                PacketId = newIndex,
                Image = videoHandler.SendFrame
            };

            File.WriteAllBytes($@"C:\tmp\clientImage{newMessage.PacketId}.jpg", newMessage.Image.EncodeToJPG());
            _client.Send(newMessage.Serialize());
        }

        private void OnDisable()
        {
            EventManager.RemoveHandler(EventType.ClientConnected, OnClientConnected);
            
            _client?.Dispose();
        }
    }
}