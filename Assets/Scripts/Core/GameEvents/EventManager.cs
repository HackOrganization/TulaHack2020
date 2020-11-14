using System;
using System.Collections.Generic;
using UnityAsyncHelper.Core;

namespace Core.GameEvents 
{
    /// <summary>
    /// Типы событий
    /// </summary>
    public enum EventType 
    {
        CameraAuthorized,
        CameraLocked,
        
        ClientConnected, //IsClientSide, LocalEndPoint, RemoteEndPoint
        ReceivedMessage, //MessageType, IMessage, AsynchronousClient
        
        DeviceHandlePosition, //CameraTypes
        DeviceGoPosition, //CameraTypes, Position[Vector2Int]
        
        CameraDrawObject,//CameraTypes, Position[Vector2Int], Size[Vector2Int], 
        CaptureNewImage, //CameraTypes
        
        EndWork
    }

    /// <summary>
    /// Реализация менеджера игровых эвентов 
    /// </summary>
    public static class EventManager 
    {
        private static readonly Dictionary<EventType, List<Action<object[]>>> _handlers = 
            new Dictionary<EventType, List<Action<object[]>>>();

        /// <summary>
        /// Добавляет слушателя событий переданного типа
        /// </summary>
        public static void AddHandler(EventType type, Action<object[]> handler)
        {
            if (_handlers.ContainsKey(type)) 
            {
                _handlers[type].Add(handler);
            }
            else {
                var handlers = new List<Action<object[]>> { handler };
                _handlers.Add(type, handlers);
            }
        }

        /// <summary>
        /// Удаляет слушателя событий переданного типа
        /// </summary>
        public static void RemoveHandler(EventType type, Action<object[]> handler) 
        {
            if (!_handlers.ContainsKey(type)) 
                return;
            
            _handlers[type].Remove(handler);
        }

        /// <summary>
        /// Отправляет эвент всем слушателям
        /// </summary>
        public static void RaiseEvent(EventType type, params object[] args)
        {
            if (!_handlers.ContainsKey(type))
                return;
            
            foreach (var handler in _handlers[type]) 
            {
                handler(args);
            }
        }

        /// <summary>
        /// Вызывает событие в основном потоке 
        /// </summary>
        public static void RaiseOnMainThread(EventType type, params object[] args)
        {
            ThreadManager.ExecuteOnMainThread(()=> RaiseEvent(type, args));
        }
    }
}