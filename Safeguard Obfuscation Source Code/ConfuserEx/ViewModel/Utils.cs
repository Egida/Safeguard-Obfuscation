using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace ConfuserEx.ViewModel
{
  public static class Utils
  {
    public static ObservableCollection<T> Wrap<T>(IList<T> list)
    {
      ObservableCollection<T> observableCollection1 = new ObservableCollection<T>((IEnumerable<T>) list);
      observableCollection1.CollectionChanged += (NotifyCollectionChangedEventHandler) ((sender, e) =>
      {
        ObservableCollection<T> observableCollection2 = (ObservableCollection<T>) sender;
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            for (int index = 0; index < e.NewItems.Count; ++index)
              list.Insert(e.NewStartingIndex + index, (T) e.NewItems[index]);
            break;
          case NotifyCollectionChangedAction.Remove:
            for (int index = 0; index < e.OldItems.Count; ++index)
              list.RemoveAt(e.OldStartingIndex);
            break;
          case NotifyCollectionChangedAction.Replace:
            list[e.NewStartingIndex] = (T) e.NewItems[0];
            break;
          case NotifyCollectionChangedAction.Move:
            list.RemoveAt(e.OldStartingIndex);
            list.Insert(e.NewStartingIndex, (T) e.NewItems[0]);
            break;
          case NotifyCollectionChangedAction.Reset:
            ((ICollection<T>) list).Clear();
            using (IEnumerator<T> enumerator = observableCollection2.GetEnumerator())
            {
              while (enumerator.MoveNext())
                ((ICollection<T>) list).Add(enumerator.Current);
              break;
            }
        }
      });
      return observableCollection1;
    }

    public static ObservableCollection<TViewModel> Wrap<TModel, TViewModel>(
      IList<TModel> list,
      Func<TModel, TViewModel> transform)
      where TViewModel : IViewModel<TModel>
    {
      ObservableCollection<TViewModel> observableCollection1 = new ObservableCollection<TViewModel>(list.Select<TModel, TViewModel>((Func<TModel, TViewModel>) (item => transform(item))));
      observableCollection1.CollectionChanged += (NotifyCollectionChangedEventHandler) ((sender, e) =>
      {
        ObservableCollection<TViewModel> observableCollection2 = (ObservableCollection<TViewModel>) sender;
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            for (int index = 0; index < e.NewItems.Count; ++index)
              list.Insert(e.NewStartingIndex + index, ((TViewModel) e.NewItems[index]).Model);
            break;
          case NotifyCollectionChangedAction.Remove:
            for (int index = 0; index < e.OldItems.Count; ++index)
              list.RemoveAt(e.OldStartingIndex);
            break;
          case NotifyCollectionChangedAction.Replace:
            list[e.NewStartingIndex] = ((TViewModel) e.NewItems[0]).Model;
            break;
          case NotifyCollectionChangedAction.Move:
            list.RemoveAt(e.OldStartingIndex);
            list.Insert(e.NewStartingIndex, ((TViewModel) e.NewItems[0]).Model);
            break;
          case NotifyCollectionChangedAction.Reset:
            ((ICollection<TModel>) list).Clear();
            using (IEnumerator<TViewModel> enumerator = observableCollection2.GetEnumerator())
            {
              while (enumerator.MoveNext())
                ((ICollection<TModel>) list).Add(enumerator.Current.Model);
              break;
            }
        }
      });
      return observableCollection1;
    }
  }
}
