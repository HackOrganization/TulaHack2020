using System;
using System.Net;
using Core;
using Networking.Client;
using Networking.Message;
using UnityAsyncHelper.Core;
using UnityEngine;
using EventType = Core.EventType;

namespace Device.Networking
{
    /// <summary>
    /// Интерфейс клиента общения с нейронной сетью
    /// </summary>
    [Serializable]
    public abstract class NeuralNetworkBaseClient: IDisposable
    {
        /// <summary>
        /// Асинхронный многопточный клиент
        /// </summary>
        protected AsynchronousClient Client;
        
        /// <summary>
        /// Состояние поддключения
        /// </summary>
        public bool Connected { get; protected set; }

        protected NeuralNetworkBaseClient(IPEndPoint endPoint)
        {
            EventManager.AddHandler(EventType.ClientConnected, OnConnected);
            EventManager.AddHandler(EventType.ReceivedMessage, OnReceived);

            //Client = AsynchronousClient.Connect(endPoint);
            // object[] SetUpConnection()
            // {
            //     return new object[] {AsynchronousClient.Connect(endPoint)};
            // }
            //
            // void SetUpConnectionCallback(object[] args)
            // {
            //     Client = (AsynchronousClient) args[0];
            // }
            
            ThreadManager.AsyncExecute(() => Client = AsynchronousClient.Connect(endPoint), null);
            Debug.Log("Connection requested");
        }

        /// <summary>
        /// При подключении клиента
        /// </summary>
        protected abstract void OnConnected(object[] args);

        /// <summary>
        /// Отправляет сообщение
        /// </summary>
        public virtual void SendMessage(IMessage message)
        {
            Client.Send(message.Serialize());
        }

        /// <summary>
        /// Перехватывает обработку получения сообщения 
        /// </summary>
        protected abstract void OnReceived(object[] args);
        
        #region DISPOSE

        /// <summary>
        /// Флаг окончания работы.
        /// </summary>
        public bool IsDisposed { get; protected set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    Debug.Log("Dispose");
                    
                    EventManager.RemoveHandler(EventType.ClientConnected, OnConnected);
                    EventManager.RemoveHandler(EventType.ReceivedMessage, OnReceived);
                    Client?.Dispose();
                }
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
        #endregion
    }
}