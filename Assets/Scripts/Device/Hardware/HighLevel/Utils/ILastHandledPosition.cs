using System.Collections.Generic;
using Device.Hardware.LowLevel.Utils.Communication.Infos;
using UnityEngine;

namespace Device.Hardware.HighLevel.Utils
{
    /// <summary>
    /// Сущность работы с последней переданной позицией наведения
    /// </summary>
    public interface ILastHandledPosition
    {
        /// <summary>
        /// Устанавливает новое значение, куда нужно ориентироваться 
        /// </summary>
        void SetUp(Vector2Int newValue);
        
        /// <summary>
        /// Преобразует позицию в набор MoveInfo 
        /// </summary>
        IEnumerable<MoveInfo> ToMoveInfos(ref bool success);
    }
}