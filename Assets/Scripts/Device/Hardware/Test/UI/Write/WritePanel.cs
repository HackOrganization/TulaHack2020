using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Device.Hardware.Test.UI.Write
{
    public abstract class WritePanel: MonoBehaviour
    {
        [Header("Controls")]
        [SerializeField, CanBeNull] protected InputField wideFieldInput;
        [SerializeField] protected InputField tightFieldInputX;
        [SerializeField] protected InputField tightFieldInputY;
        [Space] 
        [SerializeField] private Button executeButton;
        
        protected Vector2Int WideFieldBuffer = Vector2Int.zero;
        protected Vector2Int TightFieldBuffer = Vector2Int.zero;

        protected virtual void Start()
        {
            if(wideFieldInput != null)
                wideFieldInput.SetValue(WideFieldBuffer.x);
            tightFieldInputX.SetValue(TightFieldBuffer.x);
            tightFieldInputY.SetValue(TightFieldBuffer.y);
            
            executeButton.onClick.AddListener(Execute);
        }

        protected virtual void Execute()
        {
            if(wideFieldInput != null)
                WideFieldBuffer.x = wideFieldInput.GetValue();
            TightFieldBuffer.x = tightFieldInputX.GetValue();
            TightFieldBuffer.y = tightFieldInputY.GetValue();
        }
        
        protected void ClampValues(InputField inputField, int minValue, int maxValue)
        {
            inputField.SetValue(Mathf.Clamp(inputField.GetValue(), minValue, maxValue));
        }
    }

    public static class Extensions
    {
        public static int GetValue(this InputField inputField)
        {
            var text = inputField.text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                inputField.SetValue(0);
                return 0;
            }

            if (!int.TryParse(text, out var value))
            {
                inputField.SetValue(0);
                return 0;
            }
            return value;
        }

        public static void SetValue(this InputField inputField, int value)
        {
            inputField.text = $"{value}";
        }
    }
}