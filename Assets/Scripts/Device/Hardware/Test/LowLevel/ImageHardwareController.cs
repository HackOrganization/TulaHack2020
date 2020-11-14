using Device.Hardware.Test.HighLevel.Image;

namespace Device.Hardware.Test.LowLevel
{
    public class ImageHardwareController: DirectHardwareController
    {
        public override void OnStart()
        {
            wideFieldController = gameObject.AddComponent<ImageTestWideFiledHighLevelController>();
            tightFieldController = gameObject.AddComponent<ImageTestTightFieldHighLevelController>();
            
            Initialize();
        }
    }
}