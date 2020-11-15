using Device.Utils;

namespace Device.Video
{
    public class TightFieldDeviceController: DeviceController
    {
        /// <summary>
        /// Тип камеры
        /// </summary>
        protected override CameraTypes CameraType => CameraTypes.TightField;

        /// <summary>
        /// Фиксирует, что устройство готово к работе.
        /// Для этого необходиом, чтобы камера подключилась
        /// </summary>
        public override bool IsReady => videoHandler.IsAuthorized;
    }
}