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
using HyperKore.Logger;
using HyperKore.Utilities;
using HyperMTG.Properties;

namespace HyperMTG.Langs
{
	public class LanguageManager
	{
		private const string LangPrefix = "lang";

		private const string LangPostfix = "xaml";

		private static bool isFound;
		private static readonly List<Language> languages = new List<Language>();

		private static ResourceDictionary defaultdict;

		private static string LangFolder
		{
			get
			{
				if (!Directory.Exists("Langs"))
				{
					Directory.CreateDirectory("Langs");
				}
				return "Langs";
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

		public static void Initial()
		{
			defaultdict = Application.Current.Resources.MergedDictionaries[0];
			languages.Add(Language.English);

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
					catch (ArgumentException ex)
					{
						Logger.Log(ex, typeof(LanguageManager), source);
					}
				}

				if (Languages.Count > 0)
				{
					ChangeLang(Settings.Default.Language);
				}

				isFound = true;
			}
		}

		private static void ChangeLang(Language lang)
		{
			if (Languages.Contains(lang))
			{
				if (lang == Language.English)
				{
					Application.Current.Resources.MergedDictionaries[0] = defaultdict;
				}
				else
				{
					string filePath = string.Format("{0}\\{1}.{2}.{3}", LangFolder, LangPrefix, lang.GetLangCode(),
						LangPostfix);

					try
					{
						using (var fs = new FileStream(filePath, FileMode.Open))
						{
							var dict = XamlReader.Load(fs) as ResourceDictionary;
							Application.Current.Resources.MergedDictionaries[0] = dict;
						}
					}
					catch (Exception ex)
					{
						Logger.Log(ex, typeof(LanguageManager), lang);
						throw;
					}
				}

				Settings.Default.Language = lang;
				Settings.Default.Save();
			}
		}
	}
}