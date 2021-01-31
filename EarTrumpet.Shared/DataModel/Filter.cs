using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EarTrumpet.Shared.DataModel
{
    class Filter<T> where T : INotifyPropertyChanged
    {
        public ObservableCollection<T> VisibleItems { get; }

        private List<T> _all = new List<T>();
        private Func<T, bool> _doInclude;

        public Filter(ObservableCollection<T> items, Func<T, bool> doInclude)
        {
            VisibleItems = new ObservableCollection<T>();
            _doInclude = doInclude;

            items.CollectionChanged += Items_CollectionChanged;
            foreach (var s in items)
            {
                OnItemAddedToCollection(s);
            }
        }

        public void Refresh()
        {
            foreach (var s in _all.ToArray())
            {
                DoApplicabilityCheck(s);
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DoApplicabilityCheck((T)sender);
        }

        private void DoApplicabilityCheck(T s)
        {
            if (_doInclude.Invoke(s))
            {
                AddSession(s);
            }
            else
            {
                RemoveSession(s);
            }
        }

        private void OnItemAddedToCollection(T item)
        {
            item.PropertyChanged += Item_PropertyChanged;
            _all.Add(item);

            DoApplicabilityCheck(item);
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnItemAddedToCollection((T)e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var oldSession = (T)e.OldItems[0];
                        _all.Remove(oldSession);
                        oldSession.PropertyChanged -= Item_PropertyChanged;
                        RemoveSession(oldSession);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    throw new NotImplementedException();
            }
        }

        private void RemoveSession(T item)
        {
            if (VisibleItems.Contains(item))
            {
                VisibleItems.Remove(item);
            }
        }

        private void AddSession(T item)
        {
            if (!VisibleItems.Contains(item))
            {
                VisibleItems.Add(item);
            }
        }
    }
}