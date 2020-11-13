using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Extensions
{
    public static class WebCamTextureExtensions
    {
        /// <summary>
        /// Возвращает данные камеры по ее идентификационному имени 
        /// </summary>
        public static WebCamDevice GetByIdentificationName(this IEnumerable<WebCamDevice> devices, string identificationName)
        {
            foreach (var device in devices)
                if (device.name.StartsWith(identificationName, StringComparison.OrdinalIgnoreCase))
                    return device;
            
            throw new Exception($"There is no connected device with IdentificationName \"{identificationName}\"");
        }
    }
}