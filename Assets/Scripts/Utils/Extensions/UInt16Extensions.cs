using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils.Extensions
{
    public static class UInt16Extensions
    {
        /// <summary>
        /// Получает следующий индекс инкрементирование по модулю 
        /// </summary>
        public static ushort GetNext(this ushort value)
        {
            return value == ushort.MaxValue
                ? ushort.MinValue
                : ++value;
        }

        /// <summary>
        /// Получает следующий индекс из коллекции 
        /// </summary>
        public static ushort GetNextFree(this IEnumerable<ushort> collection)
        {
            if (!collection.Any())
                return ushort.MinValue;

            if(collection.Count() == ushort.MaxValue + 1)
                throw new Exception($"Collection is overfill");
            
            var last = collection.Last();
            do
            {
                last = last.GetNext();
            } 
            while (collection.Any(el => el == last));
            
            return last;
        }
    }
}