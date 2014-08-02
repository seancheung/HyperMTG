// http://www.silverlightchina.net/html/study/WPF/2011/0729/9348.html

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;
using HyperMTG.Properties;

namespace HyperMTG.Cultures
{
	public class CultureManager
	{
		private const string CulturePrefix = "lang";

		private const string CulturePostfix = "xaml";

		private const string CultureFolder = "Cultures";

		private static bool isFound;

		public static bool IsInDesignMode
		{
			get
			{
				return (bool)DesignerProperties.IsInDesignModeProperty
							.GetMetadata(typeof(DependencyObject)).DefaultValue;
			}
		}

		public CultureManager()
		{
			if (!isFound && !IsInDesignMode)
			{
				string pattern = string.Format(@"(?<={0}\.)[a-z]{{2}}(?=\.{1})", CulturePrefix, CulturePostfix);

				foreach (string source in Directory.GetFiles(CultureFolder).Where(f => Regex.IsMatch(f, pattern)))
				{
					try
					{
						string isoCode = Regex.Match(source, pattern).Value;
						CultureInfo cultureInfo = CultureInfo.GetCultureInfoByIetfLanguageTag(isoCode);
						if (cultureInfo != null)
						{
							CultureInfos.Add(cultureInfo);
						}
					}
					catch (ArgumentException)
					{
					}
				}

				if (CultureInfos.Count > 0 && Settings.Default.DefaultCulture != null)
				{
					ChangeCulture(Settings.Default.DefaultCulture);
				}

				isFound = true;
			}
		}

		public static List<CultureInfo> CultureInfos { get; private set; }


		public static void ChangeCulture(CultureInfo culture)
		{
			if (CultureInfos.Contains(culture))
			{
				string filePath = string.Format("{0}\\{1}.{2}.{3}", CultureFolder, CulturePrefix, culture.TwoLetterISOLanguageName,
					CulturePostfix);

				using (FileStream fs = new FileStream(filePath, FileMode.Open))
				{
					ResourceDictionary dict = XamlReader.Load(fs) as ResourceDictionary;

					Application.Current.Resources.MergedDictionaries[0] = dict;
					

					Settings.Default.DefaultCulture = culture;
					Settings.Default.Save();
				
				}
			}
		}
	}
}