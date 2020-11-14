namespace Device.Hardware.LowLevel.Utils
{
    public class SerialPortDetectorEventArgs
    {
        public readonly bool Result;
        
        public readonly SerialPortController Controller;

        public SerialPortDetectorEventArgs(bool result, SerialPortController controller)
        {
            Result = result;
            Controller = controller;
        }
    }
}