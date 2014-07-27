using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace HyperMTG.Resources
{
	/// <summary>
	///     Interaction logic for RichTextBlock.xaml
	/// </summary>
	public partial class RichTextBlock : UserControl, INotifyPropertyChanged
	{
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Content", typeof (string), typeof (RichTextBlock), new PropertyMetadata(0));

		public RichTextBlock()
		{
			InitializeComponent();
		}

		public string Text
		{
			get { return (string) GetValue(ContentProperty); }
			set
			{
				SetValue(ContentProperty, value);
				OnPropertyChanged("Text");
				ParseText();
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		private void ParseText()
		{
			foreach (string str in Extract(Text))
			{
				if (Regex.IsMatch(str, "{.*}"))
				{
					var iuc = new InlineUIContainer(new Path {Style = (Style) Resources["SwampStyle"]});
					DisplayTextBlock.Inlines.Add(iuc);
				}
				else
				{
					DisplayTextBlock.Inlines.Add(new Run {Text = str});
				}
			}
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		///     Split text by mana expressions
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public IEnumerable<string> Extract(string text)
		{
			var idx = new List<int>();

			int t = 0;
			while (text.IndexOf("{") >= 0)
			{
				int a = text.IndexOf("{", t);
				if (a >= 0)
				{
					idx.Add(a);
					int b = text.IndexOf("}", a);
					if (b >= 0) idx.Add(b);
					else break;

					t = b;
				}
				else break;
			}

			if (idx.Count > 0)
			{
				if (idx[0] > 0) yield return text.Substring(0, idx[0]);
			}
			else
			{
				yield return text;
				yield break;
			}

			for (int i = 0; i < idx.Count; i++)
			{
				if (i + 1 >= idx.Count) break;
				if (idx[i] + 1 == idx[i + 1]) continue;

				string s = text.Substring(idx[i], idx[i + 1] - idx[i] + 1);
				if (s.StartsWith("}")) s = s.Substring(1);
				if (s.EndsWith("{")) s = s.Remove(s.Length - 1);

				yield return s;
			}

			yield return text.Substring(idx[idx.Count - 1] + 1);
		}
	}
}