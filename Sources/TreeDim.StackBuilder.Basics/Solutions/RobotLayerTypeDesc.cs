#region Using directives
using System;
using System.Text.RegularExpressions;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public class RobotLayerTypeDesc
    {
        #region Constructor
        public RobotLayerTypeDesc(LayerDescBox layerDesc, bool symetryX, bool symetryY)
        {
            LayerDesc = layerDesc;
            SymetryX = symetryX;
            SymetryY = symetryY;
        }
        #endregion
        #region Public properties
        public LayerDescBox LayerDesc { get;}
        public bool SymetryX { get; }
        public bool SymetryY { get; }
        #endregion

        #region Object override
        public override string ToString() => $"{LayerDesc}|{SymetryXStr}|{SymetryYStr}";
        public override bool Equals(object obj)
        {
            if (obj is RobotLayerTypeDesc robotLayerTypeDesc)
                return robotLayerTypeDesc.LayerDesc.Equals(LayerDesc)
                    && robotLayerTypeDesc.SymetryX == SymetryX
                    && robotLayerTypeDesc.SymetryX == SymetryX;
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode()
                ^ LayerDesc.GetHashCode()
                ^ SymetryX.GetHashCode()
                ^ SymetryY.GetHashCode();
        }
        #endregion
        #region Static methods
        public static RobotLayerTypeDesc Parse(string value)
        {
            Regex r = new Regex(@"(?<name>.*)\|(?<axis>.*)\|(?<swap>.*)\|(?<symetryX>.*)\|(?<symetryY>.*)", RegexOptions.Singleline);
            Match m = r.Match(value);
            if (m.Success)
            {
                string patternName = m.Result("${name}");
                HalfAxis.HAxis axis = HalfAxis.Parse(m.Result("${axis}"));
                bool swapped = string.Equals("t", m.Result("${swap}"), StringComparison.CurrentCultureIgnoreCase);
                bool symetryX = string.Equals("t", m.Result("${symetryX}"), StringComparison.CurrentCultureIgnoreCase);
                bool symetryY = string.Equals("t", m.Result("${symetryY}"), StringComparison.CurrentCultureIgnoreCase);
                return new RobotLayerTypeDesc(new LayerDescBox(patternName, axis, swapped), symetryX, symetryY);
            }
            else
                throw new Exception($"Failed to parse RobotLayerTypeDesc : {value}");
        }
        public static bool TryParse(string value, out RobotLayerTypeDesc robotLayerTypeDesc)
        {
            Regex r = new Regex(@"(?<name>.*)\|(?<axis>.*)\|(?<swap>.*)\|(?<symetryX>.*)\|(?<symetryY>.*)", RegexOptions.Singleline);
            Match m = r.Match(value);
            if (m.Success)
            {
                string patternName = m.Result("${name}");
                HalfAxis.HAxis axis = HalfAxis.Parse(m.Result("${axis}"));
                bool swapped = string.Equals("t", m.Result("${swap}"), StringComparison.CurrentCultureIgnoreCase);
                bool symetryX = string.Equals("t", m.Result("${symetryX}"), StringComparison.CurrentCultureIgnoreCase);
                bool symetryY = string.Equals("t", m.Result("${symetryY}"), StringComparison.CurrentCultureIgnoreCase);

                robotLayerTypeDesc = new RobotLayerTypeDesc(new LayerDescBox(patternName, axis, swapped), symetryX, symetryY);
                return true;
            }
            robotLayerTypeDesc = null;
            return false;
        }
        #endregion
            #region Private helpers
        private string SymetryXStr => SymetryX ? "t" : "f";
        private string SymetryYStr => SymetryY ? "t" : "f";
        #endregion
    }
}
