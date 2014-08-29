using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace HyperStyle.Controls
{
	/// <summary>
	///     Interaction logic for CardControl.xaml
	/// </summary>
	public partial class CardControl
	{
		public static readonly DependencyProperty BorderStyleProperty =
			DependencyProperty.Register("BorderStyle", typeof (BorderStyle), typeof (CardControl),
				new PropertyMetadata(BorderStyle.Black, OnBorderStyleChanged));

		public static readonly DependencyProperty CardNameProperty =
			DependencyProperty.Register("CardName", typeof (string), typeof (CardControl),
				new PropertyMetadata(null, OnCardNameChanged));

		public static readonly DependencyProperty CostProperty =
			DependencyProperty.Register("Cost", typeof (string), typeof (CardControl),
				new PropertyMetadata(null, OnCostChanged));

		public static readonly DependencyProperty CardSetCodeProperty =
			DependencyProperty.Register("SetCode", typeof (string), typeof (CardControl),
				new PropertyMetadata(null, OnSetCodeChanged));

		public static readonly DependencyProperty IsMaskedProperty =
			DependencyProperty.Register("IsMasked", typeof (bool), typeof (CardControl),
				new PropertyMetadata(true, OnMaskVisibilityChanged));

		public static readonly DependencyProperty RarityProperty =
			DependencyProperty.Register("Rarity", typeof (Rarity), typeof (CardControl),
				new PropertyMetadata(Rarity.Common, OnRarityChanged));

		public static readonly DependencyProperty IsFoilProperty =
			DependencyProperty.Register("IsFoil", typeof (bool), typeof (CardControl), new PropertyMetadata(false, OnFoilChanged));

		public static readonly DependencyProperty PTProperty =
			DependencyProperty.Register("PT", typeof (string), typeof (CardControl), new PropertyMetadata(null, OnPTChanged));

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof (string), typeof (CardControl), new PropertyMetadata(null, OnTextChanged));

		public static readonly DependencyProperty FlavorProperty =
			DependencyProperty.Register("Flavor", typeof (string), typeof (CardControl),
				new PropertyMetadata(null, OnFlavorChanged));

		public static readonly DependencyProperty TypeProperty =
			DependencyProperty.Register("Type", typeof (string), typeof (CardControl), new PropertyMetadata(null, OnTypeChanged));

		public CardControl()
		{
			InitializeComponent();
		}

		public BorderStyle BorderStyle
		{
			get { return (BorderStyle) GetValue(BorderStyleProperty); }
			set { SetValue(BorderStyleProperty, value); }
		}

		public string CardName
		{
			get { return (string) GetValue(CardNameProperty); }
			set { SetValue(CardNameProperty, value); }
		}

		public string Cost
		{
			get { return (string) GetValue(CostProperty); }
			set { SetValue(CostProperty, value); }
		}

		public string SetCode
		{
			get { return (string) GetValue(CardSetCodeProperty); }
			set { SetValue(CardSetCodeProperty, value); }
		}


		public bool IsMasked
		{
			get { return (bool) GetValue(IsMaskedProperty); }
			set { SetValue(IsMaskedProperty, value); }
		}

		public Rarity Rarity
		{
			get { return (Rarity) GetValue(RarityProperty); }
			set { SetValue(RarityProperty, value); }
		}


		public bool IsFoil
		{
			get { return (bool) GetValue(IsFoilProperty); }
			set { SetValue(IsFoilProperty, value); }
		}


		public string PT
		{
			get { return (string) GetValue(PTProperty); }
			set { SetValue(PTProperty, value); }
		}


		public string Text
		{
			get { return (string) GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}


		public string Flavor
		{
			get { return (string) GetValue(FlavorProperty); }
			set { SetValue(FlavorProperty, value); }
		}


		public string Type
		{
			get { return (string) GetValue(TypeProperty); }
			set { SetValue(TypeProperty, value); }
		}

		private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CardControl source = (CardControl) d;
			string type = e.NewValue as string;
			source.TypeBox.Text = type;
		}


		private static void OnFlavorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CardControl source = (CardControl)d;
			string flavor = e.NewValue as string;
			if (!string.IsNullOrWhiteSpace(flavor))
			{
				source.TextBox.Inlines.Add(flavor);
			}
		}


		private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CardControl source = (CardControl) d;
			string text = e.NewValue as string;
			FormatTextBlock(source.TextBox, text);
		}


		private static void OnPTChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			throw new NotImplementedException();
		}


		private static void OnFoilChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CardControl source = (CardControl) d;
			if ((bool) e.NewValue)
			{
				source.FoilMask.Visibility = Visibility.Visible;
			}
			else
			{
				source.FoilMask.Visibility = Visibility.Collapsed;
			}
		}


		private static void OnRarityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private static void OnMaskVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private static void OnSetCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private static void OnCostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CardControl source = (CardControl) d;
			string cost = e.NewValue.ToString();

			Color[] colors = GetColors(cost).ToArray();
			bool isHybrid = IsHybrid(cost);

			Brush stroke = null;
			Brush fill = null;
			Brush pattern = null;

			switch (colors.Length)
			{
				case 0:
					break;
				case 1:
					switch (colors[0])
					{
						case Color.Red:
							stroke = source.FindResource("MountainBrush") as Brush;
							fill = source.FindResource("RPatternBrush") as Brush;
							pattern = source.FindResource("RPatternBrush") as Brush;
							break;
						case Color.Blue:
							stroke = source.FindResource("IslandBrush") as Brush;
							pattern = source.FindResource("UPatternBrush") as Brush;
							fill = source.FindResource("UBarBrush") as Brush;
							break;
						case Color.Green:
							stroke = source.FindResource("ForestBrush") as Brush;
							pattern = source.FindResource("GPatternBrush") as Brush;
							fill = source.FindResource("GBarBrush") as Brush;
							break;
						case Color.Black:
							stroke = source.FindResource("SwampBrush") as Brush;
							pattern = source.FindResource("BPatternBrush") as Brush;
							fill = source.FindResource("BBarBrush") as Brush;
							break;
						case Color.White:
							stroke = source.FindResource("PlainBrush") as Brush;
							pattern = source.FindResource("WPatternBrush") as Brush;
							fill = source.FindResource("WBarBrush") as Brush;
							break;
						case Color.Colorless:
							stroke = source.FindResource("ColorlessBrush") as Brush;
							pattern = source.FindResource("CPatternBrush") as Brush;
							fill = source.FindResource("CBarBrush") as Brush;
							break;
					}
					break;
				case 2:
					if (colors.Contains(Color.Black) && colors.Contains(Color.Green))
					{
						stroke = source.FindResource("SwampForestBrush") as Brush;
						if (isHybrid)
						{
							fill = source.FindResource("BGBarBrush") as Brush;
							pattern = source.FindResource("BGPatternBrush") as Brush;
						}
					}
					else if (colors.Contains(Color.Black) && colors.Contains(Color.Red))
					{
						stroke = source.FindResource("SwampMountainBrush") as Brush;
						if (isHybrid)
						{
							fill = source.FindResource("BRBarBrush") as Brush;
							pattern = source.FindResource("BRPatternBrush") as Brush;
						}
					}
					else if (colors.Contains(Color.Blue) && colors.Contains(Color.Black))
					{
						stroke = source.FindResource("IslandSwampBrush") as Brush;
						if (isHybrid)
						{
							fill = source.FindResource("UBBarBrush") as Brush;
							pattern = source.FindResource("UBPatternBrush") as Brush;
						}
					}
					else if (colors.Contains(Color.Blue) && colors.Contains(Color.Red))
					{
						stroke = source.FindResource("IslandMountainBrush") as Brush;
						if (isHybrid)
						{
							fill = source.FindResource("URBarBrush") as Brush;
							pattern = source.FindResource("URPatternBrush") as Brush;
						}
					}
					else if (colors.Contains(Color.White) && colors.Contains(Color.Blue))
					{
						stroke = source.FindResource("PlainIslandBrush") as Brush;
						if (isHybrid)
						{
							fill = source.FindResource("WUBarBrush") as Brush;
							pattern = source.FindResource("WUPatternBrush") as Brush;
						}
					}
					else if (colors.Contains(Color.White) && colors.Contains(Color.Black))
					{
						stroke = source.FindResource("PlainSwampBrush") as Brush;
						if (isHybrid)
						{
							fill = source.FindResource("WBBarBrush") as Brush;
							pattern = source.FindResource("WBPatternBrush") as Brush;
						}
					}
					else if (colors.Contains(Color.Red) && colors.Contains(Color.White))
					{
						stroke = source.FindResource("MountainPlainBrush") as Brush;
						if (isHybrid)
						{
							fill = source.FindResource("RWBarBrush") as Brush;
							pattern = source.FindResource("RWPatternBrush") as Brush;
						}
					}
					else if (colors.Contains(Color.Red) && colors.Contains(Color.Green))
					{
						stroke = source.FindResource("MountainForestBrush") as Brush;
						if (isHybrid)
						{
							fill = source.FindResource("RGBarBrush") as Brush;
							pattern = source.FindResource("RGPatternBrush") as Brush;
						}
					}
					else if (colors.Contains(Color.Green) && colors.Contains(Color.White))
					{
						stroke = source.FindResource("ForestPlainBrush") as Brush;
						if (isHybrid)
						{
							fill = source.FindResource("GWBarBrush") as Brush;
							pattern = source.FindResource("GWPatternBrush") as Brush;
						}
					}
					else if (colors.Contains(Color.Green) && colors.Contains(Color.Blue))
					{
						stroke = source.FindResource("ForestIslandBrush") as Brush;
						if (isHybrid)
						{
							fill = source.FindResource("GUBarBrush") as Brush;
							pattern = source.FindResource("GUPatternBrush") as Brush;
						}
					}
					if (!isHybrid)
					{
						fill = source.FindResource("MBarBrush") as Brush;
						pattern = source.FindResource("MPatternBrush") as Brush;
					}
					break;
				default:
					pattern = source.FindResource("MPatternBrush") as Brush;
					stroke = source.FindResource("MultiBrush") as Brush;
					fill = source.FindResource("MBarBrush") as Brush;
					break;
			}

			source.TopBar.Stroke = stroke;
			source.TopBox.Stroke = stroke;
			source.BottomBar.Stroke = stroke;
			source.BottomBox.Stroke = stroke;
			source.TopBar.Fill = fill;
			source.TopBox.Fill = fill;
			source.BottomBar.Fill = fill;
			source.BottomBox.Fill = fill;
			source.Pattern.Fill = pattern;

			FormatTextBlock(source.CostBox, cost);
		}


		private static void OnCardNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CardControl source = (CardControl) d;
			string name = e.NewValue as string;
			source.NameBox.Text = name;
		}

		private static void OnBorderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CardControl source = (CardControl) d;

			switch ((BorderStyle) e.NewValue)
			{
				case BorderStyle.Black:
					source.OutterBorder.Fill = Brushes.Black;
					source.OutterBorder.Stroke = Brushes.White;
					break;
				case BorderStyle.White:
					source.OutterBorder.Fill = Brushes.White;
					source.OutterBorder.Stroke = Brushes.Black;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static IEnumerable<Color> GetColors(string cost)
		{
			if (string.IsNullOrWhiteSpace(cost) || !Regex.IsMatch(cost, @"W|B|U|R|G"))
			{
				yield return Color.Colorless;
				yield break;
			}

			if (cost.Contains("W"))
				yield return Color.White;
			if (cost.Contains("B"))
				yield return Color.Black;
			if (cost.Contains("U"))
				yield return Color.Blue;
			if (cost.Contains("R"))
				yield return Color.Red;
			if (cost.Contains("G"))
				yield return Color.Green;
		}

		private static bool IsHybrid(string cost)
		{
			return cost != null && Regex.IsMatch(cost, @"{\D{2}}", RegexOptions.IgnoreCase);
		}

		private static IEnumerable<string> ManaSplit(string input)
		{
			return Regex.Split(input, @"(?<=})|(?!^)(?={)");
		}

		private static void FormatTextBlock(TextBlock block, string text)
		{
			if (block == null)
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}

			IEnumerable<string> result = ManaSplit(text);
			block.Inlines.Clear();
			foreach (string str in result)
			{
				if (Regex.IsMatch(str, "[{.+}]"))
				{
					Style style = block.TryFindResource("Mana" + Regex.Replace(str, "{|}", "").ToUpper()) as Style;
					if (style != null)
					{
						UserControl control = new UserControl
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

	/// <summary>
	///     Card border style
	/// </summary>
	public enum BorderStyle
	{
		Black,
		White
	}

	public enum Rarity
	{
		Common,
		Uncommon,
		Rare,
		Mythic
	}

	public enum Color
	{
		Red,
		Blue,
		Green,
		Black,
		White,
		Colorless
	}
}