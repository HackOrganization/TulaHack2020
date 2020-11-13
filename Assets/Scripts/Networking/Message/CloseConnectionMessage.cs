using System;
using Networking.Message.Utils;
using Utils.Extensions;

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
            const ushort length = (ushort) (1 + sizeof(bool));
            this.CreatePacket(length, out var data);
            
            var offset = MessageExtensions.HEADER_LENGTH;
            offset = MessageExtensions.SetByte(in data, (byte) MessageType, offset);
            MessageExtensions.SetBytes(in data, IsClientSide, offset);
            return data;
        }

        /// <summary>
        /// Обрабатывает вызов десериализации сообщения 
        /// </summary>
        public static IMessage Deserialize(in byte[] data)
        {
            var offset = MessageExtensions.HEADER_LENGTH + 1; 
            return new CloseConnectionMessage(MessageExtensions.GetBoolean(data, ref offset));
        }
    }
}