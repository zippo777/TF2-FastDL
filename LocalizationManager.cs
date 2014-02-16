/*
 * Copyright (c) 2014 Oliver Schramm
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
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
