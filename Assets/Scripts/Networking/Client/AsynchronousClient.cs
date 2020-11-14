using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Core;
using Networking.Message;
using Networking.Message.Utils;
using UnityAsyncHelper.Core;
using UnityEngine;
using Utils.Extensions;
using EventType = Core.EventType;

namespace Networking.Client
{
    public class AsynchronousClient: IDisposable
    {
        private readonly ManualResetEvent _connectDone = new ManualResetEvent(false);
        private readonly ManualResetEvent _sendDone = new ManualResetEvent(false);
        private readonly ManualResetEvent _receiveDone = new ManualResetEvent(false);

        public readonly bool IsClientSide;
        public readonly Socket Socket;

        /// <summary>
        /// Конструктор класса Асинхронного клиента на строне Клиента
        /// </summary>
        private AsynchronousClient(IPEndPoint remoteEndPoint)
        {
            Socket = new Socket(remoteEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IsClientSide = true;
        }

        /// <summary>
        /// Констурктор класса асинхронного клиента на стороне Сервера 
        /// </summary>
        public AsynchronousClient(Socket socket)
        {
            Socket = socket;
            IsClientSide = false;
        }
        
        /// <summary>
        /// Подключается к серверу
        /// </summary>
        public static AsynchronousClient Connect(IPEndPoint remoteEndPoint)
        {
            try
            {
                var newClient = new AsynchronousClient(remoteEndPoint);
                newClient.Socket.BeginConnect(remoteEndPoint, ConnectCallback, newClient);
                newClient._connectDone.WaitOne();

                return newClient;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            return null;
        }

        /// <summary>
        /// Фиксирует успешное подключение 
        /// </summary>
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                var client = (AsynchronousClient) ar.AsyncState;
                client.Socket.EndConnect(ar);

                ThreadManager.ExecuteOnMainThread(
                    () => EventManager.RaiseEvent(EventType.ClientConnected, 
                        client.IsClientSide, client.Socket.LocalEndPoint, client.Socket.RemoteEndPoint));
                client._connectDone.Set();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Принимает сообщение
        /// </summary>
        public void Receive()
        {
            try
            {
                void AsyncReceive()
                {
                    while (!IsDisposed)
                    {
                        _receiveDone.Reset();
                        var state = new ClientStateObject(this);
                        Socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, ReceiveCallback, state);
                        _receiveDone.WaitOne();
                    }
                }
                
                ThreadManager.AsyncExecute(AsyncReceive, null);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Фиксирует принятый пакет.
        /// Дожидается оставшихся пакетов, если такие имееются,
        /// либо же десериализирует данные и передает их в обработку всем подсписчикам события "ClientReceivedMessage" 
        /// </summary>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var state = (ClientStateObject) ar.AsyncState;
                var socket = state.Client.Socket;
                var bytesRead = socket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.ReceivedBytes.AddRange(state.Buffer.Take(bytesRead));
                    if (!state.MessageReceived)
                    {
                        socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, ReceiveCallback, state);
                        return;
                    }
                }
                
                if (state.MessageReceived)
                {
                    var messageType = (MessageType) state.ReceivedBytes[MessageExtensions.HEADER_LENGTH];
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        var message = SerializeManager.Deserialise(messageType, state.ReceivedBytes.ToArray());
                        EventManager.RaiseEvent(EventType.ReceivedMessage, messageType, message, state.Client);
                    });
                }
                state.Client._receiveDone.Set();
                
            }
            catch (Exception e)
            {
                Debug.Log(e);   
            }
        }

        /// <summary>
        /// Отправляет массив байтов 
        /// </summary>
        public void Send(byte[] data, bool runAsync = true)
        {
            try
            {
                void AsyncSend()
                {
                    Socket.BeginSend(data, 0, data.Length, 0, SendCallback, this);
                    _sendDone.WaitOne();    
                }
                
                if(runAsync)
                    ThreadManager.AsyncExecute(AsyncSend, null);
                else
                    AsyncSend();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Фиксирует окончание отправки
        /// </summary>
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                var client = (AsynchronousClient) ar.AsyncState;
                var bytesSend = client.Socket.EndSend(ar);
                Debug.Log($"Sent bytes: {bytesSend}");

                client._sendDone.Set();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Закрывает сокет 
        /// </summary>
        private void Close(bool sayGoodbye = true)
        {
            void AsyncClose()
            {
                Send(new CloseConnectionMessage(IsClientSide).Serialize(), false);                  
            }

            void AsyncCloseCallback()
            {
                try
                {
                    Socket.Shutdown(SocketShutdown.Both);
                }
                finally
                {
                    Socket.Dispose();
                    Socket.Close();
                }  
            }
            
            if(sayGoodbye)
                ThreadManager.AsyncExecute(AsyncClose, AsyncCloseCallback);
            else
                ThreadManager.AsyncExecute(AsyncCloseCallback, null);
        }

        /// <summary>
        /// Операция сравнения клиента 
        /// </summary>
        public bool IsThis(object obj)
        {
            if(obj is AsynchronousClient client)
                return IsThis(client.Socket.LocalEndPoint, client.Socket.RemoteEndPoint);

            return false;
        }

        /// <summary>
        /// Операция сравнения клиента 
        /// </summary>
        public bool IsThis(EndPoint localPoint, EndPoint remotePoint)
        {
            return Socket.LocalEndPoint.Equals(localPoint) && Socket.RemoteEndPoint.Equals(remotePoint);
        }

        #region DISPOSE

        /// <summary>
        /// Флаг окончания работы.
        /// </summary>
        public bool IsDisposed { get; private set; }

        protected virtual void Dispose(bool disposing, bool sayGoodbye = true)
        {
            if (!IsDisposed)
            {
                if (disposing)
                    Close(sayGoodbye);
                IsDisposed = true;
            }
        }
        
        /// <summary>
        /// Методы вызова очистки данных класса с оповещением сервера 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Методы вызова очистки данных класса без оповещения сервера 
        /// </summary>
        public void SafeDispose()
        {
            Debug.Log($"Closing socket: {Socket.LocalEndPoint} -> {Socket.RemoteEndPoint}");
            
            Dispose(true, false);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}