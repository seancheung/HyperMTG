using System.Linq;
using System.Windows;
using System.Windows.Controls;

//http://www.silverlightchina.net/html/study/WPF/2011/0724/9221.html


namespace HyperMTG.Helper
{
	public class ColumnObject : DependencyObject
	{
		#region 附加属性

		public static DependencyProperty ItemsSourceFromColumnsProperty =
			DependencyProperty.RegisterAttached("ItemsSourceFromColumns", typeof (ListView), typeof (ColumnObject),
				new PropertyMetadata(OnItemsSourceFromColumnsChanged));

		private static void OnItemsSourceFromColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ColumnObject[] cols = null;

			if (e.NewValue is ListView && ((cols = GetColumns((ListView) e.NewValue)) != null))

				((ItemsControl) d).ItemsSource = cols;
		}

		public static ListView GetItemsSourceFromColumns(ItemsControl ic)
		{
			return (ListView) ic.GetValue(ItemsSourceFromColumnsProperty);
		}

		public static void SetItemsSourceFromColumns(ItemsControl ic, ListView lv)
		{
			ic.SetValue(ItemsSourceFromColumnsProperty, lv);
		}

		#endregion

		public static DependencyProperty IsVisibleProperty =
			DependencyProperty.Register("IsVisible", typeof (bool), typeof (ColumnObject),
				new PropertyMetadata(true, OnIsVisibleChanged));

		private readonly GridViewColumnCollection collec;

		private int index;

		public ColumnObject(GridViewColumn column, GridViewColumnCollection collec)
		{
			Column = column;

			this.collec = collec;
		}

		public GridViewColumn Column { get; private set; }

		private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ColumnObject cobj = (ColumnObject) d;

			if ((bool) e.NewValue)
			{
				//尝试还原位置，此时有可能由于别的列也被隐藏造成位置无效

				if (cobj.index < 0 || cobj.index > cobj.collec.Count)

					cobj.index = cobj.collec.Count - 1;

				cobj.collec.Insert(cobj.index, cobj.Column);
			}

			else
			{
				//记住隐藏时列的位置，显示的时候尝试排列在原来位置

				cobj.index = cobj.collec.IndexOf(cobj.Column);

				if (cobj.index != -1)

					cobj.collec.RemoveAt(cobj.index);
			}
		}

		public static ColumnObject[] GetColumns(ListView lv)
		{
			if (lv.View is GridView)
			{
				GridViewColumnCollection collec = ((GridView) lv.View).Columns;

				return collec.Select(col => new ColumnObject(col, collec)).ToArray();
			}

			return null;
		}
	}
}