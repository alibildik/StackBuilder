#region Using directives
using System;

using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Engine.TestDimContainerIncrease
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var layerSolver = new LayerSolver();

            var dimBox = new Vector3D(400, 300, 100);
            var dimContainer = new Vector2D(1195, 995);
            var constraintSet = new ConstraintSetCasePallet() { };
            constraintSet.SetAllowedOrientations(new bool[] { false, false, true });
            constraintSet.SetMaxHeight(new OptDouble(true, 1800.0));
            int countFrom = 0, countTo = 0;

            Vector2D dimContainerTo = Vector2D.Zero;
            Console.WriteLine($"Container Dir0");
            if (layerSolver.FindMinPalletDimXYIncreaseForGain(dimBox, dimContainer, 144, constraintSet, 0, ref countFrom, ref countTo, ref dimContainerTo))
            {
                Console.WriteLine($"From {countFrom} to {countTo}");
                Console.WriteLine($"when container from {dimContainer} to {dimContainerTo}");
            }
            Console.WriteLine($"Container Dir1");
            if (layerSolver.FindMinPalletDimXYIncreaseForGain(dimBox, dimContainer, 144, constraintSet, 1, ref countFrom, ref countTo, ref dimContainerTo))
            {
                Console.WriteLine($"From {countFrom} to {countTo}");
                Console.WriteLine($"when container from {dimContainer} to {dimContainerTo}");
            }
            Console.WriteLine($"Container DirBoth");
            if (layerSolver.FindMinPalletDimXYIncreaseForGain(dimBox, dimContainer, 144, constraintSet, -1, ref countFrom, ref countTo, ref dimContainerTo))
            {
                Console.WriteLine($"From {countFrom} to {countTo}");
                Console.WriteLine($"when container from {dimContainer} to {dimContainerTo}");
            }

            Vector3D dimBoxTo = Vector3D.Zero;
            Console.WriteLine($"Box Dir0");
            if (layerSolver.FindMinDimXYBoxDecreaseForGain(dimBox, dimContainer, 144, constraintSet, 0, ref countFrom, ref countTo, ref dimBoxTo))
            {
                Console.WriteLine($"From {countFrom} to {countTo}");
                Console.WriteLine($"when container from {dimBox} to {dimBoxTo}");
            }
            Console.WriteLine($"Box Dir1");
            if (layerSolver.FindMinDimXYBoxDecreaseForGain(dimBox, dimContainer, 144, constraintSet, 1, ref countFrom, ref countTo, ref dimBoxTo))
            {
                Console.WriteLine($"From {countFrom} to {countTo}");
                Console.WriteLine($"when container from {dimBox} to {dimBoxTo}");
            }
        }
    }
}
