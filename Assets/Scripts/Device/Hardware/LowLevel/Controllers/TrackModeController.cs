using System.Collections.Generic;
using Device.Hardware.HighLevel;
using Device.Utils;
using UnityEngine;
using Utils.Extensions;

using GlobalWideFieldParams = Device.Utils.WideFieldParams;
using LowLevelWideFieldParams = Device.Hardware.LowLevel.Utils.WideFieldParams;
using Params = Device.Hardware.LowLevel.Utils.Params;

namespace Device.Hardware.LowLevel.Controllers
{
    /// <summary>
    /// контроллер режима слежения
    /// </summary>
    public class TrackModeController
    {
        private static readonly Dictionary<int, int> BoundSteps = new Dictionary<int, int>
        {
            { 1, LowLevelWideFieldParams.WIDEFIELD_MIN_STEPS },
            { -1, LowLevelWideFieldParams.WIDEFIELD_MAX_STEPS }
        };

        private CameraBaseController WideFiledHighLevelController =>
            HardwareController.Instance.WideFieldHighLevelController;

        private int _direction = -1;
        private bool _isEnabled;
        private int _requestsToSetupTrackMode;
        
        /// <summary>
        /// Активирует режим слежения, если это необходимо, и возвращает результат активации 
        /// </summary>
        public bool SetUp()
        {
            if (GlobalWideFieldParams.SourceCommandType != SourceCommandType.Auto)
                return false;
            
            var needChangeDirection = NeedChangeDirection();
            if (_isEnabled && !needChangeDirection)
                return false;

            if (!_isEnabled)
            {
                _requestsToSetupTrackMode = Mathf.Clamp(--_requestsToSetupTrackMode, int.MinValue + 1,
                    Params.EMPTY_REQUESTS_TO_SETUP_TRACKING_MODE);
                if (_requestsToSetupTrackMode > 0)
                    return false;    
            }
            
            var newPoint = new Vector2Int(BoundSteps[_direction], 0);
            WideFiledHighLevelController.PositionController.SetUp(newPoint);

            Reset(true);
            Debug.Log($"Set TrackMode (direction: {_direction} | point : {newPoint})");
            return true;
        }

        /// <summary>
        /// Нужно ли менять направление движения в режиме слежки
        /// </summary>
        private bool NeedChangeDirection()
        {
            var currentWideFiledPosition = WideFiledHighLevelController.CurrentPosition;
            if (BoundSteps[_direction].WideFieldNeedUpdate(in currentWideFiledPosition)) 
                return false;
            
            _direction *= -1;
            return true;
        }
        
        /// <summary>
        /// Сбрасывает режим слежения
        /// </summary>
        public void Reset(bool isEnabled = false)
        {
            _requestsToSetupTrackMode = Params.EMPTY_REQUESTS_TO_SETUP_TRACKING_MODE;
            _isEnabled = isEnabled;
        }
    }
}