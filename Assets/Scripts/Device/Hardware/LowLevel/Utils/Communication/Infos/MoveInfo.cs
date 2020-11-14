namespace Device.Hardware.LowLevel.Utils.Communication.Infos
{
    /// <summary>
    /// Класс позиции шаговика
    /// </summary>
    public class MoveInfo: ICommandInfo
    {
        /// <summary>
        /// Позиция (в шагах)
        /// </summary>
        public readonly int Position;

        public MoveInfo(int position = 0)
        {
            Position = position;
        }

        public override string ToString() => $"{Position}";
    }
}