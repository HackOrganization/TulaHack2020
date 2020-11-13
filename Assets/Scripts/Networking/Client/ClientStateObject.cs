using System.Collections.Generic;
using Networking.Utils;

namespace Networking.Client
{
    /// <summary>
    /// Сущность обработки полученных данных
    /// </summary>
    public class ClientStateObject
    {
        /// <summary>
        /// Сущность клиента
        /// </summary>
        public readonly AsynchronousClient Client;
        
        /// <summary>
        /// Буффер данных, в котором записываются отправленные данные
        /// </summary>
        public readonly byte[] Buffer;
        
        /// <summary>
        /// Объединенные данные со всех пакетов текущего сообщения
        /// </summary>
        public readonly List<byte> ReceivedBytes = new List<byte>(); 
        
        public ClientStateObject(AsynchronousClient client)
        {
            Client = client;
            Buffer = new byte[client.IsClientSide ? Params.CLIENT_BUFFER_SIZE : Params.SERVER_BUFFER_SIZE];
        }
    }  
}