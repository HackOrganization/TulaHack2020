
using Networking.Message.Utils;
using UnityEngine;
using Utils.Extensions;

namespace Networking.Message
{
    /// <summary>
    /// Сообщение с координатами на изображении широкопольной камеры
    /// </summary>
    public class WideFieldPositionMessage: IMessage
    {
        /// <summary>
        /// Тип сообщения. Требует 1 байт
        /// </summary>
        public MessageType MessageType => MessageType.WideFieldPosition;

        /// <summary>
        /// Идентификатор кадра, который мы отправили в MotionDetection. Требует 2 байта
        /// </summary>
        public ushort PacketId;

        /// <summary>
        /// Пиксельные координаты изображения. Требует на каждую координату по 2 байта
        /// </summary>
        public Vector2Int Position;

        /// <summary>
        /// Размеры изображения
        /// </summary>
        public Vector2Int Size;
        
        /// <summary>
        /// Сериализует Сообщение в массив байтов 
        /// </summary>
        public byte[] Serialize()
        {
            //MessageType + PacketId + (Position.x + Position.y) + (Size.x + Size.y)
            const ushort length = (ushort) (1 + 2 + (2 + 2) + (2 + 2));
            this.CreateMessage(length, out var data);

            var offset = MessageExtensions.HEADER_LENGTH;
            offset = MessageExtensions.SetByte(in data, (byte) MessageType, offset);
            offset = MessageExtensions.SetBytes(in data, PacketId, offset);
            offset = MessageExtensions.SetBytes(in data, (ushort)Position.x, offset);
            offset = MessageExtensions.SetBytes(in data, (ushort)Size.x, offset);
            offset = MessageExtensions.SetBytes(in data, (ushort)Size.y, offset);
            MessageExtensions.SetBytes(in data, (ushort)Position.y, offset);

            return data;
        }

        /// <summary>
        /// Десериализирует данные из массива байтов в класс 
        /// </summary>
        public static IMessage Deserialize(in byte[] data)
        {
            var offset = MessageExtensions.HEADER_LENGTH + 1;
            var message = new WideFieldPositionMessage
            {
                PacketId = MessageExtensions.GetUInt16(data, ref offset)
            };

            var positionX = MessageExtensions.GetInt16(data, ref offset);
            var positionY = MessageExtensions.GetInt16(data, ref offset);
            message.Position = new Vector2Int(positionX, positionY);

            var sizeX = MessageExtensions.GetUInt16(data, ref offset);
            var sizeY = MessageExtensions.GetUInt16(data, ref offset);
            message.Size = new Vector2Int(sizeX, sizeY);
            
            return message;
        }
    }
}