using System.ComponentModel;

namespace Device.Utils
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