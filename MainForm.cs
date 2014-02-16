﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.BZip2;
using Microsoft.Win32;

namespace TF2_FastDL
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private int currentItem = 0;
		private bool cancellingWorker = false;
		private List<string> downloadQueue = new List<string>();
		//private List<DateTime> downloadQueueLastModified = new List<DateTime>();
		private BackgroundWorker downloadWorker = null;
		private bool isClosing = false;
		private Regex regexInstallDir = new Regex("^\\s+\"installdir\"\\s+\"(?<installdir>.+)\"\\s*$", RegexOptions.IgnoreCase);
		private Regex regexLibFolder = new Regex("^\\s+\"BaseInstallFolder_\\d+\"\\s+\"(?<libraryfolder>.+)\"\\s*$", RegexOptions.IgnoreCase);
		private string steamPath = String.Empty;
		private const ushort TF2_ID = 440;
		private string tf2Path = String.Empty;

		public string SteamPath
		{
			get
			{
				return this.steamPath;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				if (File.Exists(value + @"\Steam.exe"))
				{
					this.steamPath = value;
				}
				else
				{
					throw new FileNotFoundException(LocalizationManager.GetLocalizedString("SteamNotFound"), "Steam.exe");
				}
			}
		}

		public string TF2Path
		{
			get
			{
				return this.tf2Path;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				if (File.Exists(value + @"\hl2.exe"))
				{
					this.tf2Path = this.textBoxPath.Text = value;
				}
				else
				{
					throw new FileNotFoundException(LocalizationManager.GetLocalizedString("HL2NotFound"), "hl2.exe");
				}
			}
		}

		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			// We want the designer to work so we have to put the localization here:
			this.buttonDownload.Text = LocalizationManager.GetLocalizedString("DownloadExtract");

			// Initialize the BackgroundWorker so we don't have to do it later
			this.downloadWorker = new BackgroundWorker();
			this.downloadWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
			this.downloadWorker.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker_ProgressChanged);
			this.downloadWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
			this.downloadWorker.WorkerReportsProgress = true;
			this.downloadWorker.WorkerSupportsCancellation = true;

			try
			{
				using (RegistryKey steam = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam"))
				{
					this.SteamPath = (string)steam.GetValue("InstallPath", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Steam");
				}
			}
			// Not the right steam path... can't find it.
			catch (FileNotFoundException)
			{
				return;
			}
			// Registry access not allowed - just try and error...
			catch
			{
				if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Steam\Steam.exe"))
				{
					this.SteamPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Steam";
				}
				else
				{
					return;
				}
			}

			if (!File.Exists(this.SteamPath + @"\config\config.vdf"))
			{
				return;
			}

			using (StreamReader sr = new StreamReader(this.SteamPath + @"\config\config.vdf"))
			{
				while (!sr.EndOfStream)
				{
					string temp = sr.ReadLine();
					if (Regex.IsMatch(temp, "^\\s+\"" + TF2_ID.ToString() + "\"\\s*$"))
					{
						if (Regex.IsMatch(sr.ReadLine(), "^\\s+\\{\\s*$"))
						{
							temp = sr.ReadLine();
							while (!Regex.IsMatch(temp, "^\\s+\\}\\s*$"))
							{
								if (this.regexInstallDir.IsMatch(temp))
								{
									string allegedInstallPath = this.regexInstallDir.Match(temp).Groups["installdir"].Value.Replace(@"\\", @"\");
									if (File.Exists(allegedInstallPath + @"\hl2.exe"))
									{
										this.TF2Path = allegedInstallPath;
										return;
									}
								}
								temp = sr.ReadLine();
							}
						}
					}
					else if (regexLibFolder.IsMatch(temp))
					{
						string libraryFolder = this.regexLibFolder.Match(temp).Groups["libraryfolder"].Value.Replace(@"\\", @"\");
						if (File.Exists(libraryFolder + @"\SteamApps\common\Team Fortress 2\hl2.exe"))
						{
							this.TF2Path = libraryFolder + @"\SteamApps\common\Team Fortress 2";
							return;
						}
					}
				}
			}
		}

		private void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.downloadWorker != null && this.downloadWorker.IsBusy)
			{
				this.cancellingWorker = true;
				this.isClosing = true;
				this.downloadWorker.CancelAsync();
				while (this.cancellingWorker)
				{
					Thread.Sleep(50);
				}
			}
		}

		private void ButtonPathClick(object sender, EventArgs e)
		{
			using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
			{
				folderBrowser.Description = LocalizationManager.GetLocalizedString("SelectTF2Folder");
				folderBrowser.ShowNewFolderButton = false;

				if (folderBrowser.ShowDialog() == DialogResult.OK)
				{
					if (File.Exists(folderBrowser.SelectedPath + @"\hl2.exe"))
					{
						this.TF2Path = folderBrowser.SelectedPath;
					}
					else
					{
						DirectoryInfo dirInfo = new DirectoryInfo(folderBrowser.SelectedPath);
						while (dirInfo.Parent != null)
						{
							dirInfo = dirInfo.Parent;
							if (File.Exists(dirInfo.FullName + @"\hl2.exe"))
							{
								this.TF2Path = dirInfo.FullName;
								return;
							}
						}

						MessageBox.Show(LocalizationManager.GetLocalizedString("NotTheRightTF2Folder"), LocalizationManager.GetLocalizedString("WheresTeamFortress2"), MessageBoxButtons.OK, MessageBoxIcon.Error);
						ButtonPathClick(this, null);
					}
				}
			}
		}

		private void ButtonDownloadClick(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(this.textBoxPath.Text))
			{
				//MessageBox.Show("Please select the Team Fortress 2-Folder.", "Where to go?", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				ButtonPathClick(this, null);
				if (String.IsNullOrEmpty(this.textBoxPath.Text))
				{
					return;
				}
			}

			this.Cursor = Cursors.WaitCursor;
			this.buttonDownload.Hide();
			this.buttonPath.Enabled = false;
			this.progressBarDownload.Show();
			this.Update();

			string fastdlDownloadPath = this.textBoxURL.Text + "tf/";
			string[][] tfIndex;
			using (WebClient client = new WebClient())
			{
				tfIndex = IndexParser.Parse(client.DownloadString(fastdlDownloadPath));
			}

			CreateFolders(tfIndex[0][0], tfIndex[1]);
			AddFilesToQueue(tfIndex[0][0], tfIndex[2]);
			ProcessFolder(fastdlDownloadPath, tfIndex[0][0], tfIndex[1]);

			this.progressBarTotal.Maximum = this.downloadQueue.Count * 2;
			this.Update();

			this.downloadWorker.RunWorkerAsync();
		}

		private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			this.progressBarTotal.Value = (int)e.UserState;
			this.Update();
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled || this.isClosing)
			{
				this.cancellingWorker = false;
				return;
			}
			else if (e.Error != null)
			{
				throw e.Error;
			}

			this.Cursor = Cursors.Default;
			this.Update();

			MessageBox.Show(LocalizationManager.GetLocalizedString("AllDownloaded"), LocalizationManager.GetLocalizedString("HaveFun"), MessageBoxButtons.OK, MessageBoxIcon.Information);

			this.buttonPath.Enabled = true;
			this.buttonDownload.Show();
			this.progressBarDownload.Hide();
			this.progressBarDownload.Value = 0;
			this.progressBarTotal.Value = 0;
			this.Update();
		}

		public void AddFilesToQueue(string index, string[] files/*, string[] lastModified*/)
		{
			if (files.Length == 0/* || lastModified.Length == 0*/)
			{
				return;
			}

			if (index.StartsWith("/tf", true, null))
			{
				index = index.Substring("/tf".Length);
			}
			index = index.Replace('/', '\\');

			for (int i = 0; i < files.Length; i++)
			{
				this.downloadQueue.Add(index + @"\" + files[i]);
				//this.downloadQueueLastModified.Add(new DateTime(Int64.Parse(lastModified[i])));
			}
		}

		public void CreateFolders(string index, string[] folder/*, string[] lastModified*/)
		{
			if (folder.Length == 0/* || lastModified.Length == 0*/)
			{
				return;
			}

			if (index.StartsWith("/tf", true, null))
			{
				index = index.Substring("/tf".Length);
			}
			index = index.Replace('/', '\\');

			for (int i = 0; i < folder.Length; i++)
			{
				if (!Directory.Exists(this.TF2Path + @"\tf\download" + index + @"\" + folder[i].Replace('/', '\\')))
				{
					DirectoryInfo dirInfo = Directory.CreateDirectory(this.TF2Path + @"\tf\download" + index + @"\" + folder[i].Replace('/', '\\'));
					//dirInfo.LastWriteTime = new DateTime(Int64.Parse(lastModified[i]));
				}
			}
		}

		private void ProcessFolder(string fastdlDownloadPath, string index, string[] folder)
		{
			if (index.StartsWith("/tf", true, null))
			{
				index = index.Substring("/tf".Length);
			}

			for (int i = 0; i < folder.Length; i++)
			{
				string[][] tfFolderIndex;
				using (WebClient client = new WebClient())
				{
					tfFolderIndex = IndexParser.Parse(client.DownloadString(fastdlDownloadPath + index + "/" + folder[i]));
				}

				CreateFolders(tfFolderIndex[0][0], tfFolderIndex[1]);
				AddFilesToQueue(tfFolderIndex[0][0], tfFolderIndex[2]);
				ProcessFolder(fastdlDownloadPath, tfFolderIndex[0][0], tfFolderIndex[1]);
			}
		}

		// The following methods are handled in a seperated thread so edit carefully!
		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			string[] downloadArr = this.downloadQueue.ToArray();
			//DateTime[] lastModifiedArr = this.downloadQueueLastModified.ToArray();

			int counter = 0;
			for (int i = 0; i < downloadArr.Length; i++)
			{
				this.currentItem = i;
				string destination = this.TF2Path + @"\tf\download" + downloadArr[i];
				string extractDest = Path.ChangeExtension(destination, null);
				if (File.Exists(extractDest))
				{
					counter += 2;
					this.downloadWorker.ReportProgress(counter/this.progressBarTotal.Maximum, counter);
					continue;
				}

				if (this.downloadWorker.CancellationPending)
				{
					return;
				}

				if (!File.Exists(destination))
				{
					string downloadUrl = this.textBoxURL.Text + "tf" + downloadArr[i].Replace('\\', '/');
					using (WebClient client = new WebClient())
					{
						client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
						// Never do anonymous methods... they're ugly... really ugly...
						client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);
						client.DownloadFileAsync(new Uri(downloadUrl), destination);
						while (client.IsBusy)
						{
							Thread.Sleep(150);
							if (this.downloadWorker.CancellationPending)
							{
								client.CancelAsync();
								while (client.IsBusy)
								{
									Thread.Sleep(100);
								}
								return;
							}
						}
					}
				}
				counter++;
				this.downloadWorker.ReportProgress(counter/this.progressBarTotal.Maximum, counter);

				SetProgressBarDownloadMarquee(true);
				// I love this simplicity...
				if (Path.GetExtension(destination) == ".bz2")
				{
					BZip2.Decompress(File.OpenRead(destination), File.Create(extractDest), true);
				}
				//File.SetLastWriteTime(extractDest, lastModifiedArr[i]);
				counter++;
				this.downloadWorker.ReportProgress(counter/this.progressBarTotal.Maximum, counter);
				File.Delete(destination);
				SetProgressBarDownloadMarquee(false);
			}
		}

		private void Client_DownloadProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			UpdateProgressBarDownload(e.ProgressPercentage);
		}

		private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				if (File.Exists(this.TF2Path + @"\tf\download" + this.downloadQueue[this.currentItem]))
				{
					File.Delete(this.TF2Path + @"\tf\download" + this.downloadQueue[this.currentItem]);
				}
				this.cancellingWorker = false;
				return;
			}
			else if (e.Error != null)
			{
				throw e.Error;
			}
			UpdateProgressBarDownload(0);
		}

		// And since we all LOVE threads, things are getting more complicated at this point:
		// Now we have a method, which wants to access winforms-controls...
		// Since they only can be accessed from the thread, which created them, we use a quick hack:
		private void SetProgressBarDownloadMarquee(object marquee)
		{
			if (InvokeRequired)
			{
				try
				{
					ObjDelegate method = new ObjDelegate(SetProgressBarDownloadMarquee);
					Invoke(method, marquee);
				}
				catch {}
				return;
			}

			if ((bool)marquee)
			{
				this.progressBarDownload.Style = ProgressBarStyle.Marquee;
			}
			else
			{
				this.progressBarDownload.Style = ProgressBarStyle.Blocks;
			}
		}

		private void UpdateProgressBarDownload(object value)
		{
			if (InvokeRequired)
			{
				try
				{
					ObjDelegate method = new ObjDelegate(UpdateProgressBarDownload);
					Invoke(method, value);
				}
				catch {}
				return;
			}

			this.progressBarDownload.Value = (int)value;
			this.Update();
		}

		private delegate void ObjDelegate(object value);
	}
}
