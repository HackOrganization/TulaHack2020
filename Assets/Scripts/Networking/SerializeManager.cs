using System;
using Networking.Message;
using Networking.Message.Utils;

namespace Networking
{
    /// <summary>
    /// Менеджер сериализации/десериализации сообщение сообщений.
    /// Пробрасывает десериализацию на статичный метод нужного класса, ориентируясь на тип сообщения
    /// </summary>
    public static class SerializeManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static IMessage Deserialise(MessageType messageType, in byte[] data)
        {
            switch (messageType)
            {
                case MessageType.CloseConnection:
                    return CloseConnectionMessage.Deserialize(in data);
                
                case MessageType.WideFieldImage:
                case MessageType.TightFieldImage:
                    return ImageMessage.Deserialize(in data);
                
                case MessageType.WideFieldPosition:
                    return WideFieldPositionMessage.Deserialize(in data);
            }
            return null;
        }
    }
}





















