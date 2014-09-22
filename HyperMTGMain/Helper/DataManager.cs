using System.Collections.ObjectModel;
using System.Linq;
using HyperKore.Common;

namespace HyperMTGMain.Helper
{
	public static class DataManager
	{
		private static ReadOnlyCollection<Card> _cards;
		private static ReadOnlyCollection<Set> _sets;

		public static ReadOnlyCollection<Card> Cards
		{
			get { return _cards ?? (_cards = PluginFactory.DbReader.LoadCards().ToList().AsReadOnly()); }
		}

		public static ReadOnlyCollection<Set> Sets
		{
			get { return _sets ?? (_sets = PluginFactory.DbReader.LoadSets().ToList().AsReadOnly()); }
		}
	}
}