namespace Device.Utils
{
    public static class Params
    {
        public const int WEB_CAM_FPS = 30;
        
        public const byte WIDEFIELD_DETECTION_PROBABILITY = 90;
    }

    public static class WideFieldParams
    {
        public static SourceCommandType SourceCommandType = SourceCommandType.Auto;
    }
    
    public static class TightFieldParams
    {
        public static SourceCommandType SourceCommandType = SourceCommandType.Auto;
    } 
}