#region User settings
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public class ConveyorSetting
    {
        public ConveyorSetting(int angle, int number, int gripperAngle)
        {
            Number = number;
            Angle = angle;
            GripperAngle = gripperAngle;
        }
        public int Number { get; set; } 
        public int Angle { get; set; }
        public int GripperAngle { get; set; }
    }
}
