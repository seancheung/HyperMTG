using System.Windows;

namespace HyperMTG.Cultures
{
	public class CultureManager
	{
		public static readonly CultureManager Instance = new CultureManager();

		private CultureManager()
		{
		}

		public ResourceDictionary Dictionary { get; private set; }
	}
}