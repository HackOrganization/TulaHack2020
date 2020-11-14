using Core.GameEvents;
using Device.Hardware.LowLevel;
using Device.Utils;
using Device.Video.Utils;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.GameEvents.EventType;

namespace Device.Hardware.Test.UI.Write
{
    public class ImageMovePanel: WritePanel
    {
        [SerializeField] private Button catchButton;
        
        private Vector2Int Buffer = Vector2Int.zero;

        protected override void Start()
        {
            base.Start();
            catchButton.onClick.AddListener(CashPosition);
        }

        private void CashPosition()
        {
            HardwareController.Instance.WideFieldHighLevelController.CashPosition();
        }
        
        protected override void Execute()
        {
            ClampValues(tightFieldInputX, 0, WideFieldParams.WIDTH);
            ClampValues(tightFieldInputY, 0, WideFieldParams.HEIGHT);
            
            Buffer.x = tightFieldInputX.GetValue();
            Buffer.y = tightFieldInputY.GetValue();
            
            EventManager.RaiseEvent(EventType.DeviceGoPosition, CameraTypes.WideField, Buffer);
        }
    }
}