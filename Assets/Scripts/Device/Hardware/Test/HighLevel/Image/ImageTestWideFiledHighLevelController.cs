using Core.GameEvents;
using Core.MathConversion;
using Device.Hardware.HighLevel;
using Device.Hardware.Test.HighLevel.Direct.Utils;
using Device.Utils;
using UnityEngine;
using EventType = Core.GameEvents.EventType;
using LowLevelWideFieldParams = Device.Hardware.LowLevel.Utils.WideFieldParams;

namespace Device.Hardware.Test.HighLevel.Image
{
    public class ImageTestWideFiledHighLevelController: WideFieldHighLevelController
    {
        public override void Initialize()
        {
            base.Initialize();
            PositionController = new DirectTestWideFieldPositionController();
        }

        protected override void OnNewPositionCaptured(object[] args)
        {
            var cameraType = (CameraTypes) args[0];
            if(CameraType != cameraType)
                return;
            
            var objectImagePosition = (Vector2Int) args[1];
            var azimuthStep = objectImagePosition.AzimuthWideFieldCameraStep(CashedDevicePosition);
            azimuthStep = Mathf.Clamp(azimuthStep, 
                LowLevelWideFieldParams.WIDEFIELD_MIN_STEPS,
                LowLevelWideFieldParams.WIDEFIELD_MAX_STEPS);
            PositionController.SetUp(new Vector2Int(azimuthStep, 0));
            
            //EventManager.RaiseEvent(EventType.DeviceGoPosition,CameraTypes.TightField, objectImagePosition, stepPosition);
        }
    }
}