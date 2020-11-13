using System;
using System.Linq;
using Device.Utils;
using UnityEngine;
using Utils.Extensions;

namespace Device.Data
{
    /// <summary>
    /// Идентификационные данные камеры
    /// </summary>
    [CreateAssetMenu(fileName = "CameraIdentificationSettings", menuName = "Camera/Settings/Identification", order = 0)]
    public class CameraIdentificationSettings : ScriptableObject
    {
        /// <summary>
        /// Наборы идентификационных данных камер
        /// </summary>
        public CameraInfo[] identificationData;

        /// <summary>
        /// Возвращает идентификационно имя камеры определнного типа 
        /// </summary>
        public string GetName(CameraTypes cameraType)
        {
            var info = identificationData.FirstOrDefault(d => d.cameraType == cameraType);
            if(info == default)
                throw new Exception($"Can't find CameraInfo of \"{cameraType.GetDescription()}\" CameraType");

            return info.nameIdentity;
        }
    }
}