using Device.Hardware.HighLevel;
using Device.Hardware.Test.HighLevel.Direct.Utils;
using Device.Utils;
using UnityEngine;

namespace Device.Hardware.Test.HighLevel.Direct
{
    public class DirectTestWideFieldHighLevelController: WideFieldHighLevelController
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
            
            if((SourceCommandType) args[1] != SourceCommandType.Manual)
                return;
            
            var position = (Vector2Int) args[2];
            PositionController.SetUp(position);
        }
    }
}