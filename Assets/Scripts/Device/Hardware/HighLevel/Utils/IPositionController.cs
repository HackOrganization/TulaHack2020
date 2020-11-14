using System.Collections.Generic;
using Device.Hardware.LowLevel.Utils.Communication.Infos;
using UnityEngine;

namespace Device.Hardware.HighLevel.Utils
{
    /// <summary>
    /// Сущность работы с последней переданной позицией наведения
    /// </summary>
    public interface IPositionController
    {
        /// <summary>
        /// Устанавливает новое значение, куда нужно ориентироваться 
        /// </summary>
        void SetUp(Vector2Int newValue);

        /// <summary>
        /// Возврает новую текущую позицию согласно скорости вращения шаговика, следуя к Position
        /// Если позиция достигнута, то continueInvoke меняется в false 
        /// </summary>
        Vector2Int UpdateCurrentPosition(Vector2Int currentValue, ref bool continueInvoke);
        
        /// <summary>
        /// Преобразует позицию в набор MoveInfo 
        /// </summary>
        IEnumerable<MoveInfo> TowardsInfos(ref bool success);
    }
}