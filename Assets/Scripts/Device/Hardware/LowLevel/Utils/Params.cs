namespace Device.Hardware.LowLevel.Utils
{
    public static class Params
    {
        /// <summary>
        /// Количество управляемых устройств
        /// </summary>
        public const int DEVICES_COUNT = 3;
        
        /// <summary>
        /// Число запросов с простоем до установки режима слежения
        /// </summary>
        public const int EMPTY_REQUESTS_TO_SETUP_TRACKING_MODE = 50;
        
        /// <summary>
        /// Скрость вращения по умолчанию 
        /// </summary>
        public const int DEFAULT_SPEED = 3000;
        
        /// <summary>
        /// Ускорение вращения по умолчанию
        /// </summary>
        public const int DEFAULT_ACCELARATION = 0;
    }

    public static class WideFieldParams
    {
        /// <summary>
        /// Число шагов за полный круг (АБСОЛЮТНОЕ ЗНАЧЕНИЕ)
        /// </summary>
        public const int CYCLE_STEPS = 7920;
        
        /// <summary>
        /// Минимальное время прохождения полного круга (на максимальной скорости) (в секундах)
        /// </summary>
        public const float FULL_CYCLE_MIN_TIME = 29.95f;

        /// <summary>
        /// Минимальная точка шаговика (ШПК)
        /// </summary>
        public const int WIDEFIELD_MIN_STEPS = -5500;
        
        /// <summary>
        /// Максимальная точка шаговика (ШПК)
        /// </summary>
        public const int WIDEFIELD_MAX_STEPS = 0;
    }
    
    public static class TightFieldParams
    {
        /// <summary>
        /// Число шагов за полный круг (по горизонтали) (АБСОЛЮТНОЕ ЗНАЧЕНИЕ)
        /// </summary>
        public const int CYCLE_STEPS_X = 7854;

        /// <summary>
        /// Число шагов за полный круг (по вертикали) (АБСОЛЮТНОЕ ЗНАЧЕНИЕ)
        /// </summary>
        public const int CYCLE_STEPS_Y = 6152;
        
        /// <summary>
        /// Минимальное время прохождения полного круга по горизонтали (на максимальной скорости) (в секундах)
        /// </summary>
        public const float FULL_CYCLE_MIN_TIME_X = 29.99f;
        
        /// <summary>
        /// Минимальное время прохождения полного круга по горизонтали (на максимальной скорости) (в секундах)
        /// </summary>
        public const float FULL_CYCLE_MIN_TIME_Y = 65.18f;

        /// <summary>
        /// Амплитуда шаговика (УПК X)
        /// </summary>
        public const int TIGHTFIELD_MIN_STEPS_X = -4800;
        
        /// <summary>
        /// Максимальная точка шаговика (ШПК)
        /// </summary>
        public const int TIGHTFIELD_MAX_STEPS_X = 0;

        /// <summary>
        /// Амплитуда шаговика (УПК Y)
        /// </summary>
        public const int TIGHTFIELD_MIN_STEPS_Y = -940;
        
        /// <summary>
        /// Максимальная точка шаговика (ШПК)
        /// </summary>
        public const int TIGHTFIELD_MAX_STEPS_Y = 0;
    }
}