using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using LowLevelUtils = Device.Hardware.LowLevel.Utils;

namespace UI.InputSpace
{
    public abstract class JoystickController : MonoBehaviour
    {
        private const float CLAMP_MULTIPLIER = 0.3f;
        protected const float VERTICAL_ANGLE = 90f;

        private static readonly string[] EnableStatusValues =
        {
            "Выкл",
            "Вкл"
        };
        
        protected static readonly Vector2 NullPosition = Vector2.zero;
        
        [Header("Event triggers")] 
        [SerializeField] protected EventTrigger joystickTrigger;

        [Header("Transforms")]
        [SerializeField] private RectTransform thresholdRectTransform;
        [SerializeField] protected RectTransform joystickRectTransform;

        [Header("Enabler")] 
        [SerializeField] protected Toggle enableToggle;
        [SerializeField] private Text enableStatusText;
        [SerializeField] private Image[] joystickImages;
        [SerializeField] private Color[] joystickColors;
        
        [Header("Settings")] 
        [SerializeField] private int stepMultiplier = 50;
        
        protected abstract Vector2 CurrentPosition { get; set; }
        protected Vector2Int SendStep => new Vector2Int(
            (int) (CurrentPosition.x / Magnitude * stepMultiplier), 
            (int) (CurrentPosition.y / Magnitude * stepMultiplier));

        protected bool IsDragged;
        protected float Magnitude;
        protected float SqrMagnitude;
        protected WaitForSeconds LoopWait;
        
        protected virtual void Start()
        {
            LoopWait = new WaitForSeconds(1f / LowLevelUtils.SerialPortParams.TIMEOUT);
            enableToggle.onValueChanged.AddListener(OnToggleChange);
            
            SetUpThreshold();
            SetUpTriggers();
        }

        private void SetUpThreshold()
        {
            var thresholdHalfSize = thresholdRectTransform.rect.size / 2;
            SqrMagnitude = thresholdHalfSize.sqrMagnitude * CLAMP_MULTIPLIER;
            Magnitude = Mathf.Sqrt(SqrMagnitude);
        }
        
        private void SetUpTriggers()
        {
            var entry = new EventTrigger.Entry {eventID = EventTriggerType.BeginDrag};
            entry.callback.AddListener(OnBeginDrag);
            joystickTrigger.triggers.Add(entry);
            
            entry = new EventTrigger.Entry {eventID = EventTriggerType.Drag};
            entry.callback.AddListener(OnDrag);
            joystickTrigger.triggers.Add(entry);

            entry = new EventTrigger.Entry {eventID = EventTriggerType.EndDrag};
            entry.callback.AddListener(OnEndDrag);
            joystickTrigger.triggers.Add(entry);
        }

        /// <summary>
        /// В начале управления джойстиком 
        /// </summary>
        private void OnBeginDrag(BaseEventData eventData)
        {
            if(!enableToggle.isOn)
                return;

            IsDragged = true;
            StartCoroutine(ESendPosition());
        }
        
        /// <summary>
        /// При перемещении джойстика 
        /// </summary>
        private void OnDrag(BaseEventData eventData)
        {
            if(!enableToggle.isOn)
                return;
            
            var pointerEventData = (PointerEventData) eventData;
            CurrentPosition = pointerEventData.position - pointerEventData.pressPosition;
        }
        
        /// <summary>
        /// По отпусканию джойстика 
        /// </summary>
        private void OnEndDrag(BaseEventData eventData)
        {
            CurrentPosition = NullPosition;
            IsDragged = false;
            StopCoroutine(ESendPosition());
        }

        /// <summary>
        /// При изменении флага активации джойстика 
        /// </summary>
        protected virtual void OnToggleChange(bool value)
        {
            var index = value ? 1 : 0;
            foreach (var image in joystickImages)
                image.color = joystickColors[index];

            enableStatusText.text = EnableStatusValues[index];
        }

        /// <summary>
        /// Передача текущей позиции нужному контроллеру 
        /// </summary>
        protected abstract IEnumerator ESendPosition();
    }
}

