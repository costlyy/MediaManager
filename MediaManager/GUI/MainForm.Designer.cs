using System.Windows.Forms;

namespace MediaManager.GUI
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnRestart = new System.Windows.Forms.Button();
			this.TitleVersion = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.panelQuickActions = new System.Windows.Forms.Panel();
			this.btnKillAll = new System.Windows.Forms.Button();
			this.lblStreamMode = new System.Windows.Forms.Label();
			this.panel11 = new System.Windows.Forms.Panel();
			this.btnToggleStreamMode = new System.Windows.Forms.Button();
			this.panel3 = new System.Windows.Forms.Panel();
			this.panelDownloadManager = new System.Windows.Forms.Panel();
			this.lblNoneActive = new System.Windows.Forms.Label();
			this.btnDownloadPause = new System.Windows.Forms.CheckBox();
			this.panel12 = new System.Windows.Forms.Panel();
			this.panelDownloadItem = new System.Windows.Forms.Panel();
			this.btnForce = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblSubTitle = new System.Windows.Forms.Label();
			this.lblTitle = new System.Windows.Forms.Label();
			this.btnDownloadToggle = new System.Windows.Forms.Button();
			this.btnShowDownloads = new System.Windows.Forms.Button();
			this.lblHeadingDownload = new System.Windows.Forms.Label();
			this.panel4 = new System.Windows.Forms.Panel();
			this.panelVpnManager = new System.Windows.Forms.Panel();
			this.panel13 = new System.Windows.Forms.Panel();
			this.label8 = new System.Windows.Forms.Label();
			this.tbxMyIP = new System.Windows.Forms.TextBox();
			this.btnVpnPause = new System.Windows.Forms.Button();
			this.btnToggleVpn = new System.Windows.Forms.Button();
			this.tbxConfig1 = new System.Windows.Forms.TextBox();
			this.tbxConfig2 = new System.Windows.Forms.TextBox();
			this.tbxConfig0 = new System.Windows.Forms.TextBox();
			this.panel10 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.btnConfigPriority2 = new System.Windows.Forms.Button();
			this.btnAddConfig2 = new System.Windows.Forms.Button();
			this.btnConfigPriority1 = new System.Windows.Forms.Button();
			this.btnAddConfig1 = new System.Windows.Forms.Button();
			this.btnConfigPriority0 = new System.Windows.Forms.Button();
			this.btnAddConfig0 = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel9 = new System.Windows.Forms.Panel();
			this.panelStatus = new System.Windows.Forms.Panel();
			this.statusTableDisk = new System.Windows.Forms.TableLayoutPanel();
			this.lblDisk2 = new System.Windows.Forms.Label();
			this.lblDisk1 = new System.Windows.Forms.Label();
			this.lblDisk0 = new System.Windows.Forms.Label();
			this.lblStorage0 = new System.Windows.Forms.Label();
			this.lblStorage1 = new System.Windows.Forms.Label();
			this.lblStorage2 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lblStorageTitle0 = new System.Windows.Forms.Label();
			this.lblSabUptime = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.lblSabState = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.lblVpnUptime = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.lblVpnState = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.panelStatusBg = new System.Windows.Forms.Panel();
			this.btnExit = new System.Windows.Forms.Button();
			this.panel5 = new System.Windows.Forms.Panel();
			this.lblDebugMode = new System.Windows.Forms.Label();
			this.btnHide = new System.Windows.Forms.Button();
			this.btnSettings = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.panel6 = new System.Windows.Forms.Panel();
			this.panel7 = new System.Windows.Forms.Panel();
			this.panel8 = new System.Windows.Forms.Panel();
			this.panelShade = new System.Windows.Forms.Panel();
			this.lblExtIpTimer = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panelQuickActions.SuspendLayout();
			this.panelDownloadManager.SuspendLayout();
			this.panelDownloadItem.SuspendLayout();
			this.panelVpnManager.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panelStatus.SuspendLayout();
			this.statusTableDisk.SuspendLayout();
			this.panelStatusBg.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel6.SuspendLayout();
			this.panel7.SuspendLayout();
			this.panel8.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnRestart
			// 
			this.btnRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRestart.Font = new System.Drawing.Font("Liberation Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnRestart.ForeColor = System.Drawing.Color.White;
			this.btnRestart.Location = new System.Drawing.Point(95, 186);
			this.btnRestart.Name = "btnRestart";
			this.btnRestart.Size = new System.Drawing.Size(250, 45);
			this.btnRestart.TabIndex = 0;
			this.btnRestart.Text = "Restart Machine";
			this.btnRestart.UseVisualStyleBackColor = true;
			this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
			// 
			// TitleVersion
			// 
			this.TitleVersion.AutoSize = true;
			this.TitleVersion.Font = new System.Drawing.Font("Liberation Sans", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TitleVersion.Location = new System.Drawing.Point(15, 59);
			this.TitleVersion.Name = "TitleVersion";
			this.TitleVersion.Size = new System.Drawing.Size(34, 30);
			this.TitleVersion.TabIndex = 3;
			this.TitleVersion.Text = "v.";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Liberation Sans", 24F, System.Drawing.FontStyle.Bold);
			this.label3.ForeColor = System.Drawing.Color.White;
			this.label3.Location = new System.Drawing.Point(117, 11);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(219, 36);
			this.label3.TabIndex = 11;
			this.label3.Text = "VPN Manager";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Liberation Sans", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.ForeColor = System.Drawing.Color.White;
			this.label5.Location = new System.Drawing.Point(106, 11);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(230, 36);
			this.label5.TabIndex = 12;
			this.label5.Text = "Quick Actions";
			// 
			// panelQuickActions
			// 
			this.panelQuickActions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.panelQuickActions.Controls.Add(this.btnKillAll);
			this.panelQuickActions.Controls.Add(this.lblStreamMode);
			this.panelQuickActions.Controls.Add(this.panel11);
			this.panelQuickActions.Controls.Add(this.btnToggleStreamMode);
			this.panelQuickActions.Controls.Add(this.panel3);
			this.panelQuickActions.Controls.Add(this.btnRestart);
			this.panelQuickActions.Controls.Add(this.label5);
			this.panelQuickActions.Location = new System.Drawing.Point(20, 20);
			this.panelQuickActions.Name = "panelQuickActions";
			this.panelQuickActions.Size = new System.Drawing.Size(440, 410);
			this.panelQuickActions.TabIndex = 15;
			// 
			// btnKillAll
			// 
			this.btnKillAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnKillAll.Font = new System.Drawing.Font("Liberation Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnKillAll.ForeColor = System.Drawing.Color.White;
			this.btnKillAll.Location = new System.Drawing.Point(95, 254);
			this.btnKillAll.Name = "btnKillAll";
			this.btnKillAll.Size = new System.Drawing.Size(250, 45);
			this.btnKillAll.TabIndex = 31;
			this.btnKillAll.Text = "Kill Everything";
			this.btnKillAll.UseVisualStyleBackColor = true;
			this.btnKillAll.Click += new System.EventHandler(this.btnKillAll_Click);
			// 
			// lblStreamMode
			// 
			this.lblStreamMode.AutoSize = true;
			this.lblStreamMode.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStreamMode.ForeColor = System.Drawing.Color.White;
			this.lblStreamMode.Location = new System.Drawing.Point(198, 123);
			this.lblStreamMode.Name = "lblStreamMode";
			this.lblStreamMode.Size = new System.Drawing.Size(45, 27);
			this.lblStreamMode.TabIndex = 30;
			this.lblStreamMode.Text = "0m";
			// 
			// panel11
			// 
			this.panel11.BackColor = System.Drawing.Color.White;
			this.panel11.Location = new System.Drawing.Point(0, 160);
			this.panel11.Name = "panel11";
			this.panel11.Size = new System.Drawing.Size(939, 3);
			this.panel11.TabIndex = 20;
			// 
			// btnToggleStreamMode
			// 
			this.btnToggleStreamMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnToggleStreamMode.Font = new System.Drawing.Font("Liberation Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnToggleStreamMode.ForeColor = System.Drawing.Color.White;
			this.btnToggleStreamMode.Location = new System.Drawing.Point(95, 66);
			this.btnToggleStreamMode.Name = "btnToggleStreamMode";
			this.btnToggleStreamMode.Size = new System.Drawing.Size(250, 45);
			this.btnToggleStreamMode.TabIndex = 20;
			this.btnToggleStreamMode.Text = "Streaming Mode Off";
			this.btnToggleStreamMode.UseVisualStyleBackColor = true;
			this.btnToggleStreamMode.Click += new System.EventHandler(this.btnToggleStreamMode_Click);
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.Color.White;
			this.panel3.Location = new System.Drawing.Point(0, 50);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(939, 3);
			this.panel3.TabIndex = 19;
			// 
			// panelDownloadManager
			// 
			this.panelDownloadManager.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.panelDownloadManager.Controls.Add(this.lblNoneActive);
			this.panelDownloadManager.Controls.Add(this.btnDownloadPause);
			this.panelDownloadManager.Controls.Add(this.panel12);
			this.panelDownloadManager.Controls.Add(this.panelDownloadItem);
			this.panelDownloadManager.Controls.Add(this.btnDownloadToggle);
			this.panelDownloadManager.Controls.Add(this.btnShowDownloads);
			this.panelDownloadManager.Controls.Add(this.lblHeadingDownload);
			this.panelDownloadManager.Controls.Add(this.panel4);
			this.panelDownloadManager.Location = new System.Drawing.Point(20, 20);
			this.panelDownloadManager.Name = "panelDownloadManager";
			this.panelDownloadManager.Size = new System.Drawing.Size(440, 410);
			this.panelDownloadManager.TabIndex = 16;
			// 
			// lblNoneActive
			// 
			this.lblNoneActive.AutoSize = true;
			this.lblNoneActive.Font = new System.Drawing.Font("Liberation Sans", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNoneActive.ForeColor = System.Drawing.Color.DarkGray;
			this.lblNoneActive.Location = new System.Drawing.Point(81, 244);
			this.lblNoneActive.Name = "lblNoneActive";
			this.lblNoneActive.Size = new System.Drawing.Size(280, 53);
			this.lblNoneActive.TabIndex = 38;
			this.lblNoneActive.Tag = "warning";
			this.lblNoneActive.Text = "None Active";
			this.lblNoneActive.Visible = false;
			// 
			// btnDownloadPause
			// 
			this.btnDownloadPause.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnDownloadPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDownloadPause.Font = new System.Drawing.Font("Liberation Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnDownloadPause.ForeColor = System.Drawing.Color.White;
			this.btnDownloadPause.Image = global::MediaManager.Properties.Resources.icon_pause;
			this.btnDownloadPause.Location = new System.Drawing.Point(318, 69);
			this.btnDownloadPause.Name = "btnDownloadPause";
			this.btnDownloadPause.Size = new System.Drawing.Size(97, 45);
			this.btnDownloadPause.TabIndex = 37;
			this.btnDownloadPause.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.btnDownloadPause.UseVisualStyleBackColor = true;
			this.btnDownloadPause.CheckedChanged += new System.EventHandler(this.btnDownloadPause_CheckedChanged);
			// 
			// panel12
			// 
			this.panel12.BackColor = System.Drawing.Color.White;
			this.panel12.Location = new System.Drawing.Point(0, 133);
			this.panel12.Name = "panel12";
			this.panel12.Size = new System.Drawing.Size(939, 3);
			this.panel12.TabIndex = 31;
			// 
			// panelDownloadItem
			// 
			this.panelDownloadItem.BackColor = System.Drawing.Color.Transparent;
			this.panelDownloadItem.Controls.Add(this.btnForce);
			this.panelDownloadItem.Controls.Add(this.btnCancel);
			this.panelDownloadItem.Controls.Add(this.lblSubTitle);
			this.panelDownloadItem.Controls.Add(this.lblTitle);
			this.panelDownloadItem.Location = new System.Drawing.Point(15, 153);
			this.panelDownloadItem.Name = "panelDownloadItem";
			this.panelDownloadItem.Size = new System.Drawing.Size(410, 64);
			this.panelDownloadItem.TabIndex = 35;
			this.panelDownloadItem.Visible = false;
			// 
			// btnForce
			// 
			this.btnForce.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnForce.Font = new System.Drawing.Font("Liberation Sans", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnForce.ForeColor = System.Drawing.SystemColors.Control;
			this.btnForce.Location = new System.Drawing.Point(321, 3);
			this.btnForce.Name = "btnForce";
			this.btnForce.Size = new System.Drawing.Size(42, 58);
			this.btnForce.TabIndex = 37;
			this.btnForce.Text = "↑";
			this.btnForce.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnCancel.Font = new System.Drawing.Font("Liberation Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCancel.ForeColor = System.Drawing.SystemColors.Control;
			this.btnCancel.Location = new System.Drawing.Point(366, 3);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(42, 58);
			this.btnCancel.TabIndex = 35;
			this.btnCancel.Text = "X";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// lblSubTitle
			// 
			this.lblSubTitle.AutoSize = true;
			this.lblSubTitle.Font = new System.Drawing.Font("Liberation Sans", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSubTitle.ForeColor = System.Drawing.SystemColors.Control;
			this.lblSubTitle.Location = new System.Drawing.Point(8, 34);
			this.lblSubTitle.Name = "lblSubTitle";
			this.lblSubTitle.Size = new System.Drawing.Size(175, 21);
			this.lblSubTitle.TabIndex = 31;
			this.lblSubTitle.Text = "$SizeAndTimeHere";
			// 
			// lblTitle
			// 
			this.lblTitle.AutoSize = true;
			this.lblTitle.Font = new System.Drawing.Font("Liberation Sans", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTitle.ForeColor = System.Drawing.SystemColors.Control;
			this.lblTitle.Location = new System.Drawing.Point(7, 8);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(144, 21);
			this.lblTitle.TabIndex = 30;
			this.lblTitle.Text = "$TitleGoesHere";
			// 
			// btnDownloadToggle
			// 
			this.btnDownloadToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDownloadToggle.Font = new System.Drawing.Font("Liberation Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnDownloadToggle.ForeColor = System.Drawing.Color.White;
			this.btnDownloadToggle.Image = global::MediaManager.Properties.Resources.icon_play_mid;
			this.btnDownloadToggle.Location = new System.Drawing.Point(208, 69);
			this.btnDownloadToggle.Name = "btnDownloadToggle";
			this.btnDownloadToggle.Size = new System.Drawing.Size(99, 45);
			this.btnDownloadToggle.TabIndex = 33;
			this.btnDownloadToggle.UseVisualStyleBackColor = true;
			this.btnDownloadToggle.Click += new System.EventHandler(this.btnDownloadToggle_Click);
			// 
			// btnShowDownloads
			// 
			this.btnShowDownloads.Enabled = false;
			this.btnShowDownloads.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnShowDownloads.Font = new System.Drawing.Font("Liberation Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnShowDownloads.ForeColor = System.Drawing.Color.White;
			this.btnShowDownloads.Location = new System.Drawing.Point(22, 69);
			this.btnShowDownloads.Name = "btnShowDownloads";
			this.btnShowDownloads.Size = new System.Drawing.Size(174, 45);
			this.btnShowDownloads.TabIndex = 32;
			this.btnShowDownloads.Text = "View Full";
			this.btnShowDownloads.UseVisualStyleBackColor = true;
			this.btnShowDownloads.Click += new System.EventHandler(this.btnShowDownloads_Click);
			// 
			// lblHeadingDownload
			// 
			this.lblHeadingDownload.AutoSize = true;
			this.lblHeadingDownload.Font = new System.Drawing.Font("Liberation Sans", 24F, System.Drawing.FontStyle.Bold);
			this.lblHeadingDownload.ForeColor = System.Drawing.Color.White;
			this.lblHeadingDownload.Location = new System.Drawing.Point(70, 10);
			this.lblHeadingDownload.Name = "lblHeadingDownload";
			this.lblHeadingDownload.Size = new System.Drawing.Size(308, 36);
			this.lblHeadingDownload.TabIndex = 21;
			this.lblHeadingDownload.Text = "Download Manager";
			// 
			// panel4
			// 
			this.panel4.BackColor = System.Drawing.Color.White;
			this.panel4.Location = new System.Drawing.Point(0, 50);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(939, 3);
			this.panel4.TabIndex = 20;
			// 
			// panelVpnManager
			// 
			this.panelVpnManager.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.panelVpnManager.Controls.Add(this.lblExtIpTimer);
			this.panelVpnManager.Controls.Add(this.panel13);
			this.panelVpnManager.Controls.Add(this.label8);
			this.panelVpnManager.Controls.Add(this.tbxMyIP);
			this.panelVpnManager.Controls.Add(this.btnVpnPause);
			this.panelVpnManager.Controls.Add(this.btnToggleVpn);
			this.panelVpnManager.Controls.Add(this.tbxConfig1);
			this.panelVpnManager.Controls.Add(this.tbxConfig2);
			this.panelVpnManager.Controls.Add(this.tbxConfig0);
			this.panelVpnManager.Controls.Add(this.panel10);
			this.panelVpnManager.Controls.Add(this.label1);
			this.panelVpnManager.Controls.Add(this.btnConfigPriority2);
			this.panelVpnManager.Controls.Add(this.btnAddConfig2);
			this.panelVpnManager.Controls.Add(this.btnConfigPriority1);
			this.panelVpnManager.Controls.Add(this.btnAddConfig1);
			this.panelVpnManager.Controls.Add(this.btnConfigPriority0);
			this.panelVpnManager.Controls.Add(this.btnAddConfig0);
			this.panelVpnManager.Controls.Add(this.panel2);
			this.panelVpnManager.Controls.Add(this.label3);
			this.panelVpnManager.Location = new System.Drawing.Point(20, 20);
			this.panelVpnManager.Name = "panelVpnManager";
			this.panelVpnManager.Size = new System.Drawing.Size(440, 410);
			this.panelVpnManager.TabIndex = 16;
			// 
			// panel13
			// 
			this.panel13.BackColor = System.Drawing.Color.White;
			this.panel13.Location = new System.Drawing.Point(0, 133);
			this.panel13.Name = "panel13";
			this.panel13.Size = new System.Drawing.Size(939, 3);
			this.panel13.TabIndex = 22;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.ForeColor = System.Drawing.Color.White;
			this.label8.Location = new System.Drawing.Point(8, 369);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(82, 27);
			this.label8.TabIndex = 42;
			this.label8.Text = "Ext. IP";
			// 
			// tbxMyIP
			// 
			this.tbxMyIP.BackColor = System.Drawing.SystemColors.Info;
			this.tbxMyIP.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbxMyIP.Location = new System.Drawing.Point(90, 369);
			this.tbxMyIP.Multiline = true;
			this.tbxMyIP.Name = "tbxMyIP";
			this.tbxMyIP.ReadOnly = true;
			this.tbxMyIP.Size = new System.Drawing.Size(299, 27);
			this.tbxMyIP.TabIndex = 41;
			this.tbxMyIP.TabStop = false;
			this.tbxMyIP.Tag = "99";
			this.tbxMyIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// btnVpnPause
			// 
			this.btnVpnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnVpnPause.Font = new System.Drawing.Font("Liberation Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnVpnPause.ForeColor = System.Drawing.Color.White;
			this.btnVpnPause.Image = global::MediaManager.Properties.Resources.icon_pause;
			this.btnVpnPause.Location = new System.Drawing.Point(227, 69);
			this.btnVpnPause.Name = "btnVpnPause";
			this.btnVpnPause.Size = new System.Drawing.Size(87, 45);
			this.btnVpnPause.TabIndex = 40;
			this.btnVpnPause.Tag = "1";
			this.btnVpnPause.UseVisualStyleBackColor = true;
			this.btnVpnPause.Click += new System.EventHandler(this.btnVpnPause_Click);
			// 
			// btnToggleVpn
			// 
			this.btnToggleVpn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnToggleVpn.Font = new System.Drawing.Font("Liberation Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnToggleVpn.ForeColor = System.Drawing.Color.White;
			this.btnToggleVpn.Image = global::MediaManager.Properties.Resources.icon_play_mid;
			this.btnToggleVpn.Location = new System.Drawing.Point(128, 69);
			this.btnToggleVpn.Name = "btnToggleVpn";
			this.btnToggleVpn.Size = new System.Drawing.Size(87, 45);
			this.btnToggleVpn.TabIndex = 20;
			this.btnToggleVpn.Tag = "1";
			this.btnToggleVpn.UseVisualStyleBackColor = true;
			this.btnToggleVpn.Click += new System.EventHandler(this.btnToggleVpn_Click);
			// 
			// tbxConfig1
			// 
			this.tbxConfig1.BackColor = System.Drawing.SystemColors.Info;
			this.tbxConfig1.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbxConfig1.Location = new System.Drawing.Point(13, 262);
			this.tbxConfig1.Multiline = true;
			this.tbxConfig1.Name = "tbxConfig1";
			this.tbxConfig1.ReadOnly = true;
			this.tbxConfig1.Size = new System.Drawing.Size(301, 35);
			this.tbxConfig1.TabIndex = 33;
			this.tbxConfig1.TabStop = false;
			this.tbxConfig1.Tag = "11";
			// 
			// tbxConfig2
			// 
			this.tbxConfig2.BackColor = System.Drawing.SystemColors.Info;
			this.tbxConfig2.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbxConfig2.Location = new System.Drawing.Point(13, 320);
			this.tbxConfig2.Multiline = true;
			this.tbxConfig2.Name = "tbxConfig2";
			this.tbxConfig2.ReadOnly = true;
			this.tbxConfig2.Size = new System.Drawing.Size(301, 35);
			this.tbxConfig2.TabIndex = 36;
			this.tbxConfig2.TabStop = false;
			this.tbxConfig2.Tag = "12";
			// 
			// tbxConfig0
			// 
			this.tbxConfig0.BackColor = System.Drawing.SystemColors.Info;
			this.tbxConfig0.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbxConfig0.Location = new System.Drawing.Point(13, 203);
			this.tbxConfig0.Multiline = true;
			this.tbxConfig0.Name = "tbxConfig0";
			this.tbxConfig0.ReadOnly = true;
			this.tbxConfig0.Size = new System.Drawing.Size(301, 35);
			this.tbxConfig0.TabIndex = 30;
			this.tbxConfig0.TabStop = false;
			this.tbxConfig0.Tag = "10";
			// 
			// panel10
			// 
			this.panel10.BackColor = System.Drawing.Color.White;
			this.panel10.Location = new System.Drawing.Point(0, 184);
			this.panel10.Name = "panel10";
			this.panel10.Size = new System.Drawing.Size(939, 3);
			this.panel10.TabIndex = 21;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Liberation Sans", 24F, System.Drawing.FontStyle.Bold);
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(122, 142);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(206, 36);
			this.label1.TabIndex = 39;
			this.label1.Text = "VPN Configs";
			// 
			// btnConfigPriority2
			// 
			this.btnConfigPriority2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnConfigPriority2.Font = new System.Drawing.Font("Liberation Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnConfigPriority2.ForeColor = System.Drawing.Color.White;
			this.btnConfigPriority2.Location = new System.Drawing.Point(376, 320);
			this.btnConfigPriority2.Name = "btnConfigPriority2";
			this.btnConfigPriority2.Size = new System.Drawing.Size(49, 35);
			this.btnConfigPriority2.TabIndex = 38;
			this.btnConfigPriority2.Tag = "32";
			this.btnConfigPriority2.Text = "3";
			this.btnConfigPriority2.UseVisualStyleBackColor = true;
			// 
			// btnAddConfig2
			// 
			this.btnAddConfig2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAddConfig2.Font = new System.Drawing.Font("Liberation Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnAddConfig2.ForeColor = System.Drawing.Color.White;
			this.btnAddConfig2.Location = new System.Drawing.Point(321, 320);
			this.btnAddConfig2.Name = "btnAddConfig2";
			this.btnAddConfig2.Size = new System.Drawing.Size(49, 35);
			this.btnAddConfig2.TabIndex = 37;
			this.btnAddConfig2.Tag = "22";
			this.btnAddConfig2.Text = "+";
			this.btnAddConfig2.UseVisualStyleBackColor = true;
			// 
			// btnConfigPriority1
			// 
			this.btnConfigPriority1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnConfigPriority1.Font = new System.Drawing.Font("Liberation Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnConfigPriority1.ForeColor = System.Drawing.Color.White;
			this.btnConfigPriority1.Location = new System.Drawing.Point(376, 262);
			this.btnConfigPriority1.Name = "btnConfigPriority1";
			this.btnConfigPriority1.Size = new System.Drawing.Size(49, 35);
			this.btnConfigPriority1.TabIndex = 35;
			this.btnConfigPriority1.Tag = "31";
			this.btnConfigPriority1.Text = "2";
			this.btnConfigPriority1.UseVisualStyleBackColor = true;
			// 
			// btnAddConfig1
			// 
			this.btnAddConfig1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAddConfig1.Font = new System.Drawing.Font("Liberation Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnAddConfig1.ForeColor = System.Drawing.Color.White;
			this.btnAddConfig1.Location = new System.Drawing.Point(321, 262);
			this.btnAddConfig1.Name = "btnAddConfig1";
			this.btnAddConfig1.Size = new System.Drawing.Size(49, 35);
			this.btnAddConfig1.TabIndex = 34;
			this.btnAddConfig1.Tag = "21";
			this.btnAddConfig1.Text = "+";
			this.btnAddConfig1.UseVisualStyleBackColor = true;
			// 
			// btnConfigPriority0
			// 
			this.btnConfigPriority0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnConfigPriority0.Font = new System.Drawing.Font("Liberation Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnConfigPriority0.ForeColor = System.Drawing.Color.White;
			this.btnConfigPriority0.Location = new System.Drawing.Point(376, 203);
			this.btnConfigPriority0.Name = "btnConfigPriority0";
			this.btnConfigPriority0.Size = new System.Drawing.Size(49, 35);
			this.btnConfigPriority0.TabIndex = 32;
			this.btnConfigPriority0.Tag = "30";
			this.btnConfigPriority0.Text = "1";
			this.btnConfigPriority0.UseVisualStyleBackColor = true;
			// 
			// btnAddConfig0
			// 
			this.btnAddConfig0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAddConfig0.Font = new System.Drawing.Font("Liberation Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnAddConfig0.ForeColor = System.Drawing.Color.White;
			this.btnAddConfig0.Location = new System.Drawing.Point(321, 203);
			this.btnAddConfig0.Name = "btnAddConfig0";
			this.btnAddConfig0.Size = new System.Drawing.Size(49, 35);
			this.btnAddConfig0.TabIndex = 31;
			this.btnAddConfig0.Tag = "20";
			this.btnAddConfig0.Text = "+";
			this.btnAddConfig0.UseVisualStyleBackColor = true;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.White;
			this.panel2.Controls.Add(this.panel9);
			this.panel2.Location = new System.Drawing.Point(0, 50);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(939, 3);
			this.panel2.TabIndex = 19;
			// 
			// panel9
			// 
			this.panel9.BackColor = System.Drawing.Color.White;
			this.panel9.Location = new System.Drawing.Point(0, 0);
			this.panel9.Name = "panel9";
			this.panel9.Size = new System.Drawing.Size(939, 3);
			this.panel9.TabIndex = 20;
			// 
			// panelStatus
			// 
			this.panelStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.panelStatus.Controls.Add(this.tableLayoutPanel1);
			this.panelStatus.Controls.Add(this.statusTableDisk);
			this.panelStatus.Controls.Add(this.panel1);
			this.panelStatus.Controls.Add(this.lblStorageTitle0);
			this.panelStatus.Controls.Add(this.label6);
			this.panelStatus.Location = new System.Drawing.Point(20, 20);
			this.panelStatus.Name = "panelStatus";
			this.panelStatus.Size = new System.Drawing.Size(440, 410);
			this.panelStatus.TabIndex = 17;
			// 
			// statusTableDisk
			// 
			this.statusTableDisk.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this.statusTableDisk.ColumnCount = 3;
			this.statusTableDisk.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.statusTableDisk.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
			this.statusTableDisk.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
			this.statusTableDisk.Controls.Add(this.lblDisk2, 2, 0);
			this.statusTableDisk.Controls.Add(this.lblDisk1, 1, 0);
			this.statusTableDisk.Controls.Add(this.lblDisk0, 0, 0);
			this.statusTableDisk.Controls.Add(this.lblStorage0, 0, 1);
			this.statusTableDisk.Controls.Add(this.lblStorage1, 1, 1);
			this.statusTableDisk.Controls.Add(this.lblStorage2, 2, 1);
			this.statusTableDisk.Location = new System.Drawing.Point(15, 322);
			this.statusTableDisk.Name = "statusTableDisk";
			this.statusTableDisk.RowCount = 2;
			this.statusTableDisk.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.statusTableDisk.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
			this.statusTableDisk.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.statusTableDisk.Size = new System.Drawing.Size(408, 75);
			this.statusTableDisk.TabIndex = 30;
			// 
			// lblDisk2
			// 
			this.lblDisk2.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblDisk2.AutoSize = true;
			this.lblDisk2.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDisk2.ForeColor = System.Drawing.SystemColors.Control;
			this.lblDisk2.Location = new System.Drawing.Point(299, 2);
			this.lblDisk2.Name = "lblDisk2";
			this.lblDisk2.Size = new System.Drawing.Size(80, 27);
			this.lblDisk2.TabIndex = 33;
			this.lblDisk2.Text = "DISK2";
			// 
			// lblDisk1
			// 
			this.lblDisk1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblDisk1.AutoSize = true;
			this.lblDisk1.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDisk1.ForeColor = System.Drawing.SystemColors.Control;
			this.lblDisk1.Location = new System.Drawing.Point(163, 2);
			this.lblDisk1.Name = "lblDisk1";
			this.lblDisk1.Size = new System.Drawing.Size(80, 27);
			this.lblDisk1.TabIndex = 32;
			this.lblDisk1.Text = "DISK1";
			// 
			// lblDisk0
			// 
			this.lblDisk0.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblDisk0.AutoSize = true;
			this.lblDisk0.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDisk0.ForeColor = System.Drawing.SystemColors.Control;
			this.lblDisk0.Location = new System.Drawing.Point(28, 2);
			this.lblDisk0.Name = "lblDisk0";
			this.lblDisk0.Size = new System.Drawing.Size(80, 27);
			this.lblDisk0.TabIndex = 31;
			this.lblDisk0.Text = "DISK0";
			// 
			// lblStorage0
			// 
			this.lblStorage0.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblStorage0.AutoSize = true;
			this.lblStorage0.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStorage0.ForeColor = System.Drawing.Color.White;
			this.lblStorage0.Location = new System.Drawing.Point(18, 39);
			this.lblStorage0.Name = "lblStorage0";
			this.lblStorage0.Size = new System.Drawing.Size(99, 27);
			this.lblStorage0.TabIndex = 27;
			this.lblStorage0.Text = "9999GB";
			// 
			// lblStorage1
			// 
			this.lblStorage1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblStorage1.AutoSize = true;
			this.lblStorage1.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStorage1.ForeColor = System.Drawing.Color.White;
			this.lblStorage1.Location = new System.Drawing.Point(153, 39);
			this.lblStorage1.Name = "lblStorage1";
			this.lblStorage1.Size = new System.Drawing.Size(99, 27);
			this.lblStorage1.TabIndex = 29;
			this.lblStorage1.Text = "9999GB";
			// 
			// lblStorage2
			// 
			this.lblStorage2.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblStorage2.AutoSize = true;
			this.lblStorage2.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStorage2.ForeColor = System.Drawing.Color.White;
			this.lblStorage2.Location = new System.Drawing.Point(289, 39);
			this.lblStorage2.Name = "lblStorage2";
			this.lblStorage2.Size = new System.Drawing.Size(99, 27);
			this.lblStorage2.TabIndex = 30;
			this.lblStorage2.Text = "9999GB";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.White;
			this.panel1.Location = new System.Drawing.Point(0, 50);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(939, 3);
			this.panel1.TabIndex = 18;
			// 
			// lblStorageTitle0
			// 
			this.lblStorageTitle0.AutoSize = true;
			this.lblStorageTitle0.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblStorageTitle0.ForeColor = System.Drawing.SystemColors.Control;
			this.lblStorageTitle0.Location = new System.Drawing.Point(103, 281);
			this.lblStorageTitle0.Name = "lblStorageTitle0";
			this.lblStorageTitle0.Size = new System.Drawing.Size(230, 27);
			this.lblStorageTitle0.TabIndex = 26;
			this.lblStorageTitle0.Text = "Storage Totals (GB):";
			// 
			// lblSabUptime
			// 
			this.lblSabUptime.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblSabUptime.AutoSize = true;
			this.lblSabUptime.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSabUptime.ForeColor = System.Drawing.Color.White;
			this.lblSabUptime.Location = new System.Drawing.Point(241, 160);
			this.lblSabUptime.Name = "lblSabUptime";
			this.lblSabUptime.Size = new System.Drawing.Size(137, 27);
			this.lblSabUptime.TabIndex = 25;
			this.lblSabUptime.Text = "00:00:00:00";
			// 
			// label12
			// 
			this.label12.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label12.AutoSize = true;
			this.label12.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label12.ForeColor = System.Drawing.SystemColors.Control;
			this.label12.Location = new System.Drawing.Point(5, 160);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(203, 27);
			this.label12.TabIndex = 24;
			this.label12.Text = "SABnzbd Uptime:";
			// 
			// lblSabState
			// 
			this.lblSabState.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblSabState.AutoSize = true;
			this.lblSabState.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSabState.ForeColor = System.Drawing.Color.Red;
			this.lblSabState.Location = new System.Drawing.Point(247, 109);
			this.lblSabState.Name = "lblSabState";
			this.lblSabState.Size = new System.Drawing.Size(126, 27);
			this.lblSabState.TabIndex = 23;
			this.lblSabState.Text = "STOPPED";
			// 
			// label9
			// 
			this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.ForeColor = System.Drawing.SystemColors.Control;
			this.label9.Location = new System.Drawing.Point(15, 109);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(183, 27);
			this.label9.TabIndex = 22;
			this.label9.Text = "SABnzbd State:";
			// 
			// lblVpnUptime
			// 
			this.lblVpnUptime.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblVpnUptime.AutoSize = true;
			this.lblVpnUptime.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblVpnUptime.ForeColor = System.Drawing.Color.White;
			this.lblVpnUptime.Location = new System.Drawing.Point(241, 60);
			this.lblVpnUptime.Name = "lblVpnUptime";
			this.lblVpnUptime.Size = new System.Drawing.Size(137, 27);
			this.lblVpnUptime.TabIndex = 21;
			this.lblVpnUptime.Text = "00:00:00:00";
			// 
			// label10
			// 
			this.label10.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.ForeColor = System.Drawing.SystemColors.Control;
			this.label10.Location = new System.Drawing.Point(31, 60);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(150, 27);
			this.label10.TabIndex = 20;
			this.label10.Text = "VPN Uptime:";
			// 
			// lblVpnState
			// 
			this.lblVpnState.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblVpnState.AutoSize = true;
			this.lblVpnState.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblVpnState.ForeColor = System.Drawing.Color.Red;
			this.lblVpnState.Location = new System.Drawing.Point(247, 11);
			this.lblVpnState.Name = "lblVpnState";
			this.lblVpnState.Size = new System.Drawing.Size(126, 27);
			this.lblVpnState.TabIndex = 19;
			this.lblVpnState.Text = "STOPPED";
			// 
			// label7
			// 
			this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.ForeColor = System.Drawing.SystemColors.Control;
			this.label7.Location = new System.Drawing.Point(41, 11);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(130, 27);
			this.label7.TabIndex = 18;
			this.label7.Text = "VPN State:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Liberation Sans", 24F, System.Drawing.FontStyle.Bold);
			this.label6.ForeColor = System.Drawing.Color.White;
			this.label6.Location = new System.Drawing.Point(166, 11);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(113, 36);
			this.label6.TabIndex = 12;
			this.label6.Text = "Status";
			// 
			// panelStatusBg
			// 
			this.panelStatusBg.BackColor = System.Drawing.Color.Gray;
			this.panelStatusBg.Controls.Add(this.panelStatus);
			this.panelStatusBg.Location = new System.Drawing.Point(499, 123);
			this.panelStatusBg.Name = "panelStatusBg";
			this.panelStatusBg.Size = new System.Drawing.Size(481, 450);
			this.panelStatusBg.TabIndex = 17;
			// 
			// btnExit
			// 
			this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnExit.Font = new System.Drawing.Font("Liberation Sans", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnExit.ForeColor = System.Drawing.Color.White;
			this.btnExit.Location = new System.Drawing.Point(829, 18);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(116, 71);
			this.btnExit.TabIndex = 18;
			this.btnExit.Text = "EXIT";
			this.btnExit.UseVisualStyleBackColor = false;
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// panel5
			// 
			this.panel5.BackColor = System.Drawing.Color.Gray;
			this.panel5.Controls.Add(this.lblDebugMode);
			this.panel5.Controls.Add(this.TitleVersion);
			this.panel5.Controls.Add(this.btnHide);
			this.panel5.Controls.Add(this.btnSettings);
			this.panel5.Controls.Add(this.label2);
			this.panel5.Controls.Add(this.btnExit);
			this.panel5.Location = new System.Drawing.Point(12, 12);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(968, 105);
			this.panel5.TabIndex = 19;
			// 
			// lblDebugMode
			// 
			this.lblDebugMode.AutoSize = true;
			this.lblDebugMode.Font = new System.Drawing.Font("Liberation Sans", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDebugMode.ForeColor = System.Drawing.Color.Firebrick;
			this.lblDebugMode.Location = new System.Drawing.Point(281, 72);
			this.lblDebugMode.Name = "lblDebugMode";
			this.lblDebugMode.Size = new System.Drawing.Size(200, 30);
			this.lblDebugMode.TabIndex = 21;
			this.lblDebugMode.Text = "DEBUG MODE";
			this.lblDebugMode.Visible = false;
			// 
			// btnHide
			// 
			this.btnHide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnHide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnHide.Font = new System.Drawing.Font("Liberation Sans", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnHide.ForeColor = System.Drawing.Color.White;
			this.btnHide.Location = new System.Drawing.Point(706, 18);
			this.btnHide.Name = "btnHide";
			this.btnHide.Size = new System.Drawing.Size(116, 71);
			this.btnHide.TabIndex = 20;
			this.btnHide.Text = "Hide";
			this.btnHide.UseVisualStyleBackColor = false;
			this.btnHide.Click += new System.EventHandler(this.btnHide_Click);
			// 
			// btnSettings
			// 
			this.btnSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSettings.Font = new System.Drawing.Font("Liberation Sans", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSettings.ForeColor = System.Drawing.Color.White;
			this.btnSettings.Image = global::MediaManager.Properties.Resources.WrenchIconsmall1;
			this.btnSettings.Location = new System.Drawing.Point(584, 18);
			this.btnSettings.Name = "btnSettings";
			this.btnSettings.Size = new System.Drawing.Size(116, 71);
			this.btnSettings.TabIndex = 14;
			this.btnSettings.UseVisualStyleBackColor = false;
			this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Liberation Sans", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(0, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(494, 72);
			this.label2.TabIndex = 19;
			this.label2.Text = "Media Manager";
			// 
			// panel6
			// 
			this.panel6.BackColor = System.Drawing.Color.Gray;
			this.panel6.Controls.Add(this.panelDownloadManager);
			this.panel6.Location = new System.Drawing.Point(499, 579);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(481, 450);
			this.panel6.TabIndex = 20;
			// 
			// panel7
			// 
			this.panel7.BackColor = System.Drawing.Color.Gray;
			this.panel7.Controls.Add(this.panelVpnManager);
			this.panel7.Location = new System.Drawing.Point(12, 579);
			this.panel7.Name = "panel7";
			this.panel7.Size = new System.Drawing.Size(481, 450);
			this.panel7.TabIndex = 21;
			// 
			// panel8
			// 
			this.panel8.BackColor = System.Drawing.Color.Gray;
			this.panel8.Controls.Add(this.panelQuickActions);
			this.panel8.Location = new System.Drawing.Point(12, 123);
			this.panel8.Name = "panel8";
			this.panel8.Size = new System.Drawing.Size(481, 450);
			this.panel8.TabIndex = 22;
			// 
			// panelShade
			// 
			this.panelShade.BackColor = System.Drawing.Color.Silver;
			this.panelShade.Location = new System.Drawing.Point(982, 1030);
			this.panelShade.Name = "panelShade";
			this.panelShade.Size = new System.Drawing.Size(200, 100);
			this.panelShade.TabIndex = 23;
			this.panelShade.Visible = false;
			// 
			// lblExtIpTimer
			// 
			this.lblExtIpTimer.AutoSize = true;
			this.lblExtIpTimer.Font = new System.Drawing.Font("Liberation Sans", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblExtIpTimer.ForeColor = System.Drawing.Color.Gray;
			this.lblExtIpTimer.Location = new System.Drawing.Point(395, 369);
			this.lblExtIpTimer.Name = "lblExtIpTimer";
			this.lblExtIpTimer.Size = new System.Drawing.Size(38, 27);
			this.lblExtIpTimer.TabIndex = 43;
			this.lblExtIpTimer.Text = "10";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52.33415F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.66585F));
			this.tableLayoutPanel1.Controls.Add(this.label7, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label10, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.label9, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.lblSabUptime, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.label12, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.lblSabState, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.lblVpnState, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.lblVpnUptime, 1, 1);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(15, 66);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(408, 200);
			this.tableLayoutPanel1.TabIndex = 31;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
			this.ClientSize = new System.Drawing.Size(993, 1040);
			this.Controls.Add(this.panelShade);
			this.Controls.Add(this.panel8);
			this.Controls.Add(this.panel7);
			this.Controls.Add(this.panel6);
			this.Controls.Add(this.panel5);
			this.Controls.Add(this.panelStatusBg);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Media Centre Manager";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.panelQuickActions.ResumeLayout(false);
			this.panelQuickActions.PerformLayout();
			this.panelDownloadManager.ResumeLayout(false);
			this.panelDownloadManager.PerformLayout();
			this.panelDownloadItem.ResumeLayout(false);
			this.panelDownloadItem.PerformLayout();
			this.panelVpnManager.ResumeLayout(false);
			this.panelVpnManager.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panelStatus.ResumeLayout(false);
			this.panelStatus.PerformLayout();
			this.statusTableDisk.ResumeLayout(false);
			this.statusTableDisk.PerformLayout();
			this.panelStatusBg.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.panel5.PerformLayout();
			this.panel6.ResumeLayout(false);
			this.panel7.ResumeLayout(false);
			this.panel8.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnRestart;
		private System.Windows.Forms.Label TitleVersion;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnSettings;
		private System.Windows.Forms.Panel panelQuickActions;
		private System.Windows.Forms.Panel panelDownloadManager;
		private System.Windows.Forms.Panel panelVpnManager;
		private System.Windows.Forms.Panel panelStatus;
		private System.Windows.Forms.Label lblVpnState;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lblVpnUptime;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label lblSabState;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label lblSabUptime;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label lblStorage0;
		private System.Windows.Forms.Label lblStorageTitle0;
		private System.Windows.Forms.Label lblStorage1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panelStatusBg;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel7;
		private System.Windows.Forms.Panel panel8;
		private System.Windows.Forms.Button btnHide;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnConfigPriority2;
		private System.Windows.Forms.Button btnAddConfig2;
		private System.Windows.Forms.TextBox tbxConfig2;
		private System.Windows.Forms.Button btnConfigPriority1;
		private System.Windows.Forms.Button btnAddConfig1;
		private System.Windows.Forms.TextBox tbxConfig1;
		private System.Windows.Forms.Button btnConfigPriority0;
		private System.Windows.Forms.Button btnAddConfig0;
		private System.Windows.Forms.TextBox tbxConfig0;
		private System.Windows.Forms.Panel panel9;
		private System.Windows.Forms.Panel panel10;
		private System.Windows.Forms.Button btnToggleVpn;
		private System.Windows.Forms.Panel panel11;
		private System.Windows.Forms.Button btnToggleStreamMode;
		private System.Windows.Forms.Label lblStreamMode;
		private System.Windows.Forms.Panel panelShade;
		private System.Windows.Forms.Button btnDownloadToggle;
		private System.Windows.Forms.Button btnShowDownloads;
		private System.Windows.Forms.Label lblHeadingDownload;
		private System.Windows.Forms.Panel panel12;
		private System.Windows.Forms.Panel panelDownloadItem;
		private System.Windows.Forms.Label lblSubTitle;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Button btnForce;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblDebugMode;
		private System.Windows.Forms.CheckBox btnDownloadPause;
		private System.Windows.Forms.Button btnKillAll;
		private System.Windows.Forms.Button btnVpnPause;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox tbxMyIP;
		private System.Windows.Forms.Panel panel13;
		private System.Windows.Forms.Label lblNoneActive;
		private TableLayoutPanel statusTableDisk;
		private Label lblDisk2;
		private Label lblDisk1;
		private Label lblDisk0;
		private Label lblStorage2;
		private Label lblExtIpTimer;
		private TableLayoutPanel tableLayoutPanel1;
	}
}

