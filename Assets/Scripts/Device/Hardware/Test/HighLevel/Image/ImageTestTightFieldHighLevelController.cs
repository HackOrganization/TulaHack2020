using Core.MathConversion;
using Device.Hardware.HighLevel;
using Device.Hardware.Test.HighLevel.Direct.Utils;
using Device.Utils;
using UnityEngine;

namespace Device.Hardware.Test.HighLevel.Image
{
    public class ImageTestTightFieldHighLevelController: TightFieldHighLevelController
    {
        public override void Initialize()
        {
            base.Initialize();
            PositionController = new DirectTestTightFieldPositionController();
        }

        /// <summary>
        /// Обновление координаты наведения
        /// </summary>
        protected override void OnNewPositionCaptured(object[] args)
        {
            var cameraType = (CameraTypes) args[0];
            if(CameraType != cameraType)
                return;

            if((SourceCommandType) args[1] != SourceCommandType.Auto)
                return;
            
            var objectImagePosition = (Vector2Int) args[2];
            var wideFieldAzimuthStep = (int) args[3];
            
            var tightAzimuthStep = wideFieldAzimuthStep.AzimuthTightFieldCameraStep();
            var elevationStep = objectImagePosition.ElevationTightFieldCameraStep();

            var newPosition = new Vector2Int(tightAzimuthStep, elevationStep);
            newPosition.Clamp(MinStepValue, MaxStepValue);
            PositionController.SetUp(newPosition);
        }
    }
}