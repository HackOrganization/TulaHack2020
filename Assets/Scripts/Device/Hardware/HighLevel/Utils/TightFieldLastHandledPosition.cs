using System.Collections.Generic;
using Device.Hardware.LowLevel.Utils.Communication.Infos;
using UnityEngine;
using Utils.Extensions;

namespace Device.Hardware.HighLevel.Utils
{
    /// <summary>
    /// Сущность работы с последней переданной позицией наведения узкопольной камеры
    /// </summary>
    public class TightFieldLastHandledPosition : ILastHandledPosition
    {
        private bool _updated;
        
        /// <summary>
        /// Позиция в шагах
        /// </summary>
        public Vector2Int Position { get; private set; }
        
        /// <summary>
        /// Устанавливает новое значение, куда нужно ориентироваться 
        /// </summary>
        public void SetUp(Vector2Int newValue)
        {
            if(!Position.IsTightFieldUpdatable(in newValue))
                return;
            
            Position = new Vector2Int(
                newValue.x == 0 ? Position.x : newValue.x,
                newValue.y == 0 ? Position.y : newValue.y);
            
            _updated = true;
        }

        /// <summary>
        /// Преобразует позицию в набор MoveInfo 
        /// </summary>
        public IEnumerable<MoveInfo> ToMoveInfos(ref bool success)
        {
            success |= _updated;
            _updated = false;

            return new[] { new MoveInfo(Position.x), new MoveInfo(Position.y) };
        }
    }
}