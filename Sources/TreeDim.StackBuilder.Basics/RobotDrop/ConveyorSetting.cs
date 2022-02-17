#region User settings
using System.Collections.Generic;
using System.Linq;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public class ConveyorSetting
    {
        #region Constructor
        public ConveyorSetting(int angle, int number, int gripperAngle)
        {
            Number = number;
            Angle = angle;
            GripperAngle = gripperAngle;
        }
        #endregion
        #region Public properties
        public int Number { get; set; } 
        public int Angle { get; set; }
        public int GripperAngle { get; set; }
        #endregion
        #region Equal & Clone method
        public bool Equal(ConveyorSetting cs) => Number == cs.Number && Angle == cs.Angle && GripperAngle == cs.GripperAngle;
        public ConveyorSetting Clone() => new ConveyorSetting(Angle, Number, GripperAngle);
        #endregion
        #region Static methods
        public static bool FindSetting(List<ConveyorSetting> conveyorSettings, ConveyorSetting setting, int iSel)
        {
            // sanity check
            if (null == conveyorSettings) return false;
            // loop throw all settings
            for (int i = 0; i < conveyorSettings.Count; ++i)
            {
                if (i != iSel && conveyorSettings[i].Equal(setting))
                    return true;
            }
            return false;
        }
        public static bool CanRemove(List<ConveyorSetting> conveyorSettings, int iSel)
        {
            var setting = conveyorSettings[iSel];
            return !(setting.Number == 1 && (1 == conveyorSettings.Count(cs => cs.Number == 1)));
        }
        #endregion
    }
}
