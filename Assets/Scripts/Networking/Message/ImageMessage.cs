using Networking.Message.Utils;
using UnityEngine;
using Utils.Extensions;

namespace Networking.Message
{
    /// <summary>
    /// Сообщение отправки изображения
    /// </summary>
    public class ImageMessage: IMessage
    {
        /// <summary>
        /// Тип сообщения. Требует 1 байт
        /// </summary>
        public MessageType MessageType { get; private set; }

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

        public ImageMessage(MessageType messageType)
        {
            MessageType = messageType;
        }
        
        /// <summary>
        /// Сериализует класс в массив байтов 
        /// </summary>
        public byte[] Serialize()
        {
            var imageBytes = Image.EncodeToJPG();
            //MessageType + PacketId + (Image.Width + Image.Height + Image.JPG.Length)
            var length = (ushort) (1 + 2 + (2 + 2 + imageBytes.Length));
            this.CreateMessage(length, out var data);

            var offset = MessageExtensions.HEADER_LENGTH;
            offset = MessageExtensions.SetByte(in data, (byte) MessageType, offset);
            offset = MessageExtensions.SetBytes(in data, PacketId, offset);
            offset = MessageExtensions.SetBytes(in data, (ushort) Image.width, offset);
            offset = MessageExtensions.SetBytes(in data, (ushort) Image.height, offset);
            MessageExtensions.SetBytes(in data, imageBytes, offset);
            
            return data;
        }

        /// <summary>
        /// Десериализирует данные из массива байтов в класс 
        /// </summary>
        public static IMessage Deserialize(in byte[] data)
        {
            var offset = MessageExtensions.HEADER_LENGTH;
            var message = new ImageMessage((MessageType) data[offset++])
            {
                PacketId = MessageExtensions.GetUInt16(data, ref offset)
            };
            
            var width = MessageExtensions.GetUInt16(data, ref offset);
            var height = MessageExtensions.GetUInt16(data, ref offset);
            message.Image = new Texture2D(width, height, TextureFormat.RGBA32,false);
            message.Image.LoadImage(MessageExtensions.GetBytes(data, ref offset));
            
            return message;    
        }
    }
}