using System.Collections.Generic;
using System.Net.Sockets;

namespace Networking.Utils
{
    /// <summary>
    /// Сущность обработки полученных данных
    /// </summary>
    public class ReceiveEntity {  
        
        /// <summary>
        /// Сокет клиента
        /// </summary>
        public readonly Socket WorkSocket;
        
        /// <summary>
        /// Буффер данных, в котором записываются отправленные данные
        /// </summary>
        public readonly byte[] Buffer = new byte[Params.RECEIVE_BUFFER_SEZE];  
        
        /// <summary>
        /// Объединенные данные со всех пакетов текущего сообщения
        /// </summary>
        public readonly List<byte> ReceivedBytes = new List<byte>(); 
        public ReceiveEntity(Socket client)
        {
            WorkSocket = client;
        }
    }  
}