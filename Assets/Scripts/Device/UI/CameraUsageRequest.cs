using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.OrderStart;
using UnityEngine;
using EventType = Core.EventType;

namespace Device.UI
{
    /// <summary>
    /// Обрабатывает разрешение на использование камеры (используем на всяйик пожарный)
    /// </summary>
    public class CameraUsageRequest : MonoBehaviour, IStarter
    {
        public void OnStart()
        {
            StartCoroutine(Request());
        }
        
        /// <summary>
        /// Запрашиваем разрешение на использование устройств.
        /// Разрешено или нет - вызываем события EventType.DeviceAuthorized или EventType.DeviceLocked соответсвенно для дальнейшей обработки
        /// </summary>
        private IEnumerator Request()
        {
            yield return DeviceRequest(FindWebCams, UserAuthorization.WebCam);
            //yield return DeviceRequest(FindMicrophones, UserAuthorization.Microphone);//Микрофоны не нужны, но вот на всякий пожарный
            
            EventManager.RaiseEvent(EventType.CameraAuthorized);
        }

        /// <summary>
        /// Отправляет запрос на использование устройств
        /// <param name="enumerateAvailableAction">Функция перечисления доступных устройств</param>
        /// <param name="deviceType">Тип устройств</param>> 
        /// </summary>
        private static IEnumerator DeviceRequest(Func<int> enumerateAvailableAction, UserAuthorization deviceType)
        {
            var deviceCount = enumerateAvailableAction.Invoke();
            if (deviceCount == 0)
            {
                EventManager.RaiseEvent(EventType.CameraLocked, deviceType);
                yield return null;
            }
            else
            {
                yield return Application.RequestUserAuthorization(deviceType);    
                Debug.Log(
                    Application.HasUserAuthorization(deviceType)
                        ? $"{deviceType} found"
                        : $"{deviceType} not found");
            }
        }

        /// <summary>
        /// Выводит имена всех найденных камер и возвращает их количество
        /// </summary>
        private static int FindWebCams()
        {
            var devices = WebCamTexture.devices;
            if(devices.Any())
                EnumerateDevices(devices.Select(d => d.name));

            return devices.Length;
        }

        /// <summary>
        /// Выводит имена всех найденных микрофонов и возвращает их количество
        /// </summary>
        private static int FindMicrophones()
        {
            var devices = Microphone.devices;
            EnumerateDevices(devices);

            return devices.Length;
        }

        /// <summary>
        /// Выводит в консоль имена всех доступных устройств 
        /// </summary>
        private static void EnumerateDevices(IEnumerable<string> devices)
        {
            foreach (var device in devices)
                Debug.Log($"Name: {device}");
        }
    }
}