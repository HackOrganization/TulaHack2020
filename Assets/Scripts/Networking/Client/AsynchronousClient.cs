using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Core;
using Networking.Message;
using Networking.Message.Utils;
using Networking.Utils;
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

        private byte _connectAttempts;

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
                
                var connectionObject = new ClientConnectionObject(newClient, remoteEndPoint);
                newClient.Socket.BeginConnect(remoteEndPoint, ConnectCallback, connectionObject);
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
            var connectionObject = (ClientConnectionObject) ar.AsyncState;
            var client = connectionObject.Client;
            try
            {
                client.Socket.EndConnect(ar);

                ThreadManager.ExecuteOnMainThread(
                    () => EventManager.RaiseEvent(EventType.ClientConnected,
                        client.IsClientSide, client.Socket.LocalEndPoint, client.Socket.RemoteEndPoint));
                client._connectDone.Set();
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10061)
                {
                    if (client.IsDisposed || ++client._connectAttempts == byte.MaxValue)
                    {
                        client._connectDone.Set();
                        return;
                    }
                    
                    Debug.Log($"Next attempt ({client._connectAttempts})....");
                    Thread.Sleep(Params.RETRY_SETTING_CONNECTION_TIMEOUT);
                    client.Socket.BeginConnect(connectionObject.RemoteEndPoint, ConnectCallback, connectionObject);
                    
                    return;
                }
                throw;
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
                    state.ReceivedBytes.AddRange(state.Buffer.Take(bytesRead));
                
                if (state.MessageReceived)
                {
                    var messageType = (MessageType) state.ReceivedBytes[MessageExtensions.HEADER_LENGTH];
                    var message = SerializeManager.Deserialise(messageType, state.ReceivedBytes.ToArray());
                    
                    EventManager.RaiseOnMainThread(EventType.ReceivedMessage, messageType, message, state.Client);
                    
                    state.Client._receiveDone.Set();
                }
                else
                {
                    socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, ReceiveCallback, state);
                    Debug.Log("Receive continued");
                }
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 0x80004005)
                {
                    var state = (ClientStateObject) ar.AsyncState;
                    state.Client.SafeDispose();
                    //ToDo: mb reconnect?
                }
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
            void AsyncSend()
            {
                try
                {
                    Socket.BeginSend(data, 0, data.Length, 0, SendCallback, this);
                    _sendDone.WaitOne();
                }
                catch (ObjectDisposedException)
                {
                    Debug.Log("Client was disposed!");
                    _sendDone.Set();
                }
                catch (SocketException exception)
                {
                    Debug.Log("May connection was closed!");
                    _sendDone.Set();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            
            if(runAsync)
                ThreadManager.AsyncExecute(AsyncSend, null);
            else
                AsyncSend();
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
                EventManager.RaiseEvent(EventType.EndWork, true);
                IsDisposed = true;
                
                if (disposing)
                    Close(sayGoodbye);
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