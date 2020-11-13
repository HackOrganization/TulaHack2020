using System;
using Networking.Message.Utils;

namespace Networking.Message
{
    /// <summary>
    /// Сообщение о закрытии клиента
    /// </summary>
    public class CloseConnectionMessage: IMessage
    {
        /// <summary>
        /// Тип сообщения
        /// </summary>
        public MessageType MessageType => MessageType.CloseConnection;

        /// <summary>
        /// Расположен ли сокет на стороне клиента
        /// </summary>
        public readonly bool IsClientSide;

        public CloseConnectionMessage(bool isClientSide)
        {
            IsClientSide = isClientSide;
        }
        
        /// <summary>
        /// Сериализует сообщение в массив байтов 
        /// </summary>
        public byte[] Serialize()
        {
            var data = new byte[1 + sizeof(bool)];
            var offset = SerializeManager.SetByte(in data, (byte) MessageType);
            SerializeManager.SetBytes(in data, IsClientSide, offset);
            return data;
        }

        /// <summary>
        /// Обрабатывает вызов десериализации сообщения 
        /// </summary>
        public static IMessage Deserialize(in byte[] data)
        {
            var offset = 1; 
            return new CloseConnectionMessage(SerializeManager.GetBoolean(data, ref offset));
        }
    }
}