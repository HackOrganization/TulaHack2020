using Device.Hardware.Test.UI.Write;
using UnityEngine;

namespace Device.Hardware.Test.UI.Read
{
    public class PositionPanel: ReadPanel
    {
        public override void SetInfo(in Vector2Int[] newPositions)
        {
            wideFieldInput.SetValue(newPositions[0].x);
            tightFieldInputX.SetValue(newPositions[1].x);
            tightFieldInputY.SetValue(newPositions[1].y);
        }
    }
}