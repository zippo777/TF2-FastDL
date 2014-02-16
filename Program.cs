using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace TF2_FastDL
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		// This method loads Assemblys such as the SharpZipLib which is needed for bz2-files.
		// But only when they're needed. That makes it so special...
		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			if (!args.Name.Contains("ICSharpCode.SharpZipLib"))
			{
				return null;
			}

			Assembly result = null;
			using (Stream embedded = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Program).Namespace + ".ICSharpCode.SharpZipLib.dll"))
			{
				byte[] buffer = new byte[embedded.Length];
				int length = buffer.Length;
				int offset = 0;
				while (length > 0)
				{
					int read = embedded.Read(buffer, offset, length);
					if (read == 0)
					{
						break;
					}
					length -= read;
					offset += read;
				}
				result = Assembly.Load(buffer);
			}

			return result;
		}
	}
}