using Device.Networking;
using Device.Utils;
using Device.Video.Utils;
using Networking.Message;
using Networking.Message.Utils;
using NetworkingParams = Networking.Utils.Params;

namespace Device.Video
{
    public class WideFieldDeviceController: DeviceController
    {
        /// <summary>
        /// Тип камеры
        /// </summary>
        protected override CameraTypes CameraType => CameraTypes.WideField;

        public override void Initialize()
        {
            base.Initialize();
            Client = new NeuralNetworkSocketClient(NetworkingParams.WideFieldEndPoint);
        }
        
        /// <summary>
        /// Перехватывает команду отправки изображения
        /// </summary>
        public void OnSendImageRequest()
        {
            if(videoHandler.Status != VideoStatuses.Play)
                if(!videoHandler.Play())
                    return;
            
            var newMessage = new ImageMessage(
                CameraType == CameraTypes.WideField
                    ? MessageType.WideFieldImage
                    : MessageType.TightFieldImage)
            {
                Image = videoHandler.SendFrame
            };

            Client.SendMessage(newMessage);
        }
    }
}