using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace TF2_FastDL
{
	/// <summary>
	/// Description of LocalizationManager.
	/// </summary>
	internal static class LocalizationManager
	{
		private static Assembly assembly = Assembly.GetExecutingAssembly();
		private static string currentCulture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
		private const string fileName = "strings";
		private static bool initialized = false;
		private static Regex regexStrings = new Regex("(?<name>\\S+?)\\s*?=\\s?(?<value>.+)");

		public static void Initialize()
		{
			if (!initialized)
			{
				string[] resources = assembly.GetManifestResourceNames();
				bool success = false;
				bool successEn = false;
				for (int i = 0; i < resources.Length; i++)
				{
					if (resources[i].Contains(fileName + "_" + currentCulture + ".txt"))
					{
						success = true;
					}
					else if (resources[i].Contains(fileName + "_en.txt"))
					{
						successEn = true;
					}
					if (success && successEn)
					{
						break;
					}
				}
				// This should never happen.. if so this is critical
				if (!successEn)
				{
					throw new MissingManifestResourceException();
				}
				if (!success)
				{
					currentCulture = "en";
				}
				initialized = true;
			}
		}

		public static string GetLocalizedString(string name)
		{
			return GetLocalizedString(name, currentCulture);
		}
		
		public static string GetLocalizedString(string name, string locale)
		{
			if (!initialized)
			{
				Initialize();
			}
			if (name == null || locale == null)
			{
				throw new ArgumentNullException((name == null) ? "name" : "locale");
			}

			string result = null;

			try
			{
				using (StreamReader sr = new StreamReader(assembly.GetManifestResourceStream(typeof(LocalizationManager).Namespace + "." + fileName + "_" + locale + ".txt")))
				{
					while (!sr.EndOfStream)
					{
						string text = sr.ReadLine();
						// One line comments will be ignored
						if (text.StartsWith("#") || text.StartsWith(";") || text.StartsWith("//"))
						{
							continue;
						}
						Match match = regexStrings.Match(text);
						if (match.Groups["name"].Value == name)
						{
							result = match.Groups["value"].Value;
						}
					}
				}
			}
			catch (FileNotFoundException ex)
			{
				if (locale != "en")
				{
					return GetLocalizedString(name, "en");
				}
				throw ex;
			}

			if (result == null && locale != "en")
			{
				result = GetLocalizedString(name, "en");
			}

			return result;
		}

		public static string FormatLocalizedString(string name, params object[] args)
		{
			return String.Format(GetLocalizedString(name), args);
		}
	}
}
