using Core.GameEvents;
using Device.Utils;
using LowLevelWideFieldParams = Device.Hardware.LowLevel.Utils.WideFieldParams;
using LowLevelTightFieldParams = Device.Hardware.LowLevel.Utils.TightFieldParams;

namespace Device.Hardware.Test.UI.Write
{
    public class DirectMovePanel: WritePanel
    {
        protected override void Execute()
        {
            ClampValues(wideFieldInput, LowLevelWideFieldParams.WIDEFIELD_MIN_STEPS, LowLevelWideFieldParams.WIDEFIELD_MAX_STEPS);
            ClampValues(tightFieldInputX, LowLevelTightFieldParams.TIGHTFIELD_MIN_STEPS_X, LowLevelTightFieldParams.TIGHTFIELD_MAX_STEPS_X);
            ClampValues(tightFieldInputY, LowLevelTightFieldParams.TIGHTFIELD_MIN_STEPS_Y, LowLevelTightFieldParams.TIGHTFIELD_MAX_STEPS_Y);
            
            base.Execute();
            EventManager.RaiseEvent(EventType.DeviceGoPosition, CameraTypes.WideField, SourceCommandType.Manual, WideFieldBuffer);
            EventManager.RaiseEvent(EventType.DeviceGoPosition, CameraTypes.TightField, SourceCommandType.Manual, TightFieldBuffer);
        }
    }
}