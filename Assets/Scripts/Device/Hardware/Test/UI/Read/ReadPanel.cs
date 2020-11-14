using UnityEngine;
using UnityEngine.UI;

namespace Device.Hardware.Test.UI.Read
{
    public abstract class ReadPanel: MonoBehaviour
    {
        [Header("Controls")]
        [SerializeField] protected InputField wideFieldInput;
        [SerializeField] protected InputField tightFieldInputX;
        [SerializeField] protected InputField tightFieldInputY;

        public abstract void SetInfo(in Vector2Int[] newPositions);
    }
}