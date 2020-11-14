using System.Collections;
using Core.GameEvents;
using Device.Utils;
using UnityEngine;

using EventType = Core.GameEvents.EventType;
using GlobalWideFieldParams = Device.Utils.WideFieldParams;

namespace UI.InputSpace
{
    public class WideFieldJoystickController: JoystickController
    {
        /// <summary>
        /// Текущая позиция джойстика
        /// </summary>
        protected override Vector2 CurrentPosition
        {
            get => joystickRectTransform.anchoredPosition;
            set
            {
                var newPosition = new Vector2(value.x, 0);
                if (value.sqrMagnitude > SqrMagnitude)
                    newPosition.x = Magnitude;

                joystickRectTransform.anchoredPosition = newPosition;
            }
        }

        protected override void Start()
        {
            base.Start();
            
            enableToggle.isOn = GlobalWideFieldParams.SourceCommandType == SourceCommandType.Manual;
        }

        /// <summary>
        /// При изменении флага активации джойстика 
        /// </summary>
        protected override void OnToggleChange(bool value)
        {
            base.OnToggleChange(value);
            GlobalWideFieldParams.SourceCommandType = value
                ? SourceCommandType.Manual
                : SourceCommandType.Auto;
        }

        /// <summary>
        /// Передача текущей позиции нужному контроллеру 
        /// </summary>
        protected override IEnumerator ESendPosition()
        {
            while (enableToggle.isOn && IsDragged)
            {
                //EventManager.RaiseEvent(EventType.DeviceGoPosition, CameraTypes.WideField, SourceCommandType.Manual, SendStep);
                Debug.Log($"{SendStep}");
                yield return LoopWait;
            }
        }
    }
}