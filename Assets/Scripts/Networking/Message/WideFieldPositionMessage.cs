using Networking.Message.Utils;
using UnityEngine;

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
        /// Сериализует Сообщение в массив байтов 
        /// </summary>
        public byte[] Serialize()
        {
            var offset = 0;
            var result = new byte[1 + 2 + (2 + 2)];
            offset = SerializeManager.SetByte(in result, (byte) MessageType, offset);
            offset = SerializeManager.SetBytes(in result, PacketId, offset);
            offset = SerializeManager.SetBytes(in result, (ushort)Position.x, offset);
            SerializeManager.SetBytes(in result, (ushort)Position.y, offset);

            return result;
        }

        /// <summary>
        /// Десериализирует данные из массива байтов в класс 
        /// </summary>
        public static IMessage Deserialize(in byte[] data)
        {
            var message = new WideFieldPositionMessage();
            var offset = 1;
            message.PacketId = SerializeManager.GetUInt16(data, ref offset);
            var x = SerializeManager.GetUInt16(data, ref offset);
            var y = SerializeManager.GetUInt16(data, ref offset);
            message.Position = new Vector2Int(x, y);

            return message;
        }
    }
}