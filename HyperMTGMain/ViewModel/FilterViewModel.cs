using System;
using System.Collections.Generic;
using System.Linq;
using HyperKore.Common;
using HyperMTGMain.Helper;
using HyperMTGMain.Model;
using Type = HyperKore.Common.Type;

namespace HyperMTGMain.ViewModel
{
	public class FilterViewModel
	{
		private static FilterViewModel _instance;
		private List<CheckItem<Color>> _colors;
		private List<CheckItem<Rarity>> _rarities;
		private List<CheckItem<Set>> _sets;
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

		public IEnumerable<Card> FilterCards(IEnumerable<Card> cards)
		{
			throw new NotImplementedException();
		} 
	}
}