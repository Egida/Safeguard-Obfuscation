using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ConfuserEx.ViewModel
{
	// Token: 0x0200002E RID: 46
	public static class Utils
	{
		// Token: 0x06000175 RID: 373 RVA: 0x000064B0 File Offset: 0x000046B0
		public static ObservableCollection<T> Wrap<T>(IList<T> list)
		{
			ObservableCollection<T> observableCollection = new ObservableCollection<T>(list);
			observableCollection.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				ObservableCollection<T> observableCollection2 = (ObservableCollection<T>)sender;
				switch (e.Action)
				{
				case NotifyCollectionChangedAction.Add:
					for (int i = 0; i < e.NewItems.Count; i++)
					{
						list.Insert(e.NewStartingIndex + i, (T)((object)e.NewItems[i]));
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					for (int j = 0; j < e.OldItems.Count; j++)
					{
						list.RemoveAt(e.OldStartingIndex);
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					list[e.NewStartingIndex] = (T)((object)e.NewItems[0]);
					break;
				case NotifyCollectionChangedAction.Move:
					list.RemoveAt(e.OldStartingIndex);
					list.Insert(e.NewStartingIndex, (T)((object)e.NewItems[0]));
					break;
				case NotifyCollectionChangedAction.Reset:
					list.Clear();
					foreach (T item in observableCollection2)
					{
						list.Add(item);
					}
					break;
				}
			};
			return observableCollection;
		}

		// Token: 0x06000176 RID: 374 RVA: 0x000064F0 File Offset: 0x000046F0
		public static ObservableCollection<TViewModel> Wrap<TModel, TViewModel>(IList<TModel> list, Func<TModel, TViewModel> transform) where TViewModel : IViewModel<TModel>
		{
			ObservableCollection<TViewModel> observableCollection = new ObservableCollection<TViewModel>(from item in list
			select transform(item));
			observableCollection.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				ObservableCollection<TViewModel> observableCollection2 = (ObservableCollection<TViewModel>)sender;
				switch (e.Action)
				{
				case NotifyCollectionChangedAction.Add:
					for (int i = 0; i < e.NewItems.Count; i++)
					{
						IList<TModel> list2 = list;
						int index = e.NewStartingIndex + i;
						TViewModel tviewModel = (TViewModel)((object)e.NewItems[i]);
						list2.Insert(index, tviewModel.Model);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					for (int j = 0; j < e.OldItems.Count; j++)
					{
						list.RemoveAt(e.OldStartingIndex);
					}
					break;
				case NotifyCollectionChangedAction.Replace:
				{
					IList<TModel> list3 = list;
					int newStartingIndex = e.NewStartingIndex;
					TViewModel tviewModel = (TViewModel)((object)e.NewItems[0]);
					list3[newStartingIndex] = tviewModel.Model;
					break;
				}
				case NotifyCollectionChangedAction.Move:
				{
					list.RemoveAt(e.OldStartingIndex);
					IList<TModel> list4 = list;
					int newStartingIndex2 = e.NewStartingIndex;
					TViewModel tviewModel = (TViewModel)((object)e.NewItems[0]);
					list4.Insert(newStartingIndex2, tviewModel.Model);
					break;
				}
				case NotifyCollectionChangedAction.Reset:
					list.Clear();
					foreach (TViewModel tviewModel2 in observableCollection2)
					{
						list.Add(tviewModel2.Model);
					}
					break;
				}
			};
			return observableCollection;
		}

		// Token: 0x0200002F RID: 47
		[CompilerGenerated]
		private sealed class <>c__DisplayClass0_0<T>
		{
			// Token: 0x06000177 RID: 375 RVA: 0x00002119 File Offset: 0x00000319
			public <>c__DisplayClass0_0()
			{
			}

			// Token: 0x06000178 RID: 376 RVA: 0x00006548 File Offset: 0x00004748
			internal void <Wrap>b__0(object sender, NotifyCollectionChangedEventArgs e)
			{
				ObservableCollection<T> observableCollection = (ObservableCollection<T>)sender;
				switch (e.Action)
				{
				case NotifyCollectionChangedAction.Add:
					for (int i = 0; i < e.NewItems.Count; i++)
					{
						this.list.Insert(e.NewStartingIndex + i, (T)((object)e.NewItems[i]));
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					for (int j = 0; j < e.OldItems.Count; j++)
					{
						this.list.RemoveAt(e.OldStartingIndex);
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					this.list[e.NewStartingIndex] = (T)((object)e.NewItems[0]);
					break;
				case NotifyCollectionChangedAction.Move:
					this.list.RemoveAt(e.OldStartingIndex);
					this.list.Insert(e.NewStartingIndex, (T)((object)e.NewItems[0]));
					break;
				case NotifyCollectionChangedAction.Reset:
					this.list.Clear();
					foreach (T item in observableCollection)
					{
						this.list.Add(item);
					}
					break;
				}
			}

			// Token: 0x04000086 RID: 134
			public IList<T> list;
		}

		// Token: 0x02000030 RID: 48
		[CompilerGenerated]
		private sealed class <>c__DisplayClass1_0<TModel, TViewModel> where TViewModel : IViewModel<TModel>
		{
			// Token: 0x06000179 RID: 377 RVA: 0x00002119 File Offset: 0x00000319
			public <>c__DisplayClass1_0()
			{
			}

			// Token: 0x0600017A RID: 378 RVA: 0x00002B70 File Offset: 0x00000D70
			internal TViewModel <Wrap>b__0(TModel item)
			{
				return this.transform(item);
			}

			// Token: 0x0600017B RID: 379 RVA: 0x000066B4 File Offset: 0x000048B4
			internal void <Wrap>b__1(object sender, NotifyCollectionChangedEventArgs e)
			{
				ObservableCollection<TViewModel> observableCollection = (ObservableCollection<TViewModel>)sender;
				switch (e.Action)
				{
				case NotifyCollectionChangedAction.Add:
					for (int i = 0; i < e.NewItems.Count; i++)
					{
						IList<TModel> list = this.list;
						int index = e.NewStartingIndex + i;
						TViewModel tviewModel = (TViewModel)((object)e.NewItems[i]);
						list.Insert(index, tviewModel.Model);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					for (int j = 0; j < e.OldItems.Count; j++)
					{
						this.list.RemoveAt(e.OldStartingIndex);
					}
					break;
				case NotifyCollectionChangedAction.Replace:
				{
					IList<TModel> list2 = this.list;
					int newStartingIndex = e.NewStartingIndex;
					TViewModel tviewModel = (TViewModel)((object)e.NewItems[0]);
					list2[newStartingIndex] = tviewModel.Model;
					break;
				}
				case NotifyCollectionChangedAction.Move:
				{
					this.list.RemoveAt(e.OldStartingIndex);
					IList<TModel> list3 = this.list;
					int newStartingIndex2 = e.NewStartingIndex;
					TViewModel tviewModel = (TViewModel)((object)e.NewItems[0]);
					list3.Insert(newStartingIndex2, tviewModel.Model);
					break;
				}
				case NotifyCollectionChangedAction.Reset:
					this.list.Clear();
					foreach (TViewModel tviewModel2 in observableCollection)
					{
						this.list.Add(tviewModel2.Model);
					}
					break;
				}
			}

			// Token: 0x04000087 RID: 135
			public Func<TModel, TViewModel> transform;

			// Token: 0x04000088 RID: 136
			public IList<TModel> list;
		}
	}
}
