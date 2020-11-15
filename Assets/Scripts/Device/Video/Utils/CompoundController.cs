using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Extensions;

namespace Device.Video.Utils
{
    [Serializable]
    public class CompoundController
    {
        public readonly DerivativeValue PositionValue = new DerivativeValue();
        public readonly DerivativeValue SizeValue = new DerivativeValue();
    }

    
    /// <summary>
    /// Дифференциальная составляющая регулятора 
    /// </summary>
    public class DerivativeValue
    {
        private const int VALUES_COUNT = 3;

        /// <summary>
        /// Возвращает отрегулированное значение по последним обновленным
        /// </summary>
        public Vector2Int AverageValue
        {
            get
            {
                var value = _values.Average();
                return new Vector2Int(Mathf.FloorToInt(value.x), Mathf.FloorToInt(value.y));
            }
        }
        
        private readonly List<Vector2> _values = new List<Vector2>();

        public DerivativeValue()
        {
            var nullVector = Vector2.zero;
            _values.Capacity = VALUES_COUNT;
            _values.AddRange(Enumerable.Repeat(nullVector, VALUES_COUNT));
        }
            
        public Vector2Int Update(Vector2 value)
        {
            _values.RemoveAt(0);
            _values.Add(value);

            return AverageValue;
        }
    }
}