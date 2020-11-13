﻿using System.IO;
using System.Net;
using Core;
using Core.OrderStart;
using Networking.Client;
using Networking.Message;
using Networking.Message.Utils;
using Networking.Utils;
using UnityEngine;
using Utils;
using Utils.Extensions;
using EventType = Core.EventType;

namespace Networking.Server
{
    /// <summary>
    /// Эмулятор питонвского сервера
    /// </summary>
    public class UnityServerManager : Singleton<UnityServerManager>, IStarter
    {
        private AsynchronousServer _wideFieldServer;

        public void OnStart()
        {
            EventManager.AddHandler(EventType.ClientConnected, OnClientConnected);
            EventManager.AddHandler(EventType.ReceivedMessage, OnMessageReceived);
            
            AsynchronousServer.StartListening(Params.WideFieldEndPoint, out _wideFieldServer);
        }

        /// <summary>
        /// Сообщение о подключение клиента
        /// </summary>
        /// <param name="args">IsClientSide, LocalEndPoint, RemoteEndPoint</param>
        private static void OnClientConnected(params object[] args)
        {
            if((bool) args[0])
                return;
            
            Debug.Log($"[Server] Connected: {(IPEndPoint) args[1]} -> {(IPEndPoint) args[2]}");
        }

        /// <summary>
        /// Перехватывает событие получения сообщения
        /// <param name="args">MessageType, IMessage, AsynchronousClient</param>
        /// </summary>
        private static void OnMessageReceived(object[] args)
        {
            var messageType = (MessageType) args[0]; 
            if(!messageType.GetMessageDestination().IsServerSupported())
                return;

            var client = (AsynchronousClient) args[2];
            
            switch (messageType)
            {
                case MessageType.CloseConnection:
                    client.SafeDispose();
                    break;
                
                case MessageType.WideFieldImage:
                    ResponseOnWideFieldImage(client, (ImageMessage)args[1]);
                    break;
                case MessageType.TightFieldPosition:
                    ResponseOnTightFieldImage(client, (ImageMessage)args[1]);
                    break;
                default: return;
            }
        }

        public void OnDisable()
        {
            EventManager.RemoveHandler(EventType.ClientConnected, OnClientConnected);
            EventManager.RemoveHandler(EventType.ReceivedMessage, OnMessageReceived);
            
            _wideFieldServer.Dispose();
        }

        #region SERVER RESPONSES

        /// <summary>
        /// Отправляет ответ на полченное сообщение с картинкой с широкоугольной камерой 
        /// </summary>
        private static void ResponseOnWideFieldImage(AsynchronousClient client, ImageMessage message)
        {
            File.WriteAllBytes(@"C:\tmp\serverImage.jpg",message.Image.EncodeToJPG());

            var newMessage = new WideFieldPositionMessage
            {
                PacketId = message.PacketId,
                Position = new Vector2Int(Random.Range(0, 255), Random.Range(0, 255))
            };
            client.Send(newMessage.Serialize());
            Debug.Log($"[Server] WideField[{newMessage.PacketId}]: send position \"{newMessage.Position}\"");
        }

        private static void ResponseOnTightFieldImage(AsynchronousClient client, ImageMessage message)
        {
            Debug.Log("TightFieldImage caught");
        }

        #endregion
    }
}