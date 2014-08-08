// http://www.silverlightchina.net/html/study/WPF/2011/0729/9348.html

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;
using HyperKore.Common;
using HyperKore.Utilities;
using HyperMTG.Properties;

namespace HyperMTG.Langs
{
	public class LanguageManager
	{
		private const string LangPrefix = "lang";

		private const string LangPostfix = "xaml";

		private const string LangFolder = "Langs";

		private static bool isFound;

		public LanguageManager()
		{
			Languages = new List<LANGUAGE>();

			if (!isFound && !IsInDesignMode)
			{
				string pattern = string.Format(@"(?<={0}\.)[a-z]{{2}}(?=\.{1})", LangPrefix, LangPostfix);

				foreach (string source in Directory.GetFiles(LangFolder).Where(f => Regex.IsMatch(f, pattern)))
				{
					try
					{
						string isoCode = Regex.Match(source, pattern).Value;
						LANGUAGE lang = LanguageTool.GetLangugeByCode(isoCode);
						Languages.Add(lang);
					}
					catch (ArgumentException)
					{
					}
				}

				if (Languages.Count > 0)
				{
					ChangeLang(Settings.Default.Language);
				}

				isFound = true;
			}
		}

		public static bool IsInDesignMode
		{
			get
			{
				return (bool) DesignerProperties.IsInDesignModeProperty
					.GetMetadata(typeof (DependencyObject)).DefaultValue;
			}
		}

		public static List<LANGUAGE> Languages { get; private set; }


		public static void ChangeLang(LANGUAGE lang)
		{
			if (Languages.Contains(lang))
			{
				string filePath = string.Format("{0}\\{1}.{2}.{3}", LangFolder, LangPrefix, lang.GetLangCode(),
					LangPostfix);

				using (var fs = new FileStream(filePath, FileMode.Open))
				{
					var dict = XamlReader.Load(fs) as ResourceDictionary;

					Application.Current.Resources.MergedDictionaries[0] = dict;


					Settings.Default.Language = lang;
					Settings.Default.Save();
				}
			}
		}
	}
}