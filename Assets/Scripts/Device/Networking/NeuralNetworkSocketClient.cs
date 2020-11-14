using System;
using System.Net;
using Core;
using Device.Utils;
using Networking.Client;
using Networking.Message;
using Networking.Message.Utils;
using UnityEngine;
using Utils.Extensions;
using EventType = Core.EventType;

namespace Device.Networking
{
    /// <summary>
    /// Класс общения с нейронной сетью, написанный на сокетах
    /// </summary>
    [Serializable]
    public class NeuralNetworkSocketClient: NeuralNetworkBaseClient
    {
        public NeuralNetworkSocketClient(IPEndPoint endPoint) : base(endPoint) { }

        /// <summary>
        /// Перехватывает событие подключение клиента к серверу
        /// </summary>
        /// <param name="args">[0] - isLocalClient, [1] - localEndPoint, [2] - remoteEndPoint</param>
        protected override void OnConnected(object[] args)
        {
            var isLocal = (bool) args[0];
            if(!isLocal)
                return;
            
            var localPoint = (IPEndPoint) args[1];
            var remotePoint = (IPEndPoint) args[2];
            
            if(!Client.IsThis(localPoint, remotePoint))
                return;
            
            Debug.Log($"[Client] Connected: {localPoint} -> {remotePoint}");
            Client.Receive();
            Connected = true;
        }

        /// <summary>
        /// Перехватывает событие получения сообщения
        /// <param name="args">[0] - MessageType, [1] - IMessage,[2] - AsynchronousClient</param>
        /// </summary>
        protected override void OnReceived(object[] args)
        {
            var messageType = (MessageType) args[0]; 
            if(!messageType.GetMessageDestination().IsClientSupported())
                return;
            
            var client = (AsynchronousClient) args[2];
            if(!Client.IsThis(client))
                return;

            switch (messageType)
            {
                case MessageType.CloseConnection:
                    var sendGoodbye = ((CloseConnectionMessage) args[1]).SendGoodbyeMessage;
                    if(sendGoodbye)
                        client.Dispose();
                    else
                        client.SafeDispose();
                    
                    //ToDo: close application
                    break;
                
                case MessageType.WideFieldPosition:
                    OnWideFieldPositionCaught((WideFieldPositionMessage) args[1]);
                    break;
                
                case MessageType.TightFieldPosition:
                    Debug.Log("TightFieldPosition caught");
                    break;
                
                default: return;
            }            
        }
        
        #region SERVER RESPONSES

        private void OnWideFieldPositionCaught(WideFieldPositionMessage message)
        {
            EventManager.RaiseEvent(EventType.DeviceGoPosition, CameraTypes.WideField, message.Position);
            Debug.Log($"[Client] WideField[{message.PacketId}]: take position \"{message.Position}\"");
        }
        
        #endregion
    }
}