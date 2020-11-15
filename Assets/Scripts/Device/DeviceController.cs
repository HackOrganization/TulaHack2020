using Device.Networking;
using Device.Utils;
using Device.Video;
using UnityEngine;

namespace Device
{
    /// <summary>
    /// Контроллер управления устройством 
    /// </summary>
    public abstract class DeviceController : MonoBehaviour
    {
        [Header("Camera info")] 
        [SerializeField] protected VideoHandler videoHandler;
        
        /// <summary>
        /// Тип камеры
        /// </summary>
        protected abstract CameraTypes CameraType { get; }
        
        /// <summary>
        /// Фиксирует, что устройство готово к работе.
        /// </summary>
        public abstract bool IsReady { get; } 
        
        protected NeuralNetworkBaseClient Client;
         
        /// <summary>
        /// Инициализация все компонентов устройства 
        /// </summary>
        public virtual void Initialize()
        {
            videoHandler.Initialize(CameraType);
        }

        public void Dispose()
        {
            Client?.Dispose();
            videoHandler.Dispose();
        }
        
        private void OnDisable()
        {
            Dispose();
        }
    }
}