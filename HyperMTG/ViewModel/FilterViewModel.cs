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
	internal class FilterViewModel : ObservableObject
	{
		private static readonly FilterViewModel _instance = new FilterViewModel();
		private readonly IDBReader _dbReader;
		private bool _includeUnselectedColors;
		private bool _includeUnselectedTypes;
		private int _cost;
		private bool _cmcEqualTo;
		private bool _cmcNoLessThan;

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
			Sets = LoadSets().ToList();
			IncludeUnselectedTypes = true;
			IncludeUnselectedColors = true;
			CMCNoLessThan = true;
		}

		public bool IncludeUnselectedTypes
		{
			get { return _includeUnselectedTypes; }
			set
			{
				_includeUnselectedTypes = value;
				RaisePropertyChanged("IncludeUnselectedTypes");
			}
		}

		public bool IncludeUnselectedColors
		{
			get { return _includeUnselectedColors; }
			set
			{
				_includeUnselectedColors = value;
				RaisePropertyChanged("IncludeUnselectedColors");
			}
		}

		public static FilterViewModel Instance
		{
			get { return _instance; }
		}

		public int Cost
		{
			get { return _cost; }
			set
			{
				_cost = value;
				RaisePropertyChanged("Cost");
			}
		}

		public CMCCompareType CMCCondition
		{
			get
			{
				if (CMCEqualTo)
				{
					return CMCCompareType.EqualTo;
				}
				if (CMCNoLessThan)
				{
					return CMCCompareType.NoLessThan;
				}
				return CMCCompareType.NoMoreThan;
			}
		}

		public bool CMCEqualTo
		{
			get { return _cmcEqualTo; }
			set
			{
				_cmcEqualTo = value;
				RaisePropertyChanged("CMCEqualTo");
			}
		}

		public bool CMCNoLessThan
		{
			get { return _cmcNoLessThan; }
			set
			{
				_cmcNoLessThan = value;
				RaisePropertyChanged("CMCNoLessThan");
			}
		}

		public int Power { get; set; }

		public float Rating { get; set; }

		public int Toughness { get; set; }

		public List<CheckItem<Rarity>> Rarities { get; private set; }

		public List<CheckItem<Color>> Colors { get; private set; }

		public List<CheckItem<Type>> Types { get; private set; }

		public IEnumerable Formats
		{
			get { return null; }
		}

		public List<CheckItem<Set>> Sets { get; private set; }

		public ICommand CheckSet
		{
			get { return new RelayCommand<string>(CheckSetExecute); }
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

		private IEnumerable<CheckItem<Set>> LoadSets()
		{
			if (_dbReader != null)
			{
				foreach (Set set in _dbReader.LoadSets().Where(s => s.Local))
				{
					yield return new CheckItem<Set>(set, true);
				}
			}
		}

		private static List<CheckItem<T>> GetFromEnum<T>()
		{
			return Enum.GetNames(typeof (T)).Select(item => new CheckItem<T>((T) Enum.Parse(typeof (T), item), true)).ToList();
		}

		private void CheckTypeExecute(object parameter)
		{
			switch (parameter.ToString())
			{
				case "0":
					Types.ForEach(t => t.IsChecked = true);
					break;

				case "1":
					Types.ForEach(t => t.IsChecked = false);
					break;

				case "2":
					Types.ForEach(t => t.IsChecked = !t.IsChecked);
					break;
			}
		}

		private void CheckColorExecute(object parameter)
		{
			switch (parameter.ToString())
			{
				case "0":
					Colors.ForEach(c => c.IsChecked = true);
					break;

				case "1":
					Colors.ForEach(c => c.IsChecked = false);
					break;

				case "2":
					Colors.ForEach(c => c.IsChecked = !c.IsChecked);
					break;
			}
		}

		private void CheckRarityExecute(object parameter)
		{
			switch (parameter.ToString())
			{
				case "0":
					Rarities.ForEach(r => r.IsChecked = true);
					break;

				case "1":
					Rarities.ForEach(r => r.IsChecked = false);
					break;

				case "2":
					Rarities.ForEach(r => r.IsChecked = !r.IsChecked);
					break;
			}
		}

		private void CheckSetExecute(object parameter)
		{
			switch (parameter.ToString())
			{
				case "0":
					Sets.ForEach(s => s.IsChecked = true);
					break;

				case "1":
					Sets.ForEach(s => s.IsChecked = false);
					break;

				case "2":
					Sets.ForEach(s => s.IsChecked = !s.IsChecked);
					break;
			}
		}

		public enum CMCCompareType
		{
			EqualTo,
			NoLessThan,
			NoMoreThan
		}
	}
}