using System;
using System.Windows;
using System.Windows.Media;

namespace HyperStyle.Controls
{
	/// <summary>
	///     Interaction logic for CardControl.xaml
	/// </summary>
	public partial class CardControl
	{
		public static readonly DependencyProperty BorderStyleProperty =
			DependencyProperty.Register("BorderStyle", typeof(BorderStyle), typeof(CardControl),
				new PropertyMetadata(BorderStyle.Black, OnBorderStyleChanged));

		public static readonly DependencyProperty IsHybridProperty =
			DependencyProperty.Register("IsHybrid", typeof(bool), typeof(CardControl), new PropertyMetadata(false));

		public static readonly DependencyProperty CardNameProperty =
			DependencyProperty.Register("CardName", typeof(string), typeof(CardControl),
				new PropertyMetadata(null, OnCardNameChanged));

		public static readonly DependencyProperty CostProperty =
			DependencyProperty.Register("Cost", typeof(string), typeof(CardControl), new PropertyMetadata(null, OnCostChanged));

		public static readonly DependencyProperty CardSetCodeProperty =
			DependencyProperty.Register("CardSetCode", typeof(string), typeof(CardControl),
				new PropertyMetadata(null, OnSetCodeChanged));

		public CardControl()
		{
			InitializeComponent();
		}

		public BorderStyle BorderStyle
		{
			get { return (BorderStyle)GetValue(BorderStyleProperty); }
			set { SetValue(BorderStyleProperty, value); }
		}

		public bool IsHybrid
		{
			get { return (bool)GetValue(IsHybridProperty); }
			set { SetValue(IsHybridProperty, value); }
		}

		public string CardName
		{
			get { return (string)GetValue(CardNameProperty); }
			set { SetValue(CardNameProperty, value); }
		}

		public string CardCost
		{
			get { return (string)GetValue(CostProperty); }
			set { SetValue(CostProperty, value); }
		}

		public string CardSetCode
		{
			get { return (string)GetValue(CardSetCodeProperty); }
			set { SetValue(CardSetCodeProperty, value); }
		}


		public bool IsMasked
		{
			get { return (bool)GetValue(IsMaskedProperty); }
			set { SetValue(IsMaskedProperty, value); }
		}

		public static readonly DependencyProperty IsMaskedProperty =
			DependencyProperty.Register("IsMasked", typeof(bool), typeof(CardControl),
				new PropertyMetadata(true, OnMaskVisibilityChanged));

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
			throw new NotImplementedException();
		}


		private static void OnCardNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private static void OnBorderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var source = (CardControl)d;

			switch ((BorderStyle)e.NewValue)
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
	}

	/// <summary>
	///     Card border style
	/// </summary>
	public enum BorderStyle
	{
		Black,
		White
	}

	public enum CardColor
	{
		Red,
		Blue,
		Green,
		Black,
		White,
		Colorless,
		BlackGreen,
		BlackRed,
		GreenWhite,
		GreenBlue,
		BlueBlack,
		BlueRed,
		RedGreen,
		RedWhite,
		WhiteBlack,
		WhiteBlue,
		MultiColor
	}

	public enum CardRarity
	{
		Common,
		Uncommon,
		Rare,
		Mythic
	}

	public enum CardType
	{
		Artifact,
		Equipment,
		Basic,
		Conspiracy,
		Creature,
		Enchantment,
		Aura,
		Instant,
		Land,
		Legendary,
		Ongoing,
		Phenomenon,
		Plane,
		Planeswalker,
		Scheme,
		Snow,
		Sorcery,
		Tribal,
		Vanguard,
		World
	}
}