﻿using System.Collections.Generic;
using Device.Hardware.LowLevel.Utils.Communication.Infos;
using UnityEngine;

namespace Device.Hardware.HighLevel.Utils
{
    /// <summary>
    /// Сущность работы с последней переданной позицией наведения широкоугольной камеры
    /// </summary>
    public class WideFieldLastHandledPosition: ILastHandledPosition
    {
        private bool _updated;
        
        /// <summary>
        /// Позиция в шагах
        /// </summary>
        public int Position { get; private set; }
        
        /// <summary>
        /// Устанавливает новое значение, куда нужно ориентироваться 
        /// </summary>
        public void SetUp(Vector2Int newValue)
        {
            Position = newValue.x;
            _updated = true;
        }

        /// <summary>
        /// Преобразует позицию в набор MoveInfo 
        /// </summary>
        public IEnumerable<MoveInfo> ToMoveInfos(ref bool success)
        {
            success |= _updated;
            _updated = false;

            if (success)
            {
                Debug.Log("Should Send!");
            }
            
            return new[] { new MoveInfo(Position) };
        }
    }
}