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
		private static List<Language> languages = new List<Language>();

		public static void Initial()
		{
			if (!isFound && !IsInDesignMode)
			{
				string pattern = string.Format(@"(?<={0}\.)[a-z]{{2}}(?=\.{1})", LangPrefix, LangPostfix);

				foreach (string source in Directory.GetFiles(LangFolder).Where(f => Regex.IsMatch(f, pattern)))
				{
					try
					{
						string isoCode = Regex.Match(source, pattern).Value;
						Language lang = LanguageTool.GetLangugeByCode(isoCode);
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

		public int CurrentIndex
		{
			get { return Languages.IndexOf(Settings.Default.Language); }
			set { ChangeLang(Languages[value]); }
		}

		public static bool IsInDesignMode
		{
			get
			{
				return (bool) DesignerProperties.IsInDesignModeProperty
					.GetMetadata(typeof (DependencyObject)).DefaultValue;
			}
		}

		public static List<Language> Languages
		{
			get { return languages; }
		}

		public object this[string key]
		{
			get { return Application.Current.Resources.MergedDictionaries[0][key]; }
		}
		
		private static void ChangeLang(Language lang)
		{
			if (Languages.Contains(lang))
			{
				string filePath = string.Format("{0}\\{1}.{2}.{3}", LangFolder, LangPrefix, lang.GetLangCode(),
					LangPostfix);

				using (FileStream fs = new FileStream(filePath, FileMode.Open))
				{
					ResourceDictionary dict = XamlReader.Load(fs) as ResourceDictionary;

					Application.Current.Resources.MergedDictionaries[0] = dict;

					Settings.Default.Language = lang;
					Settings.Default.Save();
				}
			}
		}
	}
}