using System.Collections;
using Core.GameEvents;
using Device.Utils;
using UnityEngine;

using EventType = Core.GameEvents.EventType;
using GlobalTightFieldParams = Device.Utils.TightFieldParams;

namespace UI.InputSpace
{
    public class TightFieldJoystickController: JoystickController
    {
        [Header("Settings")] 
        [SerializeField] private float windowAngle;
        
        /// <summary>
        /// Текущая позиция джойстика
        /// </summary>
        protected override Vector2 CurrentPosition
        {
            get => joystickRectTransform.anchoredPosition;
            set
            {
                var newPosition = value.sqrMagnitude > SqrMagnitude
                    ? Vector2.ClampMagnitude(value, Magnitude)
                    : value;

                ClampInWindow(ref newPosition);
                joystickRectTransform.anchoredPosition = newPosition;
            }
        }
        
        private float _halfWindowAngle;

        protected override void Start()
        {
            _halfWindowAngle = windowAngle / 2;
            base.Start();
            
            enableToggle.isOn = GlobalTightFieldParams.SourceCommandType == SourceCommandType.Manual;
        }

        /// <summary>
        /// Попытка ограничения движения джойстика при малых углах отклонения от 3-х часовых положений  
        /// </summary>
        private void ClampInWindow(ref Vector2 newPosition)
        {
            var absVector = new Vector2(Mathf.Abs(newPosition.x), Mathf.Abs(newPosition.y)); 
            
            var currentAbsAngle = Vector2.Angle(Vector2.right, absVector);
            if (currentAbsAngle < _halfWindowAngle)
                newPosition = new Vector2(newPosition.x, 0);
            else if (VERTICAL_ANGLE - _halfWindowAngle < currentAbsAngle)
                newPosition = new Vector2(0, newPosition.y);
        }
        
        /// <summary>
        /// При изменении флага активации джойстика 
        /// </summary>
        protected override void OnToggleChange(bool value)
        {
            base.OnToggleChange(value);
            GlobalTightFieldParams.SourceCommandType = value
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
                EventManager.RaiseEvent(EventType.DeviceGoPosition, CameraTypes.TightField, SourceCommandType.Manual, SendStep);
                Debug.Log($"{SendStep}");
                yield return LoopWait;
            }
        }
    }
}