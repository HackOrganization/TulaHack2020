namespace Core.MathConversion.Utils
{
    public static class Params
    {
        public const int CYCLE_ANGLES = 360;
    }

    public static class WideFieldParams
    {
        public const int CYCLE_STEPS = 6000;

        public const int WIDTH = 640;
        public const int HEIGHT = 480;

        //ToDo: BEFORE TEST SetUp correct
        public const int HORISONTAL_ANGLES = 40 / 2;
        public const int VERTICAL_ANGLES = 30 / 2;

        public static readonly float AngleToPixels = (float) WIDTH / (2 * HORISONTAL_ANGLES);
        public static readonly float AngleToSteps = (float) CYCLE_STEPS / Params.CYCLE_ANGLES;
    }

    public static class TightFieldParams
    {
        
    }
}