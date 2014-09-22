using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;

namespace HyperMTGMain.Helper
{
	public static class RichTextboxAssistant
	{
		public static readonly DependencyProperty BoundDocument =
			DependencyProperty.RegisterAttached("BoundDocument", typeof (string), typeof (RichTextboxAssistant),
				new FrameworkPropertyMetadata(null,
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					OnBoundDocumentChanged));

		private static void OnBoundDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var box = d as RichTextBox;

			if (box == null)
				return;

			RemoveEventHandler(box);

			string newXAML = GetBoundDocument(d);

			box.Document.Blocks.Clear();

			if (!string.IsNullOrEmpty(newXAML))
			{
				using (var xamlMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(newXAML)))
				{
					var parser = new ParserContext();
					parser.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
					parser.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
					var doc = new FlowDocument();

					var section = XamlReader.Load(xamlMemoryStream, parser) as Section;
					box.Document.Blocks.Add(section);
				}
			}

			AttachEventHandler(box);
		}

		private static void RemoveEventHandler(RichTextBox box)
		{
			Binding binding = BindingOperations.GetBinding(box, BoundDocument);

			if (binding != null)
			{
				if (binding.UpdateSourceTrigger == UpdateSourceTrigger.Default ||
				    binding.UpdateSourceTrigger == UpdateSourceTrigger.LostFocus)
				{
					box.LostFocus -= HandleLostFocus;
				}
				else
				{
					box.TextChanged -= HandleTextChanged;
				}
			}
		}

		private static void AttachEventHandler(RichTextBox box)
		{
			Binding binding = BindingOperations.GetBinding(box, BoundDocument);
			if (binding != null)
			{
				if (binding.UpdateSourceTrigger == UpdateSourceTrigger.Default ||
				    binding.UpdateSourceTrigger == UpdateSourceTrigger.LostFocus)
				{
					box.LostFocus += HandleLostFocus;
				}
				else
				{
					box.TextChanged += HandleTextChanged;
				}
			}
		}

		private static void HandleLostFocus(object sender, RoutedEventArgs e)
		{
			var box = sender as RichTextBox;
			var tr = new TextRange(box.Document.ContentStart, box.Document.ContentEnd);
			using (var ms = new MemoryStream())
			{
				tr.Save(ms, DataFormats.Xaml);
				string xamlText = Encoding.Default.GetString(ms.ToArray());
				SetBoundDocument(box, xamlText);
			}
		}

		private static void HandleTextChanged(object sender, RoutedEventArgs e)
		{
			// TODO: TextChanged is currently not working!
			var box = sender as RichTextBox;
			var tr = new TextRange(box.Document.ContentStart,
				box.Document.ContentEnd);

			using (var ms = new MemoryStream())
			{
				tr.Save(ms, DataFormats.Xaml);
				string xamlText = Encoding.Default.GetString(ms.ToArray());
				SetBoundDocument(box, xamlText);
			}
		}

		public static string GetBoundDocument(DependencyObject dp)
		{
			return dp.GetValue(BoundDocument) as string;
		}

		public static void SetBoundDocument(DependencyObject dp, string value)
		{
			dp.SetValue(BoundDocument, value);
		}
	}
}