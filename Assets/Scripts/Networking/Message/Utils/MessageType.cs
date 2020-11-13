namespace Networking.Message.Utils
{
    /// <summary>
    /// Тип сообщения. Используется для идентификации действия с сообщением. Отводится 1 байт
    /// </summary>
    public enum MessageType
    {
        WideFieldImage,
        
        WideFieldPosition,
        TightFieldPosition
    }
}