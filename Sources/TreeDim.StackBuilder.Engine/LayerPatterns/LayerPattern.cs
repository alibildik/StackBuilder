using treeDiM.StackBuilder.Basics;
using Sharp3D.Math.Core;

namespace treeDiM.StackBuilder.Engine
{
    abstract class LayerPattern
    { 
        public abstract string Name { get; }
        public abstract bool GetLayerDimensions(ILayer2D layer, out double actualLength, out double actualWidth);
        public abstract void GenerateLayer(ILayer2D layer, double actualLength, double actualWidth);
        public virtual bool CanBeSwapped => false;
        public bool GetLayerDimensionsChecked(ILayer2D layer, out double actualLength, out double actualWidth)
        {
            bool result = GetLayerDimensions(layer, out actualLength, out actualWidth);
            if (result && actualLength > GetPalletLength(layer))
                throw new EngineException($"Pattern name={Name} : actualLength={actualLength} > palletLength={GetPalletLength(layer)} ?");
            if (result && actualWidth > GetPalletWidth(layer))
                throw new EngineException($"Pattern name={Name} : actualWidth={actualWidth} > palletWidth={GetPalletWidth(layer)} ?");
            return result;
        }
        #region Non-Public Members
        protected double GetPalletLength(ILayer2D layer)
        {
            return layer.Swapped ? layer.Width : layer.Length;
        }
        protected double GetPalletWidth(ILayer2D layer)
        {
            return layer.Swapped ? layer.Length : layer.Width;
        }
        protected Vector2D GetOffset(ILayer2D layer, double actualLength, double actualWidth)
        {
            return new Vector2D(
                0.5*(GetPalletLength(layer) - actualLength),
                0.5*(GetPalletWidth(layer) - actualWidth)
                );
        }
        #endregion
    }
}
