using LowLevelParams = Device.Hardware.LowLevel.Utils.Params;

namespace Device.Hardware.LowLevel.Utils.Communication.Infos
{
    /// <summary>
    /// Класс параметров настройки
    /// </summary>
    public class SetupInfo: ICommandInfo
    {
        /// <summary>
        /// Скорость вращения (шагов в минуту)
        /// </summary>
        public readonly int Speed;
        
        /// <summary>
        /// Ускорение 
        /// </summary>
        public readonly int Acceleration;
            
        public SetupInfo(int speed = LowLevelParams.DEFAULT_SPEED, int acceleration = LowLevelParams.DEFAULT_ACCELARATION)
        {
            Speed = speed;
            Acceleration = acceleration;
        }

        public override string ToString() => $"{Speed}{CommunicationParams.SEPARATOR}{Acceleration}";
    }
}