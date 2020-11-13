using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Core;
using Networking.Message.Utils;
using Networking.Utils;
using UnityAsyncHelper.Core;
using UnityEngine;
using EventType = Core.EventType;

namespace Networking
{
    public class AsynchronousClient
    {
        private readonly ManualResetEvent _connectDone = new ManualResetEvent(false);
        private readonly ManualResetEvent _sendDone = new ManualResetEvent(false);
        private readonly ManualResetEvent _receiveDone = new ManualResetEvent(false);

        private readonly Socket _socket;

        public AsynchronousClient(IPEndPoint remoteEndPoint)
        {
            _socket = new Socket(remoteEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        
        /// <summary>
        /// Подключается к серверу
        /// </summary>
        public static void Connect(IPEndPoint remoteEndPoint)
        {
            try
            {
                var newClient = new AsynchronousClient(remoteEndPoint);
                newClient._socket.BeginConnect(remoteEndPoint, ConnectCallback, newClient);
                newClient._connectDone.WaitOne();
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
            var client = (AsynchronousClient) ar.AsyncState;
            client._socket.EndConnect(ar);
            
            ThreadManager.ExecuteOnMainThread(
                () => EventManager.RaiseEvent(EventType.ClientConnected, client._socket.RemoteEndPoint));
            client._connectDone.Set();
        }

        /// <summary>
        /// Принимает сообщение
        /// </summary>
        public void Receive()
        {
            var state = new ReceiveEntity(this);
            _socket.BeginReceive(state.Buffer, 0, Params.RECEIVE_BUFFER_SEZE, 0, ReceiveCallback, state);
            _receiveDone.WaitOne();
        }

        /// <summary>
        /// Фиксирует принятый пакет.
        /// Дожидается оставшихся пакетов, если такие имееются,
        /// либо же десериализирует данные и передает их в обработку всем подсписчикам события "ClientReceivedMessage" 
        /// </summary>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            var state = (ReceiveEntity) ar.AsyncState;
            var socket = state.Client._socket;
            var bytesRead = socket.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.ReceivedBytes.AddRange(state.Buffer.Take(bytesRead));
                socket.BeginReceive(state.Buffer, 0, Params.RECEIVE_BUFFER_SEZE, 0, ReceiveCallback, state);
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
                state.Client._receiveDone.Set();
            }
        }
    }
}



















