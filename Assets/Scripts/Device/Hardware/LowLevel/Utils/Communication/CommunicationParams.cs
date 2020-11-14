using System.Collections.Generic;
using System.Linq;
using Device.Hardware.LowLevel.Utils.Communication.Infos;
using UnityEngine;

using LowLevelParams = Device.Hardware.LowLevel.Utils.Params;

namespace Device.Hardware.LowLevel.Utils.Communication
{
    /// <summary>
    /// Параметры общения по UART
    /// </summary>
    public static class CommunicationParams
    {
        /// <summary>
        /// Запрос идентификации устройства
        /// </summary>
        public const string HELLO_REQUEST = "T;";
        
        /// <summary>
        /// Ответ на запрос идентификации устройства
        /// </summary>
        public const string HELLO_RESPONSE = "PSINA";

        /// <summary>
        /// Ответ на запрос калибровки
        /// </summary>
        public const string CALIBRATION_RESPONSE = "Calibration done";
        
        /// <summary>
        /// Знак разделитель
        /// </summary>
        public const char SEPARATOR = ',';
        
        /// <summary>
        /// Флаг окончания изображения
        /// </summary>
        private const char END_LINE_FLAG = ';';

        /// <summary>
        /// Флаг установки параметров
        /// </summary>
        private const char SETUP_FLAG = 'S';
        
        /// <summary>
        /// Флаг запроса на перемещения в указанную позицию
        /// </summary>
        private const char MOVE_FLAG = 'M';
        
        /// <summary>
        /// Флаг Запрос/Получения ответа о текущей позиции
        /// </summary>
        public const char POSITION_FLAG = 'P';
        
        /// <summary>
        /// Флаг начала запроса на калибровку 
        /// </summary>
        private const char CALIBRATION_FLAG = 'C';
        
        /// <summary>
        /// Флаг начала запросов управления состоянием лазера
        /// </summary>
        private const char LASER_FLAG = 'L';
        
        /// <summary>
        /// Возвращает команду установки параметров с базовыми настройками 
        /// </summary>
        public static string GetDefaultSetupMessage() => GetSetupMessage();

        /// <summary>
        /// Возвращает команду по установке начальных параметров
        /// </summary>
        public static string GetSetupMessage(params SetupInfo[] setupInfos)
        {
            var collectionInfos = new List<SetupInfo>(setupInfos);
            while(collectionInfos.Count < LowLevelParams.DEVICES_COUNT)
                collectionInfos.Add(new SetupInfo());

            var command = $"{SETUP_FLAG}";
            command += CommandConcat(collectionInfos.Take(LowLevelParams.DEVICES_COUNT).ToArray());
            return command;
        }
        
        /// <summary>
        /// Возвращает строку установки состояния лазера 
        /// </summary>
        public static string GetLaserMessage(bool enable)
        {
            var enableChar = enable ? '1' : '0';
            return $"{LASER_FLAG}{enableChar}{END_LINE_FLAG}";
        }

        /// <summary>
        /// Возвращает команду по установке позиции (в шагах)
        /// </summary>
        public static string GetMoveMessage(params MoveInfo[] moveInfos)
        {
            var collectionInfos = new List<MoveInfo>(moveInfos);
            while(collectionInfos.Count < LowLevelParams.DEVICES_COUNT)
                collectionInfos.Add(new MoveInfo());

            var command = $"{MOVE_FLAG}";
            command += CommandConcat(collectionInfos.Take(LowLevelParams.DEVICES_COUNT).ToArray());
            return command;
        }

        /// <summary>
        /// Возвращает строку запроса текущего положения
        /// </summary>
        public static string GetPositionRequestMessage() => $"{POSITION_FLAG}{END_LINE_FLAG}";

        /// <summary>
        /// Возвращает переведенный в Vector2Int массив текущих позиций камер 
        /// </summary>
        public static Vector2Int[] ParsePositionResponse(string message)
        {
            message = message.Substring(1, message.Length - 2);
            var values = message.Split(SEPARATOR).Select(int.Parse).ToArray();
            return new []
            {
                new Vector2Int(values[0], 0),
                new Vector2Int(values[1], values[2])
            };
        } 
        
        /// <summary>
        /// Возвращает команду по калибровке
        /// </summary>
        public static string GetCalibrationMessage() => $"{CALIBRATION_FLAG}{END_LINE_FLAG}";

        /// <summary>
        /// Объединяет набор информаций по комманде в строку.
        /// Необходима предварительная обработка набора (подразумевается, что количество команд соответсвует параметру CommunicationParams.DEVICES_COUNT)
        /// </summary>
        private static string CommandConcat(IEnumerable<ICommandInfo> commandInfos)
        {
            var result = string.Join($"{SEPARATOR}", commandInfos.Select(c => c.ToString()));
            return $"{result}{END_LINE_FLAG}";
        }
    }
}