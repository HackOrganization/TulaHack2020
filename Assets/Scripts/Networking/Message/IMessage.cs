using Networking.Message.Utils;

namespace Networking.Message
{
    public interface IMessage
    {
        /// <summary>
        /// Тип сообщения
        /// </summary>
        MessageType MessageType { get; }

        /// <summary>
        /// Функция сериализации сообщения в массив байтов 
        /// </summary>
        byte[] Serialize();
    }
}