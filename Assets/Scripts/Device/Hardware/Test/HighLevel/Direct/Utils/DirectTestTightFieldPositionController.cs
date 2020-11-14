using Device.Hardware.HighLevel.Utils;
using UnityEngine;

namespace Device.Hardware.Test.HighLevel.Direct.Utils
{
    public class DirectTestTightFieldPositionController: TightFieldPositionController
    {
        public override void SetUp(Vector2Int newValue)
        {
            if(newValue == TowardsPosition)
                return;

            TowardsPosition = newValue;
            _updated = true;
        }
    }
}