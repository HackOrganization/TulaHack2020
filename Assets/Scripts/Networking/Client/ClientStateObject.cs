using System;
using System.Collections.Generic;
using System.Linq;
using Networking.Utils;
using Utils.Extensions;

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

        private int _messageLength = -1;

        public bool MessageReceived
        {
            get
            {
                if (ReceivedBytes.Count < MessageExtensions.HEADER_LENGTH)
                    return false;

                if (_messageLength == -1)
                    _messageLength = BitConverter.ToInt32(
                        ReceivedBytes.Take(MessageExtensions.HEADER_LENGTH).ToArray(), 
                        0);
                
                return  _messageLength == ReceivedBytes.Count;
            }
        }
        
        public ClientStateObject(AsynchronousClient client)
        {
            Client = client;
            Buffer = new byte[client.IsClientSide ? Params.CLIENT_BUFFER_SIZE : Params.SERVER_BUFFER_SIZE];
        }
    }  
}