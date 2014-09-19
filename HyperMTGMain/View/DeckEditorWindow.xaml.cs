using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using HyperKore.Common;
using HyperKore.Utilities;
using HyperMTGMain.ViewModel;

namespace HyperMTGMain.View
{
	public partial class DeckEditorWindow
	{
		public DeckEditorWindow()
		{
			InitializeComponent();
		}

		private void FrameworkElement_OnTargetUpdated(object sender, DataTransferEventArgs e)
		{
			var text = sender as TextBlock;
			if (text == null)
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(text.Text))
			{
				return;
			}

			IEnumerable<string> result = text.Text.ManaSplit();
			text.Inlines.Clear();
			foreach (string str in result)
			{
				if (Regex.IsMatch(str, "[{.+}]"))
				{
					var style = Application.Current.TryFindResource("Mana" + Regex.Replace(str, "{|}", "").ToUpper()) as Style;
					if (style != null)
					{
						var control = new UserControl
						{
							Style = style,
							Width = text.FontSize,
							Height = text.FontSize,
						};
						text.Inlines.Add(new InlineUIContainer(control));
					}
					else
					{
						text.Inlines.Add(str);
					}
				}
				else
				{
					text.Inlines.Add(str);
				}
			}
		}

		private void DeckEditorWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			ViewModelManager.MessageViewModel.Clear();
			ViewModelManager.DeckEditorViewModel.LoadCards();
		}

		private void EventSetter_OnHandler_List(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				object data = (sender as ListViewItem).Content;
				if (data != null)
				{
					DragDrop.DoDragDrop(sender as ListViewItem, data, DragDropEffects.Copy);
				}
			}
		}

		private void EventSetter_OnHandler_Main(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				var card = (sender as ListViewItem).Content as Card;
				if (card != null)
				{
					DragDrop.DoDragDrop(sender as ListViewItem, card, DragDropEffects.Move);
					if (ViewModelManager.DeckEditorViewModel.DeleteMainCommand.CanExecute(card))
					{
						ViewModelManager.DeckEditorViewModel.DeleteMainCommand.Execute(card);
					}
				}
			}
		}

		private void EventSetter_OnHandler_Side(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				var card = (sender as ListViewItem).Content as Card;
				if (card != null)
				{
					DragDrop.DoDragDrop(sender as ListViewItem, card, DragDropEffects.Move);
					if (ViewModelManager.DeckEditorViewModel.DeleteSideCommand.CanExecute(card))
					{
						ViewModelManager.DeckEditorViewModel.DeleteSideCommand.Execute(card);
					}
				}
			}
		}

		private void UIElement_OnDrop_Main(object sender, DragEventArgs e)
		{
			var card = e.Data.GetData(typeof (Card)) as Card;
			if (ViewModelManager.DeckEditorViewModel.AddMainCommand.CanExecute(card))
			{
				ViewModelManager.DeckEditorViewModel.AddMainCommand.Execute(card);
			}
		}

		private void UIElement_OnDragLeave_Main(object sender, DragEventArgs e)
		{
			var card = e.Data.GetData(typeof (Card)) as Card;
			if (ViewModelManager.DeckEditorViewModel.DeleteMainCommand.CanExecute(card))
			{
				ViewModelManager.DeckEditorViewModel.DeleteMainCommand.Execute(card);
			}
		}

		private void UIElement_OnDrop_Side(object sender, DragEventArgs e)
		{
			var card = e.Data.GetData(typeof (Card)) as Card;
			if (ViewModelManager.DeckEditorViewModel.AddSideCommand.CanExecute(card))
			{
				ViewModelManager.DeckEditorViewModel.AddSideCommand.Execute(card);
			}
		}

		private void UIElement_OnDragLeave_Side(object sender, DragEventArgs e)
		{
			var card = e.Data.GetData(typeof (Card)) as Card;
			if (ViewModelManager.DeckEditorViewModel.DeleteSideCommand.CanExecute(card))
			{
				ViewModelManager.DeckEditorViewModel.DeleteSideCommand.Execute(card);
			}
		}
	}
}