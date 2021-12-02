using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace treeDiM.StackBuilder.Basics
{
    /*
    /// <summary>
    /// Gathers a set of constraint to be used while computing solutions
    /// </summary>
    public abstract class PalletConstraintSet
    {
        public PalletConstraintSet()
        {
        }
        public bool IsValid
        {
            get
            {
                bool hasValidStopCriterion =
                   UseMaximumNumberOfCases
                || UseMaximumHeight
                || UseMaximumWeightOnBox
                || UseMaximumPalletWeight;

                bool allowsAtLeastOneOrthoAxis = false;
                for (int i=0; i<6; ++i)
                {
                    if (AllowOrthoAxis((HalfAxis.HAxis)i))
                    {
                        allowsAtLeastOneOrthoAxis = true;
                        break;
                    }
                }

                return hasValidStopCriterion
                    && allowsAtLeastOneOrthoAxis
                    && (AllowAlignedLayers || AllowAlternateLayers)
                    && _allowedPatterns.Count > 0
                    && (!UseNumberOfSolutionsKept || _noSolutionsKept > 0);
            }
        }

        public bool AllowAlignedLayers { get; set; } = false;
        public bool AllowAlternateLayers { get; set; } = true;
        public bool UseMaximumNumberOfCases { get; set; }
        public bool UseMaximumHeight { get; set; }
        public bool UseMaximumPalletWeight { get; set; }
        public abstract bool UseMaximumWeightOnBox { get; set; }

        public int MaximumNumberOfItems { get; set; }
        public double MaximumPalletWeight { get; set; }
        public double MaximumHeight { get; set; }

        public abstract double MaximumWeightOnBox { get; set; }

        public void ClearAllowedPatterns()
        {
            _allowedPatterns.Clear();
        }
        public void SetAllowedPattern(string patternName)
        {
            if ( patternName == string.Empty 
                || _allowedPatterns.Contains(patternName) )  return;
            _allowedPatterns.Add(patternName);
        }
        public bool AllowPattern(string patternName)
        {
            if ((string.Equals(patternName, "Symetric Interlocked", StringComparison.CurrentCultureIgnoreCase))
            || (string.Equals(patternName, "Interlocked Filled", StringComparison.CurrentCultureIgnoreCase)))
                return _allowedPatterns.Contains("Interlocked");
            else
                return _allowedPatterns.Contains(patternName);
        }
        public string AllowedPatternString
        {
            get
            {
                var valid = _allowedPatterns.Where(x => !string.IsNullOrEmpty(x));
                return string.Join(",", valid);
            }
            set
            {
                string[] patternNames = value.Split(',');
                foreach (string patternName in patternNames)
                    SetAllowedPattern(patternName);
            }
        }

        public abstract bool AllowOrthoAxis(HalfAxis.HAxis orthoAxis);
        public string AllowOrthoAxisString
        {
            get
            {
                var allowed = Enumerable.Range(0, 6).Select(i => (HalfAxis.HAxis)i).Where(AllowOrthoAxis);
                return string.Join(",", allowed);
            }
        }
        public abstract bool AllowTwoLayerOrientations { get; set; }
        public abstract bool AllowLastLayerOrientationChange { get; set; }

        public double OverhangX { get; set; }
        public double OverhangY { get; set; }

        public abstract bool HasInterlayer { get; set; }
        public abstract int InterlayerPeriod { get; set; }
        public abstract bool HasInterlayerAntiSlip { get; set; }


        // TODO - replace with Nullable<int>?
        public bool UseNumberOfSolutionsKept { get; set; }
        public int NumberOfSolutionsKept
        {
            get { return _noSolutionsKept; }
            set
            {
                UseNumberOfSolutionsKept = true;
                _noSolutionsKept = value;
            }
        }

        public int PalletFilmTurns { get; set; } = 1;

        public bool AllowNewLayer(int iNoLayer)
        {
            return !UseMaximumWeightOnBox;
        }
        public bool AllowNewBox(int iNoBox)
        {
            return !UseMaximumNumberOfCases || (iNoBox <= MaximumNumberOfItems);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (UseMaximumHeight) sb.AppendLine($"Maximum height = {MaximumHeight}");
            if (UseMaximumPalletWeight) sb.AppendLine($"Maximum pallet weight = {MaximumPalletWeight}");
            if (UseMaximumWeightOnBox) sb.AppendLine($"Maximum weight on box = {0.0}");
            if (UseMaximumNumberOfCases) sb.AppendLine($"Maximum number of items = {MaximumNumberOfItems}");
            return sb.ToString();
        }

        #region Non-Public Members

        private List<string> _allowedPatterns = new List<string>();
        private int _noSolutionsKept;

        #endregion
    }
    */
}
