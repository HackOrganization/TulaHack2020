namespace Device.Hardware.LowLevel.Utils
{
    public class SerialPortDetectorEventArgs
    {
        public readonly bool Result;
        public readonly string PortName;

        public SerialPortDetectorEventArgs(bool result, string portName)
        {
            Result = result;
            PortName = portName;
        }
    }
}