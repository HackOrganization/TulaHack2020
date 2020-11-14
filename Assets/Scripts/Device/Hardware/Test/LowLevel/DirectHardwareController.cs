using System;
using System.Collections;
using System.Linq;
using Core.OrderStart;
using Device.Hardware.LowLevel;
using Device.Hardware.LowLevel.Utils.Communication;
using Device.Hardware.LowLevel.Utils.Communication.Infos;
using Device.Hardware.Test.HighLevel.Direct;
using Device.Hardware.Test.UI.Read;
using Device.Hardware.Test.UI.Write;
using UnityEngine;
using LowLevelUtils = Device.Hardware.LowLevel.Utils;

namespace Device.Hardware.Test.LowLevel
{
    public class DirectHardwareController : HardwareController, IStarter
    {
        [Header("TrackMode is enabled?")] 
        [SerializeField] private bool isTracking; 
        public virtual void OnStart()
        {
            wideFieldController = gameObject.AddComponent<DirectTestWideFieldHighLevelController>();
            tightFieldController = gameObject.AddComponent<DirectTestTightFieldHighLevelController>();
            
            Initialize();
        }

        #region UI CONTROL

        [Header("Panels")] 
        [SerializeField] private SetupPanel setupPanel;
        [SerializeField] private PositionPanel positionPanel;

        #endregion

        #region TIMER

        private bool _timerStarted;
        
        private DateTime _startTime;
        private DateTime[] _finishTimes;

        private int[] _awaitableSteps;

        private void StartTimer(in MoveInfo[] array)
        {
            _timerStarted = true;
            
            _startTime = DateTime.Now;
            _finishTimes = new DateTime[LowLevelUtils.Params.DEVICES_COUNT];
            _awaitableSteps = array.Select(mi => mi.Position).ToArray();
        }

        private void CheckResponse(in Vector2Int[] newPositions)
        {
            if(!_timerStarted)
                return;
            
            var checkSteps = new []
            {
                newPositions[0].x,
                newPositions[1].x,
                newPositions[1].y
            };

            for (var i = 0; i < LowLevelUtils.Params.DEVICES_COUNT; i++)
            {
                if (_finishTimes[i] != DateTime.MinValue)
                    continue;
                
                if(checkSteps[i] == _awaitableSteps[i])
                    _finishTimes[i] = DateTime.Now;
            }

            if (_finishTimes.All(el => el != DateTime.MinValue))
            {
                Debug.Log(string.Join(" | ", _finishTimes.Select(ft => (ft - _startTime).TotalMilliseconds)));
                _timerStarted = false;
            }
        }

        #endregion
        
        #region OVERRIDES
        protected override IEnumerator EWork()
        {
            yield return _untilPortOpened;

            _serialPortController.Send(CommunicationParams.GetCalibrationMessage());
            yield return new WaitUntil(() => _calibrateDone);

            var setUpMessage = CommunicationParams.GetSetupMessage(new SetupInfo(), new SetupInfo(), new SetupInfo(1000));
            _serialPortController.Send(setUpMessage);
            yield return _loopWait;
            
            positionPanel.SetInfo(new [] { Vector2Int.zero, Vector2Int.zero });
                        
            while (!_isDisabled)
            {
                var moveInfoExist = false;
                var moveInfosArray = new MoveInfo[LowLevelUtils.Params.DEVICES_COUNT];
                var index = 0;
                foreach (var controller in CameraBaseControllers)
                {
                    var enumMoveInfos = controller.PositionController.TowardsInfos(ref moveInfoExist);
                    foreach (var mi in enumMoveInfos)
                        moveInfosArray[index++] = mi;
                }

                if (moveInfoExist)
                {
                    StartTimer(moveInfosArray);
                    
                    //Если какая-то из координат была обновлена, тогда отправляем команду наведения
                    var moveMessage = CommunicationParams.GetMoveMessage(moveInfosArray);
                    _serialPortController.Send(moveMessage);
                    _trackModeController.Reset();
                }
                else if (setupPanel.IsUpdated)
                {
                    _serialPortController.Send(setupPanel.SetUpMessage);
                }
                else if (!isTracking || !_trackModeController.SetUp())
                {
                    //если режим слежения не нужно запускать, то запрашивает текущие координаты для уточнения
                    _positionRequested = true;
                    _serialPortController.Send(CommunicationParams.GetPositionRequestMessage());    
                }

                foreach (var controller in CameraBaseControllers)
                    controller.updateCurrentPosition = moveInfoExist;

                yield return _loopWait;

                if (_positionRequested)
                    yield return new WaitUntil(() => !_positionRequested);
            }
        }

        protected override void SetUpNewPositions(in Vector2Int[] newPositions)
        {
            base.SetUpNewPositions(in newPositions);
            positionPanel.SetInfo(in newPositions);
            CheckResponse(in newPositions);
        }

        #endregion
    }
}