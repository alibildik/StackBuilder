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
        public override string ToString() => $"Number={Number}, Angle={Angle}, GripperAngle={GripperAngle}";
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
        public static bool CanRemove(List<ConveyorSetting> listConveyorSettings, int iSel)
        {
            var setting = listConveyorSettings[iSel];
            return !(setting.Number == 1 && (1 == listConveyorSettings.Count(cs => cs.Number == 1)));
        }
        public static bool CanUpdate(List<ConveyorSetting> listConveyorSettings, ConveyorSetting setting, int iSel) => -1 != iSel && !listConveyorSettings.Any(cs => cs.Equal(setting));
        public static bool CanAdd(List<ConveyorSetting> listConveyorSettings, ConveyorSetting setting)  => !listConveyorSettings.Any(cs => cs.Equal(setting));
        public static bool CanEditNumber(List<ConveyorSetting> listConveyorSettings, int iSel)
        {
            var setting = listConveyorSettings[iSel];
            return !(setting.Number == 1 && (1 == listConveyorSettings.Count(cs => cs.Number == 1)));
        }
        #endregion
    }
}
