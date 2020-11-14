using Device.Hardware.LowLevel.Utils;

namespace Device.Hardware.LowLevel
{
    //ToDo: BEFORE TEST не реализован режим пассивного слежения
    /// <summary>
    /// контроллер режима слежения
    /// </summary>
    public class TrackModeController
    {
        private int _requestsToSetupTrackMode;
        
        /// <summary>
        /// Активирует режим слежения, если это необходимо, и вохвращает результат активации 
        /// </summary>
        public bool SetUp()
        {
            if (--_requestsToSetupTrackMode > 0)
                return false;


            return true;
        }
        
        /// <summary>
        /// Сбрасывает режим слежения
        /// </summary>
        public void Reset()
        {
            _requestsToSetupTrackMode = Params.EMPTY_REQUESTS_TO_SETUP_TRACKIN_MODE; 
        }
    }
}