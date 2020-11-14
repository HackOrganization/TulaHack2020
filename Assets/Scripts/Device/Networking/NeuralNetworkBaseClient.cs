using System;
using System.Net;
using Core;
using Core.GameEvents;
using Networking.Client;
using Networking.Message;
using UnityAsyncHelper.Core;
using UnityEngine;
using EventType = Core.GameEvents.EventType;

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
            SetSubscription();
            
            ThreadManager.AsyncExecute(() => Client = AsynchronousClient.Connect(endPoint), null);
        }

        /// <summary>
        /// Отправляет сообщение
        /// </summary>
        public virtual void SendMessage(IMessage message)
        {
            if(IsDisposed)
                return;
            
            Client.Send(message.Serialize());
        }

        #region GAMEEVENTS
        
        /// <summary>
        /// Устанавливает подписки на глоабльные события
        /// </summary>
        private void SetSubscription()
        {
            EventManager.AddHandler(EventType.ClientConnected, OnConnected);
            EventManager.AddHandler(EventType.ReceivedMessage, OnReceived);
        }
        
        /// <summary>
        /// Отписывается от рассылки глоабльных событий
        /// </summary>
        private void ResetSubscription()
        {
            EventManager.RemoveHandler(EventType.ClientConnected, OnConnected);
            EventManager.RemoveHandler(EventType.ReceivedMessage, OnReceived);
        }
        
        /// <summary>
        /// При подключении клиента
        /// </summary>
        protected abstract void OnConnected(object[] args);
        
        /// <summary>
        /// Перехватывает обработку получения сообщения 
        /// </summary>
        protected abstract void OnReceived(object[] args);
        
        #endregion
        
        #region DISPOSE

        /// <summary>
        /// Флаг, что асинхронный клиент или его обертка (текущий класс) были разрушены
        /// </summary>
        public bool IsAnyDisposed => IsDisposed || Client.IsDisposed;
        
        /// <summary>
        /// Флаг окончания работы.
        /// </summary>
        public bool IsDisposed { get; protected set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                //EventManager.RaiseOnMainThread(EventType.EndWork, true);
                IsDisposed = true;
                
                if (disposing)
                {
                    ResetSubscription();
                    Client?.Dispose();
                }
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