using System;

namespace Device.Utils
{
    [Serializable]
    public class CameraInfo
    {
        /// <summary>
        /// Тип камеры
        /// </summary>
        public CameraTypes cameraType;
        
        /// <summary>
        /// Ключевое слово в имени
        /// </summary>
        public string nameIdentity;
    }
}