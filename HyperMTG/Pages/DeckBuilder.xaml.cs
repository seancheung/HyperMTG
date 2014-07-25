using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HyperKore.Common;
using HyperMTG.ViewModel;

namespace HyperMTG.Pages
{
	/// <summary>
	///     Interaction logic for DeckBuilder.xaml
	/// </summary>
	public partial class DeckBuilder
	{
		public DeckBuilder()
		{
			InitializeComponent();
		}

		private void OnDrag(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				object data = ((ListViewItem) sender).Content;
				if (data != null)
				{
					DragDrop.DoDragDrop((ListViewItem) sender, data, DragDropEffects.Copy);
				}
			}
		}

		private void Lbmain_OnDrop(object sender, DragEventArgs e)
		{
			var card = e.Data.GetData(typeof(Card)) as Card;
			if (card != null)
			{
				((DeckBuiderViewModel)DataContext).Deck.MainBoard.Add(card);
			}
		}

		private void Lbside_OnDrop(object sender, DragEventArgs e)
		{
			var card = e.Data.GetData(typeof(Card)) as Card;
			if (card != null)
			{
				((DeckBuiderViewModel)DataContext).Deck.SideBoard.Add(card);
			}
		}
	}
}