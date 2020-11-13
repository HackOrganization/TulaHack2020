using System.ComponentModel;

namespace Camera.Utils
{
    /// <summary>
    /// Типы камер
    /// </summary>
    public enum CameraTypes
    {
        [Description("Широкопольная")]
        WideField,
        
        [Description("Узкопольная")]
        TightField
    }
}