using System.ComponentModel;

namespace Networking.Utils
{
    /// <summary>
    /// Набор типов, кому предназначен данный пакет сообщения
    /// </summary>
    public enum MessageDestinationTypes
    {
        [Description("Ошибка")]
        Null,
        
        [Description("И Серверу, и клиенту")]
        Both,
        
        [Description("Cерверу")]
        Server,
        
        [Description("Клиенту")]
        Client
    }
}