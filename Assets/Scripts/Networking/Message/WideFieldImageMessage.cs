using Networking.Message.Utils;
using UnityEngine;

namespace Networking.Message
{
    public class WideFieldImageMessage: IMessage
    {
        /// <summary>
        /// Тип сообщения. Требует 1 байт
        /// </summary>
        public MessageType MessageType => MessageType.WideFieldImage;

        /// <summary>
        /// Идентификатор кадра, который мы отправили в MotionDetection. Требует 2 байта
        /// </summary>
        public ushort PacketId;

        /// <summary>
        /// Изображение.
        /// Передается размер (Требует на каждую координату по 2 байта)
        /// и сами байты изображения
        /// </summary>
        public Texture2D Image;
        
        /// <summary>
        /// Сериализует класс в массив байтов 
        /// </summary>
        public byte[] Serialize()
        {
            var imageBytes = Image.EncodeToJPG();
            var data = new byte[1 + 2 + (2 + 2 + imageBytes.Length)];
            
            var offset = SerializeManager.SetByte(in data, (byte) MessageType);
            offset = SerializeManager.SetBytes(in data, PacketId, offset);
            offset = SerializeManager.SetBytes(in data, (ushort) Image.width, offset);
            offset = SerializeManager.SetBytes(in data, (ushort) Image.height, offset);
            SerializeManager.SetBytes(in data, imageBytes, offset);
            
            return data;
        }

        /// <summary>
        /// Десериализирует данные из массива байтов в класс 
        /// </summary>
        public static IMessage Deserialize(in byte[] data)
        {
            var message = new WideFieldImageMessage();
            var offset = 1;
            message.PacketId = SerializeManager.GetUInt16(data, ref offset);
            var width = SerializeManager.GetUInt16(data, ref offset);
            var height = SerializeManager.GetUInt16(data, ref offset);

            message.Image = new Texture2D(width, height, TextureFormat.RGBA32,false);
            message.Image.LoadImage(SerializeManager.GetBytes(data, ref offset));
            
            return null;    
        }
    }
}