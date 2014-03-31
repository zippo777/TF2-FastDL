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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TF2_FastDL
{
	/// <summary>
	/// Description of IndexParser.
	/// </summary>
	public static class IndexParser
	{
		// Format in which the date of last modification is provided on the index page
		// Regular expressions can be very complex - I try to explain them:
		// Finds the directory name, we're currently in
		private static Regex regexMainDir = new Regex("<title>Index of (?<maindir>.+?)</title>", RegexOptions.IgnoreCase);
		// Finds all sub-directories
		private static Regex regexDirs = new Regex(".+?<a.*?href=\"(?<directory>.+?/)\".*?>\\k<directory></a>", RegexOptions.IgnoreCase);
		// Finds all files
		//private static Regex regexFiles = new Regex(".+?<a.+?href=\"(?<file>[^/]+?)\".*?>\\k<file></a>\\s+?(?<modified>\\d{2}-\\w{3}-\\d{4}\\s\\d{2}:\\d{2})\\s+?(?<size>\\d+(\\.\\d+)?[A-Z]?)", RegexOptions.IgnoreCase);
		private static Regex regexFiles = new Regex(".+?<a.+?href=\"(?<file>[^/]+?)\".*?>\\k<file></a>", RegexOptions.IgnoreCase);

		/// <summary>
		/// Parsed the directory listing page.
		/// </summary>
		/// <param name="text">The listing page as a text.</param>
		/// <returns>Returns a jagged array containg at [0][0] the index, at [1] the folders as a single string-array and at [2] the files as a single string-array.</returns>
		public static string[][] Parse(string text)
		{
			if (!regexMainDir.IsMatch(text) || (!regexDirs.IsMatch(text) && !regexFiles.IsMatch(text)))
			{
				return null;
			}

			string mainDir = regexMainDir.Match(text).Groups["maindir"].Value;
			mainDir = Regex.Replace(mainDir, "/{2,}", "/");
			// TODO: think about that one...
			if (mainDir.EndsWith("/"))
			{
				mainDir = mainDir.Substring(0, mainDir.Length - 1);
			}

			List<string> directories = new List<string>();
			if (regexDirs.IsMatch(text))
			{
				MatchCollection matchDirs = regexDirs.Matches(text);
				for (int i = 0; i < matchDirs.Count; i++)
				{
					directories.Add(matchDirs[i].Groups["directory"].Value);
				}
			}

			List<string> files = new List<string>();
			if (regexFiles.IsMatch(text))
			{
				MatchCollection matchFiles = regexFiles.Matches(text);
				for (int i = 0; i < matchFiles.Count; i++)
				{
					files.Add(matchFiles[i].Groups["file"].Value);
				}
			}

			string[][] result = new string[3][];
			result[0] = new string[] { mainDir };
			result[1] = directories.ToArray();
			result[2] = files.ToArray();

			return result;
		}
	}
}
