using System.Collections.Generic;
using System.Linq;
using Device.Hardware.LowLevel.Utils.Communication.Infos;
using UnityEngine;

namespace Device.Hardware.LowLevel.Utils.Communication
{
    /// <summary>
    /// Параметры общения по UART
    /// </summary>
    public static class CommunicationParams
    {
        /// <summary>
        /// Количество управляемых устройств
        /// </summary>
        public const int DEVICES_COUNT = 3;
        
        /// <summary>
        /// Время поворота полного круга (в секундах)
        /// </summary>
        public const int FULL_LOOP_TIME = 4;

        /// <summary>
        /// Количество шагов за полный круг (для широкопольной камеры) 
        /// </summary>
        public const int WIDEFIELD_FULL_LOOP_STEPS = 5730;
        public const int TIGHTFIELD_FULL_LOOP_STEPS_X = 5730;
        public const int TIGHTFIELD_FULL_LOOP_STEPS_Y = 5730;
        
        public const string HELLO_REQUEST = "T;";
        public const string HELLO_RESPONSE = "PSINA";

        public const string CALIBRATION_RESPONSE = "Calibration done";
        
        public const char SEPARATOR = ',';
        private const char END_LINE_FLAG = ';';

        private const char SETUP_FLAG = 'S';
        private const char MOVE_FLAG = 'M';
        private const char POSITION_FLAG = 'P';
        private const char CALIBRATION_FLAG = 'C';
        private const char LASER_FLAG = 'L';
        

        public const int DEFAULT_SPEED = 3500;
        public const int DEFAULT_ACCELARATION = 0;

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
            while(collectionInfos.Count < DEVICES_COUNT)
                collectionInfos.Add(new SetupInfo());

            var command = $"{SETUP_FLAG}";
            command += CommandConcat(collectionInfos.Take(DEVICES_COUNT).ToArray());
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
            while(collectionInfos.Count < DEVICES_COUNT)
                collectionInfos.Add(new MoveInfo());

            var command = $"{MOVE_FLAG}";
            command += CommandConcat(collectionInfos.Take(DEVICES_COUNT).ToArray());
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