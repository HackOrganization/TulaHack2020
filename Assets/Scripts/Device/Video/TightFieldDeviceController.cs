using Device.Utils;

namespace Device.Video
{
    public class TightFieldDeviceController: DeviceController
    {
        /// <summary>
        /// Тип камеры
        /// </summary>
        protected override CameraTypes CameraType => CameraTypes.TightField;
    }
}