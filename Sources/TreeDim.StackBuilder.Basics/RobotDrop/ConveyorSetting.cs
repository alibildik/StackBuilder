#region User settings
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public class ConveyorSetting
    {
        public ConveyorSetting(int angle, int number)
        {
            Number = number;
            Angle = angle;
        }
        public int Angle { get; set; }
        public int Number { get; set; } 
    }
}
