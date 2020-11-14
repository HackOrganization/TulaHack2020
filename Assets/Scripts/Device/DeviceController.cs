using System.Net;
using Device.Networking;
using Device.Utils;
using Device.Video;
using Networking.Message;
using Networking.Message.Utils;
using UnityEngine;

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
        
        /// <summary>
        /// Фиксирует, что устройство готово к работе.
        /// Для этого необходиом, чтобы было создано соединение с сервером и камера подключилась
        /// </summary>
        public bool IsReady => _client.Connected && videoHandler.IsAuthorized;
        
        private NeuralNetworkBaseClient _client;
         
        /// <summary>
        /// Инициализация все компонентов устройства 
        /// </summary>
        public void Initialize(IPEndPoint endPoint)
        {
            if (!execute)
                return;
            
            videoHandler.Initialize(cameraType);
            _client = new NeuralNetworkSocketClient(endPoint);
        }
        
        /// <summary>
        /// Перехватывает команду отправки изображения
        /// </summary>
        public void OnSendImageRequest(ushort packetId)
        {
            if(videoHandler.Status != VideoStatuses.Play)
                if(!videoHandler.Play())
                    return;
            
            videoHandler.Capture();
            
            var newMessage = new ImageMessage(
                cameraType == CameraTypes.WideField
                    ? MessageType.WideFieldImage
                    : MessageType.TightFieldImage)
            {
                PacketId = packetId,
                Image = videoHandler.SendFrame
            };

            _client.SendMessage(newMessage);
        }

        private void OnDisable()
        {
            _client?.Dispose();
        }
    }
}