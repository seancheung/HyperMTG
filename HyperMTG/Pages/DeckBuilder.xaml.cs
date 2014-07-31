using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using FirstFloor.ModernUI.Windows.Controls;
using HyperKore.Utilities;

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

		private void FrameworkElement_OnTargetUpdated(object sender, DataTransferEventArgs e)
		{
			var block = sender as BBCodeBlock;
			if (block == null)
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(block.BBCode))
			{
				return;
			}

			IEnumerable<string> result = block.BBCode.ManaSplit();
			block.Inlines.Clear();
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
							Width = block.FontSize,
							Height = block.FontSize,
						};
						block.Inlines.Add(new InlineUIContainer(control));
					}
					else
					{
						block.Inlines.Add(str);
					}
				}
				else
				{
					block.Inlines.Add(str);
				}
			}
		}

	}
}