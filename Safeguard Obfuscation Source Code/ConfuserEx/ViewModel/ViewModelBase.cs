using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ConfuserEx.ViewModel
{
  public class ViewModelBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string property)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(property));
    }

    protected bool SetProperty<T>(ref T field, T value, string property)
    {
      if (EqualityComparer<T>.Default.Equals(field, value))
        return false;
      field = value;
      this.OnPropertyChanged(property);
      return true;
    }

    protected bool SetProperty<T>(bool changed, Action<T> setter, T value, string property)
    {
      if (!changed)
        return false;
      setter(value);
      this.OnPropertyChanged(property);
      return true;
    }
  }
}
