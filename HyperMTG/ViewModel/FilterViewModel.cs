using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HyperKore.Common;
using HyperMTG.Helper;
using HyperMTG.Model;
using HyperMTG.Properties;
using HyperPlugin;
using Type = HyperKore.Common.Type;

namespace HyperMTG.ViewModel
{
	internal class FilterViewModel
	{
		private static readonly FilterViewModel _instance = new FilterViewModel();
		private readonly IDBReader _dbReader;

		/// <summary>
		///     Initializes a new instance of the FilterViewModel class.
		/// </summary>
		private FilterViewModel()
		{
			_dbReader = PluginManager.Instance.GetPlugin<IDBReader>();
			if (_dbReader != null)
			{
				_dbReader.Language = Settings.Default.Language;
			}

			Cost = 0;
			Power = 0;
			Rating = 0f;
			Toughness = 0;
			Types = GetFromEnum<Type>();
			Colors = GetFromEnum<Color>();
			Rarities = GetFromEnum<Rarity>();
			Sets = LoadSets().ToArray();
		}

		public static FilterViewModel Instance
		{
			get { return _instance; }
		}

		public int Cost { get; set; }

		public int Power { get; set; }

		public float Rating { get; set; }

		public int Toughness { get; set; }

		public IEnumerable<CheckItem<Rarity>> Rarities { get; set; }

		public IEnumerable<CheckItem<Color>> Colors { get; set; }

		public IEnumerable<CheckItem<Type>> Types { get; set; }

		public IEnumerable Formats
		{
			get { return null; }
		}

		public IEnumerable<CheckItem<Set>> Sets { get; set; }

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

		private IEnumerable<CheckItem<Set>> LoadSets()
		{
			if (_dbReader != null)
			{
				foreach (Set set in _dbReader.LoadSets().Where(s => s.Local))
				{
					yield return new CheckItem<Set>(set, false);
				}
			}
		}

		private static IEnumerable<CheckItem<T>> GetFromEnum<T>()
		{
			return Enum.GetNames(typeof (T)).Select(item => new CheckItem<T>((T) Enum.Parse(typeof (T), item), false)).ToList();
		}

		private void CheckTypeExecute(object parameter)
		{
			foreach (var type in Types)
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
			foreach (var color in Colors)
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
			foreach (var rarity in Rarities)
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