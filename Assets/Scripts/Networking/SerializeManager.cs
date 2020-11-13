using System;
using System.Collections.Generic;
using Networking.Message;
using Networking.Message.Utils;

namespace Networking
{
    /// <summary>
    /// Менеджер сериализации/десериализации сообщение сообщений.
    /// Пробрасывает десериализацию на статичный метод нужного класса, ориентируясь на тип сообщения
    /// </summary>
    public static class SerializeManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static IMessage Deserialise(MessageType messageType, in byte[] data)
        {
            switch (messageType)
            {
                case MessageType.WideFieldPosition:
                    return WideFieldPositionMessage.Deserialize(data);
            }

            return null;
        }
        
        /// <summary>
        /// Возвращает занчение типа UInt16 из массива байтов начиная с offset
        /// </summary>
        public static ushort GetBytes(in Array source, ref int offset)
        {
            var result = new byte[2];
            Array.Copy(source, offset, result, 0, 2);
            offset += 2;
            return BitConverter.ToUInt16(result, 0);
        }
        
        /// <summary>
        /// Добавляет в массив байтов значение Value типа byte и возвращает новую позицию головки записи
        /// </summary>
        public static int SetByte(in byte[] destination, byte value, int offset = 0)
        {
            destination[offset] = value;
            return offset + 1;
        }

        /// <summary>
        /// Добавляет в массив байтов массив байтов Source и возвращает новую позицию головки записи
        /// </summary>
        public static int SetBytes(in byte[] destination, Array source, int offset = 0)
        {
            var length = source.Length;
            Array.Copy(source, 0, destination, offset, length);
            return offset + length;
        }
        
        /// <summary>
        /// Добавляет в массив байтов значение Value типа UInt16 и возвращает новую позицию головки записи
        /// </summary>
        public static int SetBytes(in byte[] destination, ushort value, int offset = 0, int length = 0)
        {
            if(length == 0)
                length = sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(value), 0, destination, offset, length);
            return offset + length;
        }
    }
}





















