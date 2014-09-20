using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using HyperKore.Common;
using HyperKore.Utilities;
using HyperMTGMain.Helper;
using HyperMTGMain.Model;
using HyperMTGMain.View;
using Type = HyperKore.Common.Type;

namespace HyperMTGMain.ViewModel
{
	public class FilterViewModel : ObservableClass
	{
		private static FilterViewModel _instance;
		private List<CheckItem<Color>> _colors;
		private bool _includeUnselectedColors;
		private bool _includeUnselectedTypes;
		private string _nameKeyword;
		private List<CheckItem<Rarity>> _rarities;
		private List<CheckItem<Set>> _sets;
		private string _textKeyword;
		private string _typeKeyword;
		private List<CheckItem<Type>> _types;

		private FilterViewModel()
		{
		}

		internal static FilterViewModel Instance
		{
			get { return _instance ?? (_instance = new FilterViewModel()); }
		}

		public List<CheckItem<Rarity>> Rarities
		{
			get { return _rarities ?? (_rarities = GetFromEnum<Rarity>()); }
		}

		public List<CheckItem<Color>> Colors
		{
			get { return _colors ?? (_colors = GetFromEnum<Color>()); }
		}

		public List<CheckItem<Type>> Types
		{
			get { return _types ?? (_types = GetFromEnum<Type>()); }
		}

		public List<CheckItem<Set>> Sets
		{
			get { return _sets ?? (_sets = LoadSets().ToList()); }
		}

		public bool IncludeUnselectedTypes
		{
			get { return _includeUnselectedTypes; }
			set
			{
				_includeUnselectedTypes = value;
				OnPropertyChanged("IncludeUnselectedTypes");
			}
		}

		public bool IncludeUnselectedColors
		{
			get { return _includeUnselectedColors; }
			set
			{
				_includeUnselectedColors = value;
				OnPropertyChanged("IncludeUnselectedColors");
			}
		}

		public string NameKeyword
		{
			get { return _nameKeyword; }
			set
			{
				_nameKeyword = value;
				OnPropertyChanged("NameKeyword");
			}
		}

		public string TypeKeyword
		{
			get { return _typeKeyword; }
			set
			{
				_typeKeyword = value;
				OnPropertyChanged("TypeKeyword");
			}
		}

		public string TextKeyword
		{
			get { return _textKeyword; }
			set
			{
				_textKeyword = value;
				OnPropertyChanged("TextKeyword");
			}
		}

		public ICommand CheckSetCommand
		{
			get { return new RelayCommand<string>(CheckSet); }
		}

		public ICommand CheckTypeCommand
		{
			get { return new RelayCommand<string>(CheckType); }
		}

		public ICommand CheckColorCommand
		{
			get { return new RelayCommand<string>(CheckColor); }
		}

		public ICommand OkCommand
		{
			get { return new RelayCommand(FilterData, CanFilterData); }
		}

		public ICommand ResetCommand
		{
			get { return new RelayCommand(ResetData, CanResetData); }
		}

		public ICommand ClearCommand
		{
			get { return new RelayCommand(Clear); }
		}

		private void CheckType(object parameter)
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

		private void CheckColor(object parameter)
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

		private void CheckSet(object parameter)
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

		private static List<CheckItem<T>> GetFromEnum<T>()
		{
			return Enum.GetNames(typeof (T)).Select(item => new CheckItem<T>((T) Enum.Parse(typeof (T), item), true)).ToList();
		}

		private IEnumerable<CheckItem<Set>> LoadSets()
		{
			if (PluginFactory.ComponentsAvailable)
			{
				foreach (Set set in PluginFactory.DbReader.LoadSets().Where(s => s.Local))
				{
					yield return new CheckItem<Set>(set, true);
				}
			}
		}

		private void FilterData()
		{
			ViewManager.FilterWindow.Close();
			ViewModelManager.DeckEditorViewModel.Cards = FilterCards().ToList();
		}

		private bool CanFilterData()
		{
			return PluginFactory.ComponentsAvailable;
		}

		private void ResetData()
		{
			_instance = null;
			ViewManager.FilterWindow.Close();
			ViewManager.FilterWindow.ShowDialog();
		}

		private bool CanResetData()
		{
			return PluginFactory.ComponentsAvailable;
		}

		private void Clear()
		{
			NameKeyword = null;
			TypeKeyword = null;
			TextKeyword = null;
		}

		private IEnumerable<Card> FilterCards()
		{
			IQueryable<Card> result = PluginFactory.DbReader.LoadCards()
				.AsQueryable()
				.WhereIn(c => c.SetCode, Sets.Where(s => s.IsChecked).Select(p => p.Content.SetCode));
			result = result.WhereIn(c => c.GetRarity(), Rarities.Where(r => r.IsChecked).Select(p => p.Content));
			result = IncludeUnselectedColors
				? result.Where(c => c.GetColors().Any(Colors.Where(k => k.IsChecked).Select(p => p.Content).Contains))
				: result.Where(c => c.GetColors().All(Colors.Where(k => k.IsChecked).Select(p => p.Content).Contains));
			result = IncludeUnselectedTypes
				? result.Where(c => c.GetTypes().Any(Types.Where(t => t.IsChecked).Select(p => p.Content).Contains))
				: result.Where(c => c.GetTypes().All(Types.Where(t => t.IsChecked).Select(p => p.Content).Contains));

			if (!string.IsNullOrWhiteSpace(NameKeyword))
			{
				result = result.Where(c => Regex.IsMatch(c.Name, NameKeyword, RegexOptions.IgnoreCase));
			}
			if (!string.IsNullOrWhiteSpace(TypeKeyword))
			{
				result = result.Where(c => Regex.IsMatch(c.Type, TypeKeyword, RegexOptions.IgnoreCase));
			}
			if (!string.IsNullOrWhiteSpace(TextKeyword))
			{
				result = result.Where(c => Regex.IsMatch(c.Text, TextKeyword, RegexOptions.IgnoreCase));
			}

			return result;
		}
	}
}