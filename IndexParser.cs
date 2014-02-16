using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TF2_FastDL
{
	/// <summary>
	/// Description of IndexParser.
	/// </summary>
	internal static class IndexParser
	{
		// Format in which the date of last modification is provided on the index page
		//private const string LAST_MODIFIED_FORMAT = "dd-MMM-yyyy HH:mm";
		// Regular expressions can be very complex - I try to explain them:
		// Finds the directory name, we're currently in
		private static Regex regexMainDir = new Regex("<title>Index of (?<maindir>.+?)</title>", RegexOptions.IgnoreCase);
		// Finds all sub-directories their last modified time
		//private static Regex regexDirs = new Regex(".+?<a.*?href=\"(?<directory>.+?/)\">\\k<directory></a>\\s+?(?<modified>\\d{2}-\\w{3}-\\d{4}\\s\\d{2}:\\d{2})", RegexOptions.IgnoreCase);
		private static Regex regexDirs = new Regex(".+?<a.*?href=\"(?<directory>.+?/)\">\\k<directory></a>", RegexOptions.IgnoreCase);
		// Finds all files and their last modified time
		//private static Regex regexFiles = new Regex(".+?<a.+?href=\"(?<file>[^/]+?)\">\\k<file></a>\\s+?(?<modified>\\d{2}-\\w{3}-\\d{4}\\s\\d{2}:\\d{2})\\s+?(?<size>\\d+(\\.\\d+)?[A-Z]?)", RegexOptions.IgnoreCase);
		//private static Regex regexFiles = new Regex(".+?<a.+?href=\"(?<file>[^/]+?)\">\\k<file></a>\\s+?(?<modified>\\d{2}-[A-Za-z]{3}-\\d{4}\\s\\d{2}:\\d{2})", RegexOptions.IgnoreCase);
		private static Regex regexFiles = new Regex(".+?<a.+?href=\"(?<file>[^/]+?)\">\\k<file></a>", RegexOptions.IgnoreCase);

		public static string[][] Parse(string text)
		{
			if (!regexMainDir.IsMatch(text) || (!regexDirs.IsMatch(text) && !regexFiles.IsMatch(text)))
			{
				return null;
			}

			string mainDir = regexMainDir.Match(text).Groups["maindir"].Value;
			mainDir = Regex.Replace(mainDir, "/{2,}", "/");

			List<string> directories = new List<string>();
			//List<DateTime> dirLastModified = new List<DateTime>();
			if (regexDirs.IsMatch(text))
			{
				MatchCollection matchDirs = regexDirs.Matches(text);
				for (int i = 0; i < matchDirs.Count; i++)
				{
					directories.Add(matchDirs[i].Groups["directory"].Value);
					//dirLastModified.Add(DateTime.ParseExact(matchDirs[i].Groups["modified"].Value, LAST_MODIFIED_FORMAT, CultureInfo.GetCultureInfoByIetfLanguageTag("en-US")));
				}
			}

			List<string> files = new List<string>();
			//List<DateTime> filesLastModified = new List<DateTime>();
			if (regexFiles.IsMatch(text))
			{
				MatchCollection matchFiles = regexFiles.Matches(text);
				for (int i = 0; i < matchFiles.Count; i++)
				{
					files.Add(matchFiles[i].Groups["file"].Value);
					//filesLastModified.Add(DateTime.ParseExact(matchFiles[i].Groups["modified"].Value, LAST_MODIFIED_FORMAT, CultureInfo.GetCultureInfoByIetfLanguageTag("en-US")));
				}
			}

			string[][] result = new string[3][];
			result[0] = new string[] { mainDir };
			result[1] = directories.ToArray();
			result[2] = files.ToArray();
			/*result[2] = new string[dirLastModified.Count];
			result[3] = files.ToArray();
			result[4] = new string[filesLastModified.Count];*/
			/*for (int i = 0; i < dirLastModified.Count; i++)
			{
				result[2][i] = dirLastModified[i].Ticks.ToString();
			}
			for (int i = 0; i < filesLastModified.Count; i++)
			{
				result[4][i] = filesLastModified[i].Ticks.ToString();
			}*/

			return result;
		}
	}
}
