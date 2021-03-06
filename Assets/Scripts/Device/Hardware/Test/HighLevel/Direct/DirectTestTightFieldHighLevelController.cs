﻿using Device.Hardware.HighLevel;
using Device.Hardware.Test.HighLevel.Direct.Utils;
using Device.Utils;
using UnityEngine;

namespace Device.Hardware.Test.HighLevel.Direct
{
    public class DirectTestTightFieldHighLevelController: TightFieldHighLevelController
    {
        public override void Initialize()
        {
            base.Initialize();
            PositionController = new DirectTestTightFieldPositionController();
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