using System.Collections.Generic;
using System.Linq;
using Device.Hardware.LowLevel.Utils.Communication.Infos;

namespace Device.Hardware.LowLevel.Utils.Communication
{
    /// <summary>
    /// Параметры общения по UART
    /// </summary>
    public static class CommunicationParams
    {
        public const int DEVICES_COUNT = 3;
        
        public const string HELLO_REQUEST = "TARAFIMOV";
        public const string HELLO_RESPONSE = "PSINA";

        public const string SEPARATOR = ",";
        private const char END_LINE_FLAG = ';';

        private const char SETUP_FLAG = 's';
        private const char MOVE_FLAG = 's';
        private const char CALIBRATION_FLAG = 'c';

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
        /// Возвращает команду по калибровке
        /// </summary>
        public static string GetCalibrationMessage() => $"{CALIBRATION_FLAG}{END_LINE_FLAG}";

        /// <summary>
        /// Объединяет набор информаций по комманде в строку.
        /// Необходима предварительная обработка набора (подразумевается, что количество команд соответсвует параметру CommunicationParams.DEVICES_COUNT)
        /// </summary>
        private static string CommandConcat(IEnumerable<ICommandInfo> commandInfos)
        {
            var result = string.Join(SEPARATOR, commandInfos.Select(c => c.ToString()));
            return $"{result}{END_LINE_FLAG}";
        }
    }
}