using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Core;
using Networking.Client;
using Networking.Utils;
using UnityAsyncHelper.Core;
using UnityEngine;
using EventType = Core.EventType;

namespace Networking.Server
{
    public class AsynchronousServer: IDisposable
    {
        public static readonly Dictionary<AsynchronousServer, List<AsynchronousClient>> ListenerClientMap = new Dictionary<AsynchronousServer, List<AsynchronousClient>>();

        private readonly ManualResetEvent _allDone = new ManualResetEvent(false);
        private readonly Socket _socket;

        /// <summary>
        /// Конструктор класса сервера
        /// </summary>
        private AsynchronousServer(IPEndPoint localEndPoint)
        {
            _socket = new Socket(localEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        
        /// <summary>
        /// Начинает прослшку конкретного узла
        /// </summary>
        public static void StartListening(IPEndPoint localEndPoint, out AsynchronousServer listener)
        {
            try
            {
                listener = ListenerClientMap.Keys.FirstOrDefault(l =>
                    ((IPEndPoint) l._socket.LocalEndPoint).Equals(localEndPoint));
                if (listener != null)
                {
                    Debug.Log($"Listener of {localEndPoint} is already opened");
                    return;
                }

                listener = new AsynchronousServer(localEndPoint);
                listener._socket.Bind(localEndPoint);
                listener._socket.Listen(Params.SERVER_CONNECTIONS_COUNT);
                ListenerClientMap.Add(listener, new List<AsynchronousClient>());

                ThreadManager.AsyncExecute(listener.AcceptClients, null);
            }
            catch (Exception e)
            {
                listener = null;
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Принимает новые входящие подключения. Запускаем в новом потоке.
        /// </summary>
        private void AcceptClients()
        {
            try
            {
                while (!IsDisposed)
                {
                    Debug.Log("Waiting for connection...");

                    _allDone.Reset();
                    _socket.BeginAccept(AcceptCallback, this);
                    _allDone.WaitOne();
                }

                Debug.Log($"Server listening point {_socket.LocalEndPoint} is closed");
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Фиксирует новое входящее подключение.
        /// Подключенный источник сохраняется в карту.
        /// Запсукается ожидание чтения.
        /// </summary>
        private static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                var server = (AsynchronousServer) ar.AsyncState;
                server._allDone.Set();
                var client = new AsynchronousClient(server._socket.EndAccept(ar));
                ListenerClientMap[server].Add(client);

                client.Receive();
                ThreadManager.ExecuteOnMainThread(
                    () => EventManager.RaiseEvent(EventType.ClientConnected, 
                        client.IsClientSide, client.Socket.LocalEndPoint, client.Socket.RemoteEndPoint));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        #region DISPOSE

        /// <summary>
        /// Флаг окончания работы.
        /// </summary>
        public bool IsDisposed { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    foreach (var client in ListenerClientMap[this])
                    {
                        client.Dispose();
                    }
                    ListenerClientMap[this].Clear();
                    ListenerClientMap.Remove(this);
                }
                IsDisposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}