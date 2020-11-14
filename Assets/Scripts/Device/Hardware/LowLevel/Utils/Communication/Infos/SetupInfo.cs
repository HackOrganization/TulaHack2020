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
            
        public SetupInfo(int speed = 0, int acceleration = 0)
        {
            Speed = speed;
            Acceleration = acceleration;
        }

        public override string ToString() => $"{Speed}{CommunicationParams.SEPARATOR}{Acceleration}";
    }
}