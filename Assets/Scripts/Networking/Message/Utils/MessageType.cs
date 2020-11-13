using Attributes;
using Networking.Utils;

namespace Networking.Message.Utils
{
    /// <summary>
    /// Тип сообщения. Используется для идентификации действия с сообщением. Отводится 1 байт
    /// </summary>
    public enum MessageType
    {
        [MessageDestination(MessageDestinationTypes.Both)]
        CloseConnection,
        
        [MessageDestination(MessageDestinationTypes.Server)]
        WideFieldImage,
        
        [MessageDestination(MessageDestinationTypes.Server)]
        TightFieldImage,
        
        [MessageDestination(MessageDestinationTypes.Client)]
        WideFieldPosition,
        [MessageDestination(MessageDestinationTypes.Client)]
        TightFieldPosition
    }
}