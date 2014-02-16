namespace TF2_FastDL
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.components != null)
				{
					this.components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBoxPath = new System.Windows.Forms.TextBox();
			this.buttonPath = new System.Windows.Forms.Button();
			this.textBoxURL = new System.Windows.Forms.TextBox();
			this.buttonDownload = new System.Windows.Forms.Button();
			this.progressBarTotal = new System.Windows.Forms.ProgressBar();
			this.progressBarDownload = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// textBoxPath
			// 
			this.textBoxPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxPath.Location = new System.Drawing.Point(12, 12);
			this.textBoxPath.Name = "textBoxPath";
			this.textBoxPath.ReadOnly = true;
			this.textBoxPath.Size = new System.Drawing.Size(338, 20);
			this.textBoxPath.TabIndex = 0;
			// 
			// buttonPath
			// 
			this.buttonPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonPath.Location = new System.Drawing.Point(356, 10);
			this.buttonPath.Name = "buttonPath";
			this.buttonPath.Size = new System.Drawing.Size(30, 23);
			this.buttonPath.TabIndex = 1;
			this.buttonPath.Text = "...";
			this.buttonPath.UseVisualStyleBackColor = true;
			this.buttonPath.Click += new System.EventHandler(this.ButtonPathClick);
			// 
			// textBoxURL
			// 
			this.textBoxURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxURL.Location = new System.Drawing.Point(12, 38);
			this.textBoxURL.Name = "textBoxURL";
			this.textBoxURL.ReadOnly = true;
			this.textBoxURL.Size = new System.Drawing.Size(374, 20);
			this.textBoxURL.TabIndex = 2;
			this.textBoxURL.Text = "http://fastdl.5bo.de/";
			// 
			// buttonDownload
			// 
			this.buttonDownload.Location = new System.Drawing.Point(12, 64);
			this.buttonDownload.Name = "buttonDownload";
			this.buttonDownload.Size = new System.Drawing.Size(374, 23);
			this.buttonDownload.TabIndex = 3;
			this.buttonDownload.Text = "Download and extract";
			this.buttonDownload.UseVisualStyleBackColor = true;
			this.buttonDownload.Click += new System.EventHandler(this.ButtonDownloadClick);
			// 
			// progressBarTotal
			// 
			this.progressBarTotal.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.progressBarTotal.Location = new System.Drawing.Point(0, 96);
			this.progressBarTotal.Name = "progressBarTotal";
			this.progressBarTotal.Size = new System.Drawing.Size(398, 23);
			this.progressBarTotal.Step = 1;
			this.progressBarTotal.TabIndex = 4;
			// 
			// progressBarDownload
			// 
			this.progressBarDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.progressBarDownload.Location = new System.Drawing.Point(0, 72);
			this.progressBarDownload.MarqueeAnimationSpeed = 15;
			this.progressBarDownload.Name = "progressBarDownload";
			this.progressBarDownload.Size = new System.Drawing.Size(398, 23);
			this.progressBarDownload.Step = 1;
			this.progressBarDownload.TabIndex = 5;
			this.progressBarDownload.Visible = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(398, 119);
			this.Controls.Add(this.progressBarDownload);
			this.Controls.Add(this.progressBarTotal);
			this.Controls.Add(this.buttonDownload);
			this.Controls.Add(this.textBoxURL);
			this.Controls.Add(this.buttonPath);
			this.Controls.Add(this.textBoxPath);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "TF2_FastDL";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ProgressBar progressBarDownload;
		private System.Windows.Forms.ProgressBar progressBarTotal;
		private System.Windows.Forms.Button buttonDownload;
		private System.Windows.Forms.TextBox textBoxURL;
		private System.Windows.Forms.Button buttonPath;
		private System.Windows.Forms.TextBox textBoxPath;
	}
}
