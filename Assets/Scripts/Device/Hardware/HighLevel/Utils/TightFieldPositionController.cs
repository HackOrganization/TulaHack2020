using System.Collections.Generic;
using Device.Hardware.LowLevel.Utils.Communication.Infos;
using UnityEngine;
using Utils.Extensions;

using LowLevelTightFieldParams = Device.Hardware.LowLevel.Utils.TightFieldParams;

namespace Device.Hardware.HighLevel.Utils
{
    /// <summary>
    /// Сущность работы с последней переданной позицией наведения узкопольной камеры
    /// </summary>
    public class TightFieldPositionController : IPositionController
    {
        protected bool _updated;
        
        /// <summary>
        /// Позиция в шагах
        /// </summary>
        public Vector2Int TowardsPosition { get; protected set; }
        
        /// <summary>
        /// Устанавливает новое значение, куда нужно ориентироваться 
        /// </summary>
        public virtual void SetUp(Vector2Int newValue)
        {
            if(!TowardsPosition.TightFieldNeedUpdate(in newValue))
                return;
            
            TowardsPosition = new Vector2Int(
                newValue.x < 0 ? TowardsPosition.x : newValue.x,
                newValue.y < 0 ? TowardsPosition.y : newValue.y);
            
            _updated = true;
        }

        /// <summary>
        /// Возврает новую текущую позицию согласно скорости вращения шаговика, следуя к Position
        /// Если позиция достигнута, то continueInvoke меняется в false 
        /// </summary>
        public Vector2Int UpdateCurrentPosition(Vector2Int currentValue, ref bool continueInvoke)
        {
            var deltaTime = Time.deltaTime;
            var ratio = new Vector2(
                deltaTime / LowLevelTightFieldParams.FULL_CYCLE_MIN_TIME_X,
                deltaTime / LowLevelTightFieldParams.FULL_CYCLE_MIN_TIME_Y
                );
            ratio.Scale(new Vector2(LowLevelTightFieldParams.CYCLE_STEPS_X, LowLevelTightFieldParams.CYCLE_STEPS_Y));
            
            var newValue = new Vector2Int(
                    (int) Mathf.MoveTowards(currentValue.x, TowardsPosition.x, ratio.x),    
                    (int) Mathf.MoveTowards(currentValue.y, TowardsPosition.y, ratio.y)    
                );
            continueInvoke = newValue != TowardsPosition;
            
            return newValue;
        }

        /// <summary>
        /// Преобразует позицию в набор MoveInfo 
        /// </summary>
        public IEnumerable<MoveInfo> TowardsInfos(ref bool success)
        {
            success |= _updated;
            _updated = false;

            return new[] { new MoveInfo(TowardsPosition.x), new MoveInfo(TowardsPosition.y) };
        }
    }
}