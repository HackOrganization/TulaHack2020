using System.Collections;
using System.Collections.Generic;
using System.Net;
using Core;
using Device.Hardware;
using Device.Networking;
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
        public bool IsReady => _client.Connected && videoHandler.IsAuthorized;
        
        private NeuralNetworkBaseClient _client;
        private readonly Dictionary<ushort, Vector2> _framePositionMap = new Dictionary<ushort, Vector2>(); 
        
        /// <summary>
        /// Инициализация все компонентов устройства 
        /// </summary>
        public void Initialize(IPEndPoint endPoint)
        {
            if (!execute)
                return;
            
            videoHandler.Initialize(cameraType);
            hardwareController.Initialize();
            _client = new NeuralNetworkSocketClient(endPoint);
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
            _client.SendMessage(newMessage);
        }

        private void OnDisable()
        {
            _client?.Dispose();
        }
    }
}