using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Core;
using Networking.Message;
using Networking.Message.Utils;
using Networking.Utils;
using UnityAsyncHelper.Core;
using UnityEngine;
using EventType = Core.EventType;

namespace Networking
{
    public static class AsynchronousClient
    {
        private static readonly ManualResetEvent ConnectDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent SendDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent ReceiveDone = new ManualResetEvent(false);

        private static Socket _client;
        
        /// <summary>
        /// Подключается к серверу
        /// </summary>
        public static void Connect()
        {
            try
            {
                var remoteEndPoint = Params.EndPoint;
                _client = new Socket(remoteEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _client.BeginConnect(remoteEndPoint, ConnectCallback, _client);
                ConnectDone.WaitOne();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Фиксирует успешное подключение 
        /// </summary>
        private static void ConnectCallback(IAsyncResult ar)
        {
            var client = (Socket) ar.AsyncState;
            client.EndConnect(ar);
            
            ThreadManager.ExecuteOnMainThread(
                () => EventManager.RaiseEvent(EventType.ClientConnected, client.RemoteEndPoint));
            ConnectDone.Set();
        }

        /// <summary>
        /// Принимает сообщение
        /// </summary>
        public static void Receive()
        {
            var state = new ReceiveEntity(_client);
            _client.BeginReceive(state.Buffer, 0, Params.RECEIVE_BUFFER_SEZE, 0, ReceiveCallback, state);
            ReceiveDone.WaitOne();
        }

        /// <summary>
        /// Фиксирует принятый пакет.
        /// Дожидается оставшихся пакетов, если такие имееются,
        /// либо же десериализирует данные и передает их в обработку всем подсписчикам события "ClientReceivedMessage" 
        /// </summary>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            var state = (ReceiveEntity) ar.AsyncState;
            var client = state.WorkSocket;
            var bytesRead = client.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.ReceivedBytes.AddRange(state.Buffer.Take(bytesRead));
                client.BeginReceive(state.Buffer, 0, Params.RECEIVE_BUFFER_SEZE, 0, ReceiveCallback, state);
            }
            else
            {
                if (state.ReceivedBytes.Count > 1)
                {
                    var messageType = (MessageType) state.ReceivedBytes[0];
                    var message = SerializeManager.Deserialise(messageType, state.ReceivedBytes.ToArray());
                    ThreadManager.ExecuteOnMainThread(
                        () => EventManager.RaiseEvent(EventType.ClientReceivedMessage, messageType, message));
                }
                ReceiveDone.Set();
            }
        }
    }
}



















