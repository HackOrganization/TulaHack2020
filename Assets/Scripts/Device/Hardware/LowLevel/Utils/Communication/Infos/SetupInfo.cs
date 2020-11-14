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
            
        public SetupInfo(int speed = CommunicationParams.DEFAULT_SPEED, int acceleration = CommunicationParams.DEFAULT_ACCELARATION)
        {
            Speed = speed;
            Acceleration = acceleration;
        }

        public override string ToString() => $"{Speed}{CommunicationParams.SEPARATOR}{Acceleration}";
    }
}