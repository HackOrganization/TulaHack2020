using Device.Hardware.HighLevel.Utils;
using UnityEngine;

namespace Device.Hardware.Test.HighLevel.Direct.Utils
{
    public class DirectTestWideFieldPositionController: WideFieldPositionController
    {
        public override void SetUp(Vector2Int newValue)
        {
            if(TowardsPosition == newValue.x)
                return;

            TowardsPosition = newValue.x;
            _updated = true;
        }
    }
}