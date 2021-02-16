using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ConfuserEx.ViewModel
{
	// Token: 0x02000031 RID: 49
	public class ViewModelBase : INotifyPropertyChanged
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600017C RID: 380 RVA: 0x00006858 File Offset: 0x00004A58
		// (remove) Token: 0x0600017D RID: 381 RVA: 0x00006890 File Offset: 0x00004A90
		public event PropertyChangedEventHandler PropertyChanged
		{
			[CompilerGenerated]
			add
			{
				PropertyChangedEventHandler propertyChangedEventHandler = this.PropertyChanged;
				PropertyChangedEventHandler propertyChangedEventHandler2;
				do
				{
					propertyChangedEventHandler2 = propertyChangedEventHandler;
					PropertyChangedEventHandler value2 = (PropertyChangedEventHandler)Delegate.Combine(propertyChangedEventHandler2, value);
					propertyChangedEventHandler = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this.PropertyChanged, value2, propertyChangedEventHandler2);
				}
				while (propertyChangedEventHandler != propertyChangedEventHandler2);
			}
			[CompilerGenerated]
			remove
			{
				PropertyChangedEventHandler propertyChangedEventHandler = this.PropertyChanged;
				PropertyChangedEventHandler propertyChangedEventHandler2;
				do
				{
					propertyChangedEventHandler2 = propertyChangedEventHandler;
					PropertyChangedEventHandler value2 = (PropertyChangedEventHandler)Delegate.Remove(propertyChangedEventHandler2, value);
					propertyChangedEventHandler = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this.PropertyChanged, value2, propertyChangedEventHandler2);
				}
				while (propertyChangedEventHandler != propertyChangedEventHandler2);
			}
		}

		// Token: 0x0600017E RID: 382 RVA: 0x000068C8 File Offset: 0x00004AC8
		protected virtual void OnPropertyChanged(string property)
		{
			bool flag = this.PropertyChanged != null;
			if (flag)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}

		// Token: 0x0600017F RID: 383 RVA: 0x000068F8 File Offset: 0x00004AF8
		protected bool SetProperty<T>(ref T field, T value, string property)
		{
			bool flag = !EqualityComparer<T>.Default.Equals(field, value);
			bool result;
			if (flag)
			{
				field = value;
				this.OnPropertyChanged(property);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00006938 File Offset: 0x00004B38
		protected bool SetProperty<T>(bool changed, Action<T> setter, T value, string property)
		{
			bool result;
			if (changed)
			{
				setter(value);
				this.OnPropertyChanged(property);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00002119 File Offset: 0x00000319
		public ViewModelBase()
		{
		}

		// Token: 0x04000089 RID: 137
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PropertyChangedEventHandler PropertyChanged;
	}
}
