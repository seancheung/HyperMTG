using System;
using System.Collections;
using System.Linq;
using System.Windows.Input;
using HyperKore.Common;
using HyperMTG.Helper;
using HyperMTG.Model;

namespace HyperMTG.ViewModel
{
	internal class FilterViewModel
	{
		/// <summary>
		///     Initializes a new instance of the FilterViewModel class.
		/// </summary>
		public FilterViewModel()
		{
			Cost = 0;
			Power = 0;
			Rating = 0f;
			Toughness = 0;
			Types = GetFromEnum<TYPE>();
			Colors = GetFromEnum<COLOR>();
			Rarities = GetFromEnum<RARITY>();
		}

		public int Cost { get; set; }

		public int Power { get; set; }

		public float Rating { get; set; }

		public int Toughness { get; set; }

		public IEnumerable Rarities { get; set; }

		public IEnumerable Colors { get; set; }

		public IEnumerable Types { get; set; }

		public IEnumerable Formats
		{
			get { return null; }
		}

		public IEnumerable Sets
		{
			get { return null; }
		}

		public ICommand CheckType
		{
			get { return new RelayCommand<string>(CheckTypeExecute); }
		}

		public ICommand CheckColor
		{
			get { return new RelayCommand<string>(CheckColorExecute); }
		}

		public ICommand CheckRarity
		{
			get { return new RelayCommand<string>(CheckRarityExecute); }
		}

		private static IEnumerable GetFromEnum<T>()
		{
			return Enum.GetNames(typeof (T)).Select(item => new CheckItem(item, false)).ToList();
		}

		private void CheckTypeExecute(object parameter)
		{
			foreach (CheckItem type in Types)
			{
				switch (parameter.ToString())
				{
					case "0":
						type.IsChecked = false;
						break;

					case "1":
						type.IsChecked = true;
						break;

					case "2":
						type.IsChecked = null;
						break;
				}
			}
		}

		private void CheckColorExecute(object parameter)
		{
			foreach (CheckItem color in Colors)
			{
				switch (parameter.ToString())
				{
					case "0":
						color.IsChecked = false;
						break;

					case "1":
						color.IsChecked = true;
						break;

					case "2":
						color.IsChecked = null;
						break;
				}
			}
		}

		private void CheckRarityExecute(object parameter)
		{
			foreach (CheckItem rarity in Rarities)
			{
				switch (parameter.ToString())
				{
					case "0":
						rarity.IsChecked = false;
						break;

					case "1":
						rarity.IsChecked = true;
						break;

					case "2":
						rarity.IsChecked = null;
						break;
				}
			}
		}
	}
}