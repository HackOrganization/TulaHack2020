using System.Collections.Generic;
using Device.Hardware.LowLevel.Utils.Communication.Infos;
using UnityEngine;
using Utils.Extensions;

using LowLevelWideFieldParams = Device.Hardware.LowLevel.Utils.WideFieldParams;

namespace Device.Hardware.HighLevel.Utils
{
    /// <summary>
    /// Сущность работы с последней переданной позицией наведения широкоугольной камеры
    /// </summary>
    public class WideFieldPositionController: IPositionController
    {
        protected bool _updated;
        
        /// <summary>
        /// Позиция в шагах
        /// </summary>
        public int TowardsPosition { get; protected set; }
        
        /// <summary>
        /// Устанавливает новое значение, куда нужно ориентироваться 
        /// </summary>
        public virtual void SetUp(Vector2Int newValue)
        {
            if(!TowardsPosition.WideFieldNeedUpdate(in newValue))
                return;
            
            TowardsPosition = newValue.x;
            _updated = true;
        }

        /// <summary>
        /// Возврает новую текущую позицию согласно скорости вращения шаговика, следуя к Position
        /// Если позиция достигнута, то continueInvoke меняется в false 
        /// </summary>
        public Vector2Int UpdateCurrentPosition(Vector2Int currentValue, ref bool continueInvoke)
        {
            //ToDo: can need calculate ratio of CurrentSpeed and MaxSpeed, if CurrentSpeed != MaxSpeed
            var ratio = Time.deltaTime / LowLevelWideFieldParams.FULL_CYCLE_MIN_TIME;
            var deltaStep = LowLevelWideFieldParams.CYCLE_STEPS * ratio;

            var newValue = (int) Mathf.MoveTowards(currentValue.x, TowardsPosition, deltaStep);
            continueInvoke = newValue != TowardsPosition;
            
            return new Vector2Int(newValue, 0);
        }

        /// <summary>
        /// Преобразует позицию в набор MoveInfo 
        /// </summary>
        public IEnumerable<MoveInfo> TowardsInfos(ref bool success)
        {
            success |= _updated;
            _updated = false;
            
            return new[] { new MoveInfo(TowardsPosition) };
        }
    }
}