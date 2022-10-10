using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;

namespace treeDiM.StackBuilder.Basics
{
    /// <summary>
    /// This class holds Name / description properties for Box / Pallet / Interlayer / Analysis
    /// it also handle dependancy problems
    /// </summary>
    public abstract class ItemBase : IDisposable
    {
        public ItemBase(Document document)
        {
            _parentDocument = document;
        }

        public abstract GlobID ID { get; }

        public Document ParentDocument => _parentDocument;
        public string Name => ID.Name;
        public string Description => ID.Description;
        public List<ItemBase> DirectDependancies => _dependancies;
        public List<ItemBase> Dependancies
        {
            get
            {
                List<ItemBase> listDependancies = new List<ItemBase>();
                foreach (var dep in _dependancies)
                {
                    listDependancies.Add(dep);
                    listDependancies.AddRange(dep.Dependancies);
                }
                return listDependancies;
            }
        }
        public bool HasDependancies => _dependancies.Count > 0;
        public string DependancyString => string.Join(Environment.NewLine, Dependancies.Select(d => d.Name).ToArray());

        public virtual void AddDependancy(ItemBase dependancy)
        {
            // if analysis is temporary, do not record dependancy
            if (dependancy is AnalysisLayered analysis && analysis.Temporary)
                return;
            if (dependancy is AnalysisHetero hAnalysis && hAnalysis.Temporary)
                return;
            // check if dependancy already recorded
            if (_dependancies.Contains(dependancy))
            {
                _log.Warn(string.Format("Tried to add {0} as a dependancy of {1} a second time!", dependancy.ID.Name, ID.Name));
                return;
            }
            // actually add dependancy
            _dependancies.Add(dependancy);    
        }

        public virtual void RemoveDependancy(ItemBase dependancie)
        {
            _dependancies.Remove(dependancie);  
        }

        public void EndUpdate()
        {
            // update dependancies
            foreach (ItemBase item in _dependancies)
                item.OnEndUpdate(this);
        }

        public virtual void OnAttributeModified(ItemBase modifiedAttribute) {}

        public virtual void OnEndUpdate(ItemBase updatedAttribute) {}

        public void AddListener(IItemListener listener)
        {
            _listeners.Add(listener);
        }

        public void RemoveListener(IItemListener listener)
        {
            _listeners.Remove(listener);
        }

        public override string ToString()
        {
            return string.Format("Name:{0} \nDescription: {1}\n", ID.Name, ID.Description);
        }

        #region Non-Public/Disposable Members

        private Document _parentDocument;
        private List<ItemBase> _dependancies = new List<ItemBase>();
        private bool disposed = false;
        List<IItemListener> _listeners = new List<IItemListener>();
        static readonly ILog _log = LogManager.GetLogger(typeof(ItemBase));

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            // TODO - adjust for correctness (we don't have a finalizer, Dispose(bool) accessing non-this outside of disposing=true
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects.
        private void Dispose(bool disposing)
        {
            // TODO - adjust for correctness (we don't have a finalizer, Dispose(bool) accessing non-this outside of disposing=true
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                OnDispose();
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources
                    int iCount = _dependancies.Count;
                    while (_dependancies.Count > 0)
                    {
                        try { _parentDocument.RemoveItem(_dependancies[0]); } catch (Exception ex) { _log.Error(ex.ToString()); }
                        if (_dependancies.Count == iCount)
                        {
                            _log.Warn(string.Format("Failed to remove correctly dependancy {0} ", _dependancies[0].ID.Name));
                            _dependancies.Remove(_dependancies[0]);
                            break;
                        }
                    }
                }
                RemoveItselfFromDependancies();
                KillListeners();
                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false, only the following code is executed.

                // Note disposing has been done.
                disposed = true;
            }
        }
        protected void KillListeners()
        {
            while (_listeners.Count > 0)
                _listeners[0].Kill(this);
        }
        protected void Modify()
        {
            // update dependancies
            foreach (ItemBase item in _dependancies)
                item.OnAttributeModified(this);
            // update listeners
            UpdateListeners();
            // update parent document
            if (null != _parentDocument)
                _parentDocument.Modify();
        }
        protected virtual void OnDispose() { }
        protected virtual void RemoveItselfFromDependancies() { }
        protected void UpdateListeners()
        {
            foreach (IItemListener listener in _listeners)
                listener.Update(this);
        }

        #endregion
    }
}
