using System.Linq;
using Device.Hardware.LowLevel.Utils;
using Device.Hardware.LowLevel.Utils.Communication;
using Device.Hardware.LowLevel.Utils.Communication.Infos;

namespace Device.Hardware.Test.UI.Write
{
    public class SetupPanel: WritePanel
    {
        public bool IsUpdated
        {
            get
            {
                var value = _isUpdated;
                if (value)
                    _isUpdated = false;

                return value;
            }
            private set => _isUpdated = value;
        }

        private bool _isUpdated;

        public string SetUpMessage
        {
            get
            {
                var setupInfoArray = new []
                {
                    new SetupInfo(WideFieldBuffer.x),
                    new SetupInfo(TightFieldBuffer.x),
                    new SetupInfo(TightFieldBuffer.y)
                };
            
                return CommunicationParams.GetSetupMessage(setupInfoArray.Where(info => info.Speed >= 0).ToArray());
            }    
        }
            
        protected override void Execute()
        {
            ClampValues(wideFieldInput, 0, Params.DEFAULT_SPEED);
            ClampValues(tightFieldInputX, 0, Params.DEFAULT_SPEED);
            ClampValues(tightFieldInputY, 0, Params.DEFAULT_SPEED);
            
            base.Execute();
            IsUpdated = true;
        }
    }
}