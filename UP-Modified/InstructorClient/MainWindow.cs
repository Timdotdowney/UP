using FileIO;
using InstructorClient.Properties;
using Microsoft.Ink;
using NetworkIO;
using Ookii.Dialogs.Wpf;
using SlideModel;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace InstructorClient
{
	public class MainWindow : Form
	{
		private const bool showInstrInk = false;

		private const WebService.SyncMode syncMode = WebService.SyncMode.Instructor;

		public const string LECTURE_CONNECT_FAILED = "Failed to connect to lecture!";

		public const string LECTURE_DISCONNECT_FAILED = "Failed to disconnect from lecture";

		public const string SYNC_PROMPT = "Would you like to sync to this lecture now?";

		public const string VERSION = "3.3.1";

		public const string REVISION = "$Rev: 462 $";

		public const string DATE = "$Date: 2011-06-09 14:20:32 -0700 (Thu, 09 Jun 2011) $";

		private bool previewingSubs;

		private CustomColors penColors;

		private CustomPenAttributes penAttributes;

		private WebService webService;

		private bool submissionsEnabled;

		private string lectureFileName;

		public string server;

		public string username;

		public string password;

		public string classroom;

		public string lecture;

		private Thread inkThread;

		private object inkThreadLock = new object();

		private Thread updateThread;

		private object updateThreadLock = new object();

		private Thread monitorDetectionThread;

		public static int INK_TOTAL_MS = 1000;

		private static int REFRESH_TOTAL_MS = 1500;

		private static int THREAD_SLEEP_MS = 100;

		private static TimeSpan THREAD_SLEEP_TS = new TimeSpan(0, 0, 0, 0, THREAD_SLEEP_MS);

		private static int MONITOR_SLEEP_MS = 10000;

		private Color default_show_sub_color;

		private Color default_show_sub_text_color;

		private Color new_show_sub_color = Color.Red;

		private Color new_show_sub_text_color = Color.White;

		private string cmdline_filename;

		private static string DEFAULT_WINDOW_TITLE = "Ubiquitous Presenter";

		private IContainer components;

		private RadioButton inkRadio;

		private RadioButton eraseRadio;

		private Button clearButton;

		private ImageList imList0;

		private ImageList imList1;

		private ImageList imList2;

		private ImageList imList3;

		private RadioButton colorRadioBlack;

		private RadioButton colorRadioRed;

		private RadioButton colorRadioGreen;

		private RadioButton colorRadioBlue;

		private Panel colorButtonsPanel;

		private StatusBar statusBar;

		private StatusBarPanel statusBarPanel1;

		private StatusBarPanel slideStatusBarPanel;

		private StatusBarPanel uploadQueueStatus;

		private CheckBox instrInkCheck;

		private RadioButton colorRadioYellow;

		private Button rightPrevButton;

		private Button rightNextButton;

		private Button leftPrevButton;

		private Button leftNextButton;

		private MainMenu mainMenu1;

		private MenuItem menuQuit;

		private MenuItem menuConnect;

		private MenuItem menuImport;

		private Panel colorRadioPanel;

		private Panel inkRadioPanel;

		private MenuItem menuItem4;

		private MenuItem menuDisconnect;

		private MenuItem menuSaveLecture;

		private MenuItem menuExportLecture;

		private MenuItem menuItem3;

		private MenuItem menuOpenLecture;

		private MenuItem fileMenu;

		private MenuItem helpMenu;

		private MenuItem menuItem1;

		private MenuItem menuItem11;

		private NotifyIcon notifyIcon;

		private ContextMenu notifyContextMenu;

		private MenuItem notifyMenuItem;

		private HttpWebRequest versionReq = (HttpWebRequest)WebRequest.Create("http://up.ucsd.edu/download/version.php");

		private Button instrCurButton;

		private MenuItem menuItem6;

		private MenuItem menuItem7;

		private MenuItem menuItem7_1;

		private MenuItem menuItem8;

		private MenuItem menuItem8_1;

		private MenuItem menuItem9;

		private CheckBox minimizeCheck;

		private Button blankSlideButton;

		private Form secondMonitorForm;

		private InkPanel secondMonitorView;

		private Button showHideSubsButton;

		private Button toggleSubmissionsEnabled;

		private SplitContainer mainSplitter;

		private SlideList slideList;

		private InkPanel inkPanel;

		private SubmissionPreviewPanel subPreviewPanel;

		private MenuItem menuItemSync;

		private MenuItem menuItemClose;

		private MenuItem menuItem2;

		private MenuItem menuChangePen;

		private RadioButton thinInkRadio;

		private MenuItem menuItem5;

		private MenuItem menuItemBgColor;

		private MenuItem menuItem10;

		private MenuItem menuItem_subThumbSize;

		private MenuItem changeSlidePreview;

		private MenuItem changeSubPreview;

		private bool SubsEnabled
		{
			get
			{
				return submissionsEnabled;
			}
			set
			{
				submissionsEnabled = value;
				if (submissionsEnabled)
				{
					toggleSubmissionsEnabled.Text = "Disable Submissions";
				}
				else
				{
					toggleSubmissionsEnabled.Text = "Enable Submissions";
				}
			}
		}

		private Slide CurrentSlide => slideList.SelectedSlide;

		public MainWindow(string file_arg)
		{
			cmdline_filename = file_arg;
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, false);
			InitializeComponent();
            menuImport.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
            menuItemClose.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
            menuItemSync.Shortcut = System.Windows.Forms.Shortcut.CtrlK;
            slideList.SelectedIndex = 0;
			setupCustomColors();
			penAttributes = new CustomPenAttributes();
			setStatus("Ubiquitous Presenter 3.3.1");
			setSlideStatus("(No Slide Selected)");
			setUploadQueueStatus("(No Pending Uploads)");
			base.Closing += MainWindow_Closing;
			SlideList expr_CB = slideList;
			expr_CB.SlideChangeDelegate = (UpdateMainSlideDelegate)Delegate.Combine(expr_CB.SlideChangeDelegate, new UpdateMainSlideDelegate(updateMainSlideImage));
			SlideList expr_F2 = slideList;
			expr_F2.SlideChangeDelegate = (UpdateMainSlideDelegate)Delegate.Combine(expr_F2.SlideChangeDelegate, new UpdateMainSlideDelegate(updateMinimizeButton));
			SlideList expr_169 = slideList;
			expr_169.SlideChangeDelegate = (UpdateMainSlideDelegate)Delegate.Combine(expr_169.SlideChangeDelegate, new UpdateMainSlideDelegate(sendChangeSlide));
			SlideList expr_168 = slideList;
			expr_168.SlideChangeDelegate = (UpdateMainSlideDelegate)Delegate.Combine(expr_168.SlideChangeDelegate, new UpdateMainSlideDelegate(updateSlideStatusBar));
			InkPanel expr_167 = inkPanel;
			expr_167.inkChangedDelegate = (InkChangedDelegate)Delegate.Combine(expr_167.inkChangedDelegate, new InkChangedDelegate(slideList.InvalidateCurrentThumbnail));
			minimizeCheck.Click += minimizeCheck_Click;
			mainSplitter.Panel2.Layout += Panel2_Layout;
			mainSplitter.Panel1.Invalidated += MainSplitter_Invalidated;
			inkPanel.BackColor = penColors.getColor(5);
			setupSecondMonitor();
			inkRadio.Checked = true;
			colorRadioBlack.Checked = true;
			setWindowTitle();
			default_show_sub_color = showHideSubsButton.BackColor;
			default_show_sub_text_color = showHideSubsButton.ForeColor;
			setupToolTips();
			notifyContextMenu = new ContextMenu();
			notifyMenuItem = new MenuItem();
			notifyContextMenu.MenuItems.AddRange(new MenuItem[1]
			{
				notifyMenuItem
			});
			notifyMenuItem.Index = 0;
			notifyMenuItem.Text = "Download new version";
			notifyMenuItem.Click += GoToDownloadUpdate;
			notifyIcon = new NotifyIcon();
			notifyIcon.Text = "Ubiquitous Presenter";
			notifyIcon.Visible = true;
			notifyIcon.Icon = Resources.PresenterIcon;
			notifyIcon.ContextMenu = notifyContextMenu;
			notifyIcon.BalloonTipClicked += GoToDownloadUpdate;
			toStateIdle();
		}

		private void setupCustomColors()
		{
			if (penColors == null)
			{
				penColors = new CustomColors();
			}
			colorRadioBlack.BackColor = penColors.getColor(0);
			colorRadioRed.BackColor = penColors.getColor(1);
			colorRadioGreen.BackColor = penColors.getColor(2);
			colorRadioBlue.BackColor = penColors.getColor(3);
			colorRadioYellow.BackColor = penColors.getColor(4);
		}

		private void restoreCurrentPenColor()
		{
		}

		private void setupToolTips()
		{
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(leftPrevButton, "Previous slide");
			toolTip.SetToolTip(leftNextButton, "Next slide");
			toolTip.SetToolTip(instrCurButton, "Go to instructor's current slide");
			toolTip.SetToolTip(minimizeCheck, "Minimize/unminimize this slide");
			toolTip.SetToolTip(inkRadio, "Thick Pen");
			toolTip.SetToolTip(thinInkRadio, "Thin Pen");
			toolTip.SetToolTip(eraseRadio, "Erase mode");
			toolTip.SetToolTip(clearButton, "Clear current slide");
			toolTip.SetToolTip(colorRadioBlack, "Set pen color");
			toolTip.SetToolTip(colorRadioRed, "Set pen color");
			toolTip.SetToolTip(colorRadioGreen, "Set pen color");
			toolTip.SetToolTip(colorRadioBlue, "Set pen color");
			toolTip.SetToolTip(colorRadioYellow, "Set pen color");
			toolTip.SetToolTip(blankSlideButton, "New whiteboard");
			toolTip.SetToolTip(rightPrevButton, "Previous slide");
			toolTip.SetToolTip(rightNextButton, "Next slide");
		}

		private void setupSecondMonitor()
		{
			secondMonitorForm = new Form();
			secondMonitorView = new InkPanel(false);
			secondMonitorView.BackColor = penColors.getColor(5);
			inkPanel.otherPanel = secondMonitorView;
			secondMonitorView.BackColor = inkPanel.BackColor;
			secondMonitorView.BorderStyle = inkPanel.BorderStyle;
			secondMonitorView.Location = inkPanel.Location;
			secondMonitorView.Anchor = inkPanel.Anchor;
			secondMonitorView.Name = "secondMonitorView";
			secondMonitorView.InkEnabled = inkPanel.InkEnabled;
			secondMonitorForm.SuspendLayout();
			secondMonitorForm.AccessibleDescription = "secondMonitorForm";
			secondMonitorForm.AccessibleName = "secondMonitorForm";
			secondMonitorForm.BackColor = Color.White;
			secondMonitorForm.Enabled = false;
			secondMonitorForm.FormBorderStyle = FormBorderStyle.None;
			secondMonitorForm.ShowInTaskbar = false;
			secondMonitorForm.Visible = false;
			secondMonitorForm.Name = "secondMonitorForm";
			secondMonitorForm.Text = "Dual-Monitor Display";
			secondMonitorForm.Controls.Add(secondMonitorView);
			secondMonitorView.BackColor = penColors.getColor(5);
			secondMonitorForm.ResumeLayout();
			monitorDetectionThread = new Thread(monitorDetectionThreadMethod);
		}

		private void setupOtherMonitors()
		{
			Rectangle desktopBounds = default(Rectangle);
			bool flag = false;
			Screen[] allScreens = Screen.AllScreens;
			foreach (Screen screen in allScreens)
			{
				if (!screen.Primary)
				{
					desktopBounds = screen.Bounds;
					flag = true;
					break;
				}
			}
			if (flag && !secondMonitorForm.Visible)
			{
				secondMonitorForm.Visible = true;
				secondMonitorForm.DesktopBounds = desktopBounds;
				secondMonitorView.Size = secondMonitorForm.Size;
				Focus();
			}
			else if (!flag && secondMonitorForm.Visible)
			{
				secondMonitorForm.Visible = false;
				Focus();
			}
		}

		private void monitorDetectionThreadMethod()
		{
			while (true)
			{
				UpdateMonitorsDelegate method = setupOtherMonitors;
				Invoke(method);
				Thread.Sleep(MONITOR_SLEEP_MS);
			}
		}

		private void setStatus(string msg)
		{
			statusBarPanel1.Text = msg;
			statusBar.Invalidate();
		}

		private void setSlideStatus(string msg)
		{
			slideStatusBarPanel.Text = msg;
			statusBar.Invalidate();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(InstructorClient.MainWindow));
			inkRadio = new System.Windows.Forms.RadioButton();
			imList1 = new System.Windows.Forms.ImageList(components);
			eraseRadio = new System.Windows.Forms.RadioButton();
			clearButton = new System.Windows.Forms.Button();
			imList0 = new System.Windows.Forms.ImageList(components);
			imList2 = new System.Windows.Forms.ImageList(components);
			imList3 = new System.Windows.Forms.ImageList(components);
			colorRadioBlack = new System.Windows.Forms.RadioButton();
			colorRadioRed = new System.Windows.Forms.RadioButton();
			colorRadioGreen = new System.Windows.Forms.RadioButton();
			colorRadioBlue = new System.Windows.Forms.RadioButton();
			colorButtonsPanel = new System.Windows.Forms.Panel();
			toggleSubmissionsEnabled = new System.Windows.Forms.Button();
			showHideSubsButton = new System.Windows.Forms.Button();
			blankSlideButton = new System.Windows.Forms.Button();
			minimizeCheck = new System.Windows.Forms.CheckBox();
			instrCurButton = new System.Windows.Forms.Button();
			inkRadioPanel = new System.Windows.Forms.Panel();
			thinInkRadio = new System.Windows.Forms.RadioButton();
			colorRadioPanel = new System.Windows.Forms.Panel();
			colorRadioYellow = new System.Windows.Forms.RadioButton();
			rightPrevButton = new System.Windows.Forms.Button();
			rightNextButton = new System.Windows.Forms.Button();
			leftNextButton = new System.Windows.Forms.Button();
			leftPrevButton = new System.Windows.Forms.Button();
			instrInkCheck = new System.Windows.Forms.CheckBox();
			statusBar = new System.Windows.Forms.StatusBar();
			statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
			slideStatusBarPanel = new System.Windows.Forms.StatusBarPanel();
			uploadQueueStatus = new System.Windows.Forms.StatusBarPanel();
			mainMenu1 = new System.Windows.Forms.MainMenu(components);
			fileMenu = new System.Windows.Forms.MenuItem();
			menuConnect = new System.Windows.Forms.MenuItem();
			menuImport = new System.Windows.Forms.MenuItem();
			menuItemSync = new System.Windows.Forms.MenuItem();
			menuDisconnect = new System.Windows.Forms.MenuItem();
			menuItem3 = new System.Windows.Forms.MenuItem();
			menuOpenLecture = new System.Windows.Forms.MenuItem();
			menuItemClose = new System.Windows.Forms.MenuItem();
			menuSaveLecture = new System.Windows.Forms.MenuItem();
			menuExportLecture = new System.Windows.Forms.MenuItem();
			menuItem4 = new System.Windows.Forms.MenuItem();
			menuQuit = new System.Windows.Forms.MenuItem();
			menuItem6 = new System.Windows.Forms.MenuItem();
			menuItem7 = new System.Windows.Forms.MenuItem();
			menuItem7_1 = new System.Windows.Forms.MenuItem();
			menuItem8 = new System.Windows.Forms.MenuItem();
			menuItem8_1 = new System.Windows.Forms.MenuItem();
			changeSlidePreview = new System.Windows.Forms.MenuItem();
			changeSubPreview = new System.Windows.Forms.MenuItem();
			menuItem_subThumbSize = new System.Windows.Forms.MenuItem();
			menuItem9 = new System.Windows.Forms.MenuItem();
			menuItem2 = new System.Windows.Forms.MenuItem();
			menuChangePen = new System.Windows.Forms.MenuItem();
			menuItem10 = new System.Windows.Forms.MenuItem();
			menuItem5 = new System.Windows.Forms.MenuItem();
			menuItemBgColor = new System.Windows.Forms.MenuItem();
			helpMenu = new System.Windows.Forms.MenuItem();
			menuItem1 = new System.Windows.Forms.MenuItem();
			menuItem11 = new System.Windows.Forms.MenuItem();
			mainSplitter = new System.Windows.Forms.SplitContainer();
			slideList = new InstructorClient.SlideList();
			subPreviewPanel = new InstructorClient.SubmissionPreviewPanel();
			inkPanel = new InstructorClient.InkPanel();
			colorButtonsPanel.SuspendLayout();
			inkRadioPanel.SuspendLayout();
			colorRadioPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)statusBarPanel1).BeginInit();
			((System.ComponentModel.ISupportInitialize)slideStatusBarPanel).BeginInit();
			((System.ComponentModel.ISupportInitialize)uploadQueueStatus).BeginInit();
			mainSplitter.Panel1.SuspendLayout();
			mainSplitter.Panel2.SuspendLayout();
			mainSplitter.SuspendLayout();
			SuspendLayout();
			inkRadio.Appearance = System.Windows.Forms.Appearance.Button;
			inkRadio.ImageIndex = 28;
			inkRadio.ImageList = imList1;
			inkRadio.Location = new System.Drawing.Point(52, 0);
			inkRadio.Name = "inkRadio";
			inkRadio.Size = new System.Drawing.Size(40, 40);
			inkRadio.TabIndex = 2;
			inkRadio.CheckedChanged += new System.EventHandler(inkRadio_CheckedChanged);
			imList1.ImageStream = (System.Windows.Forms.ImageListStreamer)componentResourceManager.GetObject("imList1.ImageStream");
			imList1.TransparentColor = System.Drawing.Color.Transparent;
			imList1.Images.SetKeyName(0, "");
			imList1.Images.SetKeyName(1, "");
			imList1.Images.SetKeyName(2, "");
			imList1.Images.SetKeyName(3, "");
			imList1.Images.SetKeyName(4, "");
			imList1.Images.SetKeyName(5, "");
			imList1.Images.SetKeyName(6, "");
			imList1.Images.SetKeyName(7, "");
			imList1.Images.SetKeyName(8, "");
			imList1.Images.SetKeyName(9, "");
			imList1.Images.SetKeyName(10, "");
			imList1.Images.SetKeyName(11, "");
			imList1.Images.SetKeyName(12, "");
			imList1.Images.SetKeyName(13, "");
			imList1.Images.SetKeyName(14, "");
			imList1.Images.SetKeyName(15, "");
			imList1.Images.SetKeyName(16, "");
			imList1.Images.SetKeyName(17, "");
			imList1.Images.SetKeyName(18, "");
			imList1.Images.SetKeyName(19, "");
			imList1.Images.SetKeyName(20, "");
			imList1.Images.SetKeyName(21, "");
			imList1.Images.SetKeyName(22, "");
			imList1.Images.SetKeyName(23, "");
			imList1.Images.SetKeyName(24, "");
			imList1.Images.SetKeyName(25, "");
			imList1.Images.SetKeyName(26, "");
			imList1.Images.SetKeyName(27, "");
			imList1.Images.SetKeyName(28, "");
			imList1.Images.SetKeyName(29, "");
			imList1.Images.SetKeyName(30, "");
			imList1.Images.SetKeyName(31, "");
			imList1.Images.SetKeyName(32, "");
			imList1.Images.SetKeyName(33, "");
			imList1.Images.SetKeyName(34, "");
			imList1.Images.SetKeyName(35, "");
			imList1.Images.SetKeyName(36, "");
			imList1.Images.SetKeyName(37, "");
			imList1.Images.SetKeyName(38, "");
			imList1.Images.SetKeyName(39, "");
			imList1.Images.SetKeyName(40, "");
			imList1.Images.SetKeyName(41, "");
			imList1.Images.SetKeyName(42, "");
			imList1.Images.SetKeyName(43, "");
			imList1.Images.SetKeyName(44, "");
			imList1.Images.SetKeyName(45, "");
			imList1.Images.SetKeyName(46, "");
			imList1.Images.SetKeyName(47, "");
			imList1.Images.SetKeyName(48, "");
			imList1.Images.SetKeyName(49, "");
			imList1.Images.SetKeyName(50, "");
			eraseRadio.Appearance = System.Windows.Forms.Appearance.Button;
			eraseRadio.ImageIndex = 22;
			eraseRadio.ImageList = imList1;
			eraseRadio.Location = new System.Drawing.Point(98, 0);
			eraseRadio.Name = "eraseRadio";
			eraseRadio.Size = new System.Drawing.Size(40, 40);
			eraseRadio.TabIndex = 3;
			eraseRadio.CheckedChanged += new System.EventHandler(eraseRadio_CheckedChanged);
			clearButton.ImageIndex = 40;
			clearButton.ImageList = imList1;
			clearButton.Location = new System.Drawing.Point(132, 0);
			clearButton.Name = "clearButton";
			clearButton.Size = new System.Drawing.Size(40, 40);
			clearButton.TabIndex = 17;
			clearButton.Click += new System.EventHandler(clearButton_Click);
			imList0.ImageStream = (System.Windows.Forms.ImageListStreamer)componentResourceManager.GetObject("imList0.ImageStream");
			imList0.TransparentColor = System.Drawing.Color.Transparent;
			imList0.Images.SetKeyName(0, "");
			imList0.Images.SetKeyName(1, "");
			imList0.Images.SetKeyName(2, "");
			imList0.Images.SetKeyName(3, "");
			imList0.Images.SetKeyName(4, "");
			imList0.Images.SetKeyName(5, "");
			imList0.Images.SetKeyName(6, "");
			imList0.Images.SetKeyName(7, "");
			imList0.Images.SetKeyName(8, "");
			imList0.Images.SetKeyName(9, "");
			imList0.Images.SetKeyName(10, "");
			imList0.Images.SetKeyName(11, "");
			imList0.Images.SetKeyName(12, "");
			imList0.Images.SetKeyName(13, "");
			imList0.Images.SetKeyName(14, "");
			imList0.Images.SetKeyName(15, "");
			imList0.Images.SetKeyName(16, "");
			imList0.Images.SetKeyName(17, "");
			imList0.Images.SetKeyName(18, "");
			imList0.Images.SetKeyName(19, "");
			imList0.Images.SetKeyName(20, "");
			imList0.Images.SetKeyName(21, "");
			imList0.Images.SetKeyName(22, "");
			imList0.Images.SetKeyName(23, "");
			imList0.Images.SetKeyName(24, "");
			imList0.Images.SetKeyName(25, "");
			imList0.Images.SetKeyName(26, "");
			imList0.Images.SetKeyName(27, "");
			imList0.Images.SetKeyName(28, "");
			imList0.Images.SetKeyName(29, "");
			imList0.Images.SetKeyName(30, "");
			imList0.Images.SetKeyName(31, "");
			imList0.Images.SetKeyName(32, "");
			imList0.Images.SetKeyName(33, "");
			imList0.Images.SetKeyName(34, "");
			imList0.Images.SetKeyName(35, "");
			imList0.Images.SetKeyName(36, "");
			imList0.Images.SetKeyName(37, "");
			imList2.ImageStream = (System.Windows.Forms.ImageListStreamer)componentResourceManager.GetObject("imList2.ImageStream");
			imList2.TransparentColor = System.Drawing.Color.Transparent;
			imList2.Images.SetKeyName(0, "");
			imList2.Images.SetKeyName(1, "");
			imList2.Images.SetKeyName(2, "");
			imList2.Images.SetKeyName(3, "");
			imList2.Images.SetKeyName(4, "");
			imList2.Images.SetKeyName(5, "");
			imList2.Images.SetKeyName(6, "");
			imList2.Images.SetKeyName(7, "");
			imList2.Images.SetKeyName(8, "");
			imList2.Images.SetKeyName(9, "");
			imList2.Images.SetKeyName(10, "");
			imList2.Images.SetKeyName(11, "");
			imList2.Images.SetKeyName(12, "");
			imList2.Images.SetKeyName(13, "");
			imList2.Images.SetKeyName(14, "");
			imList2.Images.SetKeyName(15, "");
			imList2.Images.SetKeyName(16, "");
			imList2.Images.SetKeyName(17, "");
			imList2.Images.SetKeyName(18, "");
			imList2.Images.SetKeyName(19, "");
			imList2.Images.SetKeyName(20, "");
			imList2.Images.SetKeyName(21, "");
			imList2.Images.SetKeyName(22, "");
			imList2.Images.SetKeyName(23, "");
			imList2.Images.SetKeyName(24, "");
			imList2.Images.SetKeyName(25, "");
			imList2.Images.SetKeyName(26, "");
			imList3.ImageStream = (System.Windows.Forms.ImageListStreamer)componentResourceManager.GetObject("imList3.ImageStream");
			imList3.TransparentColor = System.Drawing.Color.Transparent;
			imList3.Images.SetKeyName(0, "");
			imList3.Images.SetKeyName(1, "");
			imList3.Images.SetKeyName(2, "");
			imList3.Images.SetKeyName(3, "");
			colorRadioBlack.Appearance = System.Windows.Forms.Appearance.Button;
			colorRadioBlack.BackColor = System.Drawing.Color.Black;
			colorRadioBlack.ForeColor = System.Drawing.Color.Black;
			colorRadioBlack.Location = new System.Drawing.Point(8, 0);
			colorRadioBlack.Name = "colorRadioBlack";
			colorRadioBlack.Size = new System.Drawing.Size(40, 40);
			colorRadioBlack.TabIndex = 18;
			colorRadioBlack.UseVisualStyleBackColor = false;
			colorRadioBlack.CheckedChanged += new System.EventHandler(colorRadioBlack_CheckedChanged);
			colorRadioRed.Appearance = System.Windows.Forms.Appearance.Button;
			colorRadioRed.BackColor = System.Drawing.Color.Red;
			colorRadioRed.Location = new System.Drawing.Point(48, 0);
			colorRadioRed.Name = "colorRadioRed";
			colorRadioRed.Size = new System.Drawing.Size(40, 40);
			colorRadioRed.TabIndex = 19;
			colorRadioRed.UseVisualStyleBackColor = false;
			colorRadioRed.CheckedChanged += new System.EventHandler(colorRadioRed_CheckedChanged);
			colorRadioGreen.Appearance = System.Windows.Forms.Appearance.Button;
			colorRadioGreen.BackColor = System.Drawing.Color.Green;
			colorRadioGreen.Location = new System.Drawing.Point(88, 0);
			colorRadioGreen.Name = "colorRadioGreen";
			colorRadioGreen.Size = new System.Drawing.Size(40, 40);
			colorRadioGreen.TabIndex = 20;
			colorRadioGreen.UseVisualStyleBackColor = false;
			colorRadioGreen.CheckedChanged += new System.EventHandler(colorRadioGreen_CheckedChanged);
			colorRadioBlue.Appearance = System.Windows.Forms.Appearance.Button;
			colorRadioBlue.BackColor = System.Drawing.Color.Blue;
			colorRadioBlue.ImageIndex = 4;
			colorRadioBlue.Location = new System.Drawing.Point(128, 0);
			colorRadioBlue.Name = "colorRadioBlue";
			colorRadioBlue.Size = new System.Drawing.Size(40, 40);
			colorRadioBlue.TabIndex = 21;
			colorRadioBlue.UseVisualStyleBackColor = false;
			colorRadioBlue.CheckedChanged += new System.EventHandler(colorRadioBlue_CheckedChanged);
			colorButtonsPanel.Controls.Add(toggleSubmissionsEnabled);
			colorButtonsPanel.Controls.Add(showHideSubsButton);
			colorButtonsPanel.Controls.Add(blankSlideButton);
			colorButtonsPanel.Controls.Add(minimizeCheck);
			colorButtonsPanel.Controls.Add(instrCurButton);
			colorButtonsPanel.Controls.Add(inkRadioPanel);
			colorButtonsPanel.Controls.Add(colorRadioPanel);
			colorButtonsPanel.Controls.Add(rightPrevButton);
			colorButtonsPanel.Controls.Add(rightNextButton);
			colorButtonsPanel.Controls.Add(clearButton);
			colorButtonsPanel.Controls.Add(leftNextButton);
			colorButtonsPanel.Controls.Add(leftPrevButton);
			colorButtonsPanel.Controls.Add(instrInkCheck);
			colorButtonsPanel.Dock = System.Windows.Forms.DockStyle.Top;
			colorButtonsPanel.Location = new System.Drawing.Point(0, 0);
			colorButtonsPanel.Name = "colorButtonsPanel";
			colorButtonsPanel.Size = new System.Drawing.Size(804, 40);
			colorButtonsPanel.TabIndex = 22;
			toggleSubmissionsEnabled.Enabled = false;
			toggleSubmissionsEnabled.Location = new System.Drawing.Point(642, 0);
			toggleSubmissionsEnabled.Name = "toggleSubmissionsEnabled";
			toggleSubmissionsEnabled.Size = new System.Drawing.Size(81, 40);
			toggleSubmissionsEnabled.TabIndex = 36;
			toggleSubmissionsEnabled.Text = "Enable Submissions";
			toggleSubmissionsEnabled.UseVisualStyleBackColor = true;
			toggleSubmissionsEnabled.Click += new System.EventHandler(toggleSubmissionsEnabled_Click);
			showHideSubsButton.ImageList = imList0;
			showHideSubsButton.Location = new System.Drawing.Point(729, 0);
			showHideSubsButton.Name = "showHideSubsButton";
			showHideSubsButton.Size = new System.Drawing.Size(85, 40);
			showHideSubsButton.TabIndex = 35;
			showHideSubsButton.Text = "Show Submissions";
			showHideSubsButton.Click += new System.EventHandler(showHideSubsButton_Click);
			blankSlideButton.ImageIndex = 3;
			blankSlideButton.ImageList = imList0;
			blankSlideButton.Location = new System.Drawing.Point(584, 0);
			blankSlideButton.Name = "blankSlideButton";
			blankSlideButton.Size = new System.Drawing.Size(40, 40);
			blankSlideButton.TabIndex = 34;
			blankSlideButton.Click += new System.EventHandler(noteButton_Click);
			minimizeCheck.AccessibleDescription = "";
			minimizeCheck.Appearance = System.Windows.Forms.Appearance.Button;
			minimizeCheck.Location = new System.Drawing.Point(86, 0);
			minimizeCheck.Name = "minimizeCheck";
			minimizeCheck.Size = new System.Drawing.Size(41, 40);
			minimizeCheck.TabIndex = 33;
			minimizeCheck.Text = "Min";
			minimizeCheck.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			minimizeCheck.CheckedChanged += new System.EventHandler(minimizeCheck_CheckedChanged);
			instrCurButton.Location = new System.Drawing.Point(86, 0);
			instrCurButton.Name = "instrCurButton";
			instrCurButton.Size = new System.Drawing.Size(39, 40);
			instrCurButton.TabIndex = 32;
			instrCurButton.Text = "Cur";
			instrCurButton.Visible = false;
			instrCurButton.Click += new System.EventHandler(instrCurButton_Click);
			inkRadioPanel.Controls.Add(thinInkRadio);
			inkRadioPanel.Controls.Add(eraseRadio);
			inkRadioPanel.Controls.Add(inkRadio);
			inkRadioPanel.Location = new System.Drawing.Point(196, 0);
			inkRadioPanel.Name = "inkRadioPanel";
			inkRadioPanel.Size = new System.Drawing.Size(141, 40);
			inkRadioPanel.TabIndex = 31;
			thinInkRadio.Appearance = System.Windows.Forms.Appearance.Button;
			thinInkRadio.ImageIndex = 28;
			thinInkRadio.ImageList = imList1;
			thinInkRadio.Location = new System.Drawing.Point(6, 0);
			thinInkRadio.Name = "thinInkRadio";
			thinInkRadio.Size = new System.Drawing.Size(40, 40);
			thinInkRadio.TabIndex = 4;
			thinInkRadio.CheckedChanged += new System.EventHandler(thinInkRadio_CheckedChanged);
			colorRadioPanel.Controls.Add(colorRadioGreen);
			colorRadioPanel.Controls.Add(colorRadioBlack);
			colorRadioPanel.Controls.Add(colorRadioBlue);
			colorRadioPanel.Controls.Add(colorRadioYellow);
			colorRadioPanel.Controls.Add(colorRadioRed);
			colorRadioPanel.Location = new System.Drawing.Point(352, 0);
			colorRadioPanel.Name = "colorRadioPanel";
			colorRadioPanel.Size = new System.Drawing.Size(216, 48);
			colorRadioPanel.TabIndex = 30;
			colorRadioYellow.Appearance = System.Windows.Forms.Appearance.Button;
			colorRadioYellow.BackColor = System.Drawing.Color.Yellow;
			colorRadioYellow.ImageIndex = 25;
			colorRadioYellow.Location = new System.Drawing.Point(168, 0);
			colorRadioYellow.Name = "colorRadioYellow";
			colorRadioYellow.Size = new System.Drawing.Size(40, 40);
			colorRadioYellow.TabIndex = 22;
			colorRadioYellow.UseVisualStyleBackColor = false;
			colorRadioYellow.CheckedChanged += new System.EventHandler(colorRadioYellow_CheckedChanged);
			rightPrevButton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			rightPrevButton.ImageIndex = 2;
			rightPrevButton.ImageList = imList1;
			rightPrevButton.Location = new System.Drawing.Point(724, 0);
			rightPrevButton.Name = "rightPrevButton";
			rightPrevButton.Size = new System.Drawing.Size(40, 40);
			rightPrevButton.TabIndex = 26;
			rightPrevButton.Click += new System.EventHandler(rightPrevButton_Click);
			rightNextButton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			rightNextButton.ImageIndex = 5;
			rightNextButton.ImageList = imList1;
			rightNextButton.Location = new System.Drawing.Point(764, 0);
			rightNextButton.Name = "rightNextButton";
			rightNextButton.Size = new System.Drawing.Size(40, 40);
			rightNextButton.TabIndex = 27;
			rightNextButton.Click += new System.EventHandler(rightNextButton_Click);
			leftNextButton.ImageIndex = 5;
			leftNextButton.ImageList = imList1;
			leftNextButton.Location = new System.Drawing.Point(40, 0);
			leftNextButton.Name = "leftNextButton";
			leftNextButton.Size = new System.Drawing.Size(40, 40);
			leftNextButton.TabIndex = 29;
			leftNextButton.Click += new System.EventHandler(leftNextButton_Click);
			leftPrevButton.ImageIndex = 2;
			leftPrevButton.ImageList = imList1;
			leftPrevButton.Location = new System.Drawing.Point(0, 0);
			leftPrevButton.Name = "leftPrevButton";
			leftPrevButton.Size = new System.Drawing.Size(40, 40);
			leftPrevButton.TabIndex = 28;
			leftPrevButton.Click += new System.EventHandler(leftPrevButton_Click);
			instrInkCheck.Checked = true;
			instrInkCheck.CheckState = System.Windows.Forms.CheckState.Checked;
			instrInkCheck.Location = new System.Drawing.Point(584, 8);
			instrInkCheck.Name = "instrInkCheck";
			instrInkCheck.Size = new System.Drawing.Size(88, 24);
			instrInkCheck.TabIndex = 25;
			instrInkCheck.Text = "Instructor Ink";
			instrInkCheck.Visible = false;
			instrInkCheck.CheckedChanged += new System.EventHandler(instrInkCheck_CheckedChanged);
			statusBar.Location = new System.Drawing.Point(0, 476);
			statusBar.Name = "statusBar";
			statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[3]
			{
				slideStatusBarPanel,
				statusBarPanel1,
				uploadQueueStatus
			});
			statusBar.ShowPanels = true;
			statusBar.Size = new System.Drawing.Size(804, 24);
			statusBar.TabIndex = 24;
			statusBarPanel1.Name = "statusBarPanel1";
			statusBarPanel1.Width = 200;
			slideStatusBarPanel.Name = "slideStatusBarPanel";
			slideStatusBarPanel.Width = 200;
			uploadQueueStatus.Name = "statusBarPanel1";
			uploadQueueStatus.Width = 200;
			mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[4]
			{
				fileMenu,
				menuItem6,
				menuItem2,
				helpMenu
			});
			fileMenu.Index = 0;
			fileMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[11]
			{
				menuConnect,
				menuImport,
				menuItemSync,
				menuDisconnect,
				menuItem3,
				menuOpenLecture,
				menuItemClose,
				menuSaveLecture,
				menuExportLecture,
				menuItem4,
				menuQuit
			});
			fileMenu.Text = "File";
			menuConnect.Index = 0;
			menuConnect.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
			menuConnect.Text = "Download Lecture or Create Whiteboard Lecture";
			menuConnect.Click += new System.EventHandler(menuConnect_Click);
			menuImport.Index = 1;
			menuImport.Text = "Import Lecture...";
			menuImport.Click += new System.EventHandler(menuImport_Click);
			menuItemSync.Enabled = false;
			menuItemSync.Index = 2;
			menuItemSync.Text = "Sync to Open Lecture...";
			menuItemSync.Click += new System.EventHandler(menuItemSync_Click);
			menuDisconnect.Enabled = false;
			menuDisconnect.Index = 3;
			menuDisconnect.Shortcut = System.Windows.Forms.Shortcut.CtrlD;
			menuDisconnect.Text = "Disconnect";
			menuDisconnect.Click += new System.EventHandler(menuDisconnect_Click);
			menuItem3.Index = 4;
			menuItem3.Text = "-";
			menuOpenLecture.Index = 5;
			menuOpenLecture.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			menuOpenLecture.Text = "Open Lecture...";
			menuOpenLecture.Click += new System.EventHandler(menuOpenLecture_Click);
			menuItemClose.Enabled = false;
			menuItemClose.Index = 6;
			menuItemClose.Text = "Close Lecture";
			menuItemClose.Click += new System.EventHandler(menuItemClose_Click);
			menuSaveLecture.Enabled = false;
			menuSaveLecture.Index = 7;
			menuSaveLecture.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			menuSaveLecture.Text = "Save Lecture";
			menuSaveLecture.Click += new System.EventHandler(menuItem2_Click);
			menuExportLecture.Enabled = false;
			menuExportLecture.Index = 8;
			menuExportLecture.Text = "Export Lecture...";
			menuExportLecture.Click += new System.EventHandler(menuExportLecture_Click);
			menuItem4.Index = 9;
			menuItem4.Text = "-";
			menuQuit.Index = 10;
			menuQuit.Shortcut = System.Windows.Forms.Shortcut.CtrlQ;
			menuQuit.Text = "Exit";
			menuQuit.Click += new System.EventHandler(menuQuit_Click);
			menuItem6.Index = 1;
			menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[8]
			{
				menuItem7,
				menuItem7_1,
				menuItem8,
				menuItem8_1,
				changeSlidePreview,
				menuItem_subThumbSize,
				changeSubPreview,
				menuItem9
			});
			menuItem6.Text = "Slides";
			menuItem7.Index = 0;
			menuItem7.Shortcut = System.Windows.Forms.Shortcut.AltDownArrow;
			menuItem7.Text = "Next Slide";
			menuItem7.Click += new System.EventHandler(menuItem7_Click);
			menuItem7_1.Visible = false;
			menuItem7_1.Shortcut = System.Windows.Forms.Shortcut.AltRightArrow;
			menuItem7_1.Click += new System.EventHandler(menuItem7_Click);
			menuItem8.Index = 1;
			menuItem8.Shortcut = System.Windows.Forms.Shortcut.AltUpArrow;
			menuItem8.Text = "Previous Slide";
			menuItem8.Click += new System.EventHandler(menuItem8_Click);
			menuItem8_1.Visible = false;
			menuItem8_1.Shortcut = System.Windows.Forms.Shortcut.AltLeftArrow;
			menuItem8_1.Click += new System.EventHandler(menuItem8_Click);
			changeSlidePreview.Index = 2;
			changeSlidePreview.Text = "Slide Preview Size...";
			changeSlidePreview.Click += new System.EventHandler(changeSlidePreview_Click);
			changeSubPreview.Index = 4;
			changeSubPreview.Text = "Submission Preview Size...";
			changeSubPreview.Click += new System.EventHandler(changeSubPreview_Click);
			menuItem_subThumbSize.Index = 3;
			menuItem_subThumbSize.Text = "Submission Thumb Size...";
			menuItem_subThumbSize.Click += new System.EventHandler(menuItem_subThumbSize_Click);
			menuItem9.Index = 5;
			menuItem9.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
			menuItem9.Text = "Current Instructor Slide";
			menuItem9.Visible = false;
			menuItem9.Click += new System.EventHandler(menuItem9_Click);
			menuItem2.Index = 2;
			menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[4]
			{
				menuChangePen,
				menuItem10,
				menuItem5,
				menuItemBgColor
			});
			menuItem2.Text = "Ink";
			menuChangePen.Index = 0;
			menuChangePen.Text = "Current Pen Color...";
			menuChangePen.Click += new System.EventHandler(menuChangePen_Click);
			menuItem10.Index = 1;
			menuItem10.Text = "Restore Default Pen Color";
			menuItem10.Click += new System.EventHandler(menuItem10_Click_2);
			menuItem5.Index = 2;
			menuItem5.Text = "Current Ink Style...";
			menuItem5.Click += new System.EventHandler(menuItem5_Click);
			menuItemBgColor.Index = 3;
			menuItemBgColor.Text = "Background Color...";
			menuItemBgColor.Click += new System.EventHandler(menuItem10_Click_1);
			helpMenu.Index = 3;
			helpMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[2]
			{
				menuItem11,
				menuItem1
			});
			helpMenu.Text = "Help";
			menuItem11.Index = 0;
			menuItem11.Text = "Projection FAQ";
			menuItem11.Click += new System.EventHandler(menuItem11_Click);
			menuItem1.Index = 1;
			menuItem1.Text = "About NoteTaker...";
			menuItem1.Click += new System.EventHandler(menuItem1_Click);
			mainSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
			mainSplitter.Location = new System.Drawing.Point(0, 40);
			mainSplitter.Name = "mainSplitter";
			mainSplitter.Panel1.Controls.Add(slideList);
			mainSplitter.Panel2.Controls.Add(subPreviewPanel);
			mainSplitter.Panel2.Controls.Add(inkPanel);
			mainSplitter.Size = new System.Drawing.Size(804, 436);
			mainSplitter.SplitterDistance = 156;
			mainSplitter.TabIndex = 25;
			slideList.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
			slideList.AutoScrollMargin = new System.Drawing.Size(1, 1);
			slideList.AutoScrollMinSize = new System.Drawing.Size(1, 1);
			slideList.BackColor = System.Drawing.SystemColors.Control;
			slideList.Location = new System.Drawing.Point(4, 7);
			slideList.Margin = new System.Windows.Forms.Padding(2);
			slideList.Name = "slideList";
			slideList.PreviousSlideHadSubmissions = false;
			slideList.SelectedIndex = 0;
			slideList.Size = new System.Drawing.Size(150, 424);
			slideList.SlideChangeDelegate = null;
			slideList.TabIndex = 0;
			slideList.Paint += new System.Windows.Forms.PaintEventHandler(slideList_Paint);
			subPreviewPanel.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
			subPreviewPanel.AutoScroll = true;
			subPreviewPanel.Location = new System.Drawing.Point(0, 0);
			subPreviewPanel.Name = "subPreviewPanel";
			subPreviewPanel.Size = new System.Drawing.Size(515, 640);
			subPreviewPanel.TabIndex = 0;
			subPreviewPanel.Visible = false;
			inkPanel.Color = System.Drawing.Color.Black;
			inkPanel.InkEditingMode = Microsoft.Ink.InkOverlayEditingMode.Ink;
			inkPanel.InkEnabled = false;
			inkPanel.Location = new System.Drawing.Point(0, 0);
			inkPanel.Name = "inkPanel";
			inkPanel.Size = new System.Drawing.Size(200, 100);
			inkPanel.Slide = null;
			inkPanel.TabIndex = 1;
			AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			base.ClientSize = new System.Drawing.Size(804, 500);
			base.Controls.Add(mainSplitter);
			base.Controls.Add(statusBar);
			base.Controls.Add(colorButtonsPanel);
			base.Menu = mainMenu1;
			MinimumSize = new System.Drawing.Size(400, 300);
			base.Name = "MainWindow";
			Text = "NoteTaker";
			base.Shown += new System.EventHandler(MainWindow_Load);
			colorButtonsPanel.ResumeLayout(false);
			inkRadioPanel.ResumeLayout(false);
			colorRadioPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)statusBarPanel1).EndInit();
			((System.ComponentModel.ISupportInitialize)slideStatusBarPanel).EndInit();
			((System.ComponentModel.ISupportInitialize)uploadQueueStatus).EndInit();
			mainSplitter.Panel1.ResumeLayout(false);
			mainSplitter.Panel2.ResumeLayout(false);
			mainSplitter.ResumeLayout(false);
			ResumeLayout(false);
		}

		private void changeSubPreview_Click(object sender, EventArgs e)
		{
			SlidePreviewPanel.changeSubmissionSize();
		}

		private void MainWindow_Closing(object sender, CancelEventArgs args)
		{
			if (!closeLecture())
			{
				args.Cancel = true;
			}
			else
			{
				if (monitorDetectionThread != null)
				{
					monitorDetectionThread.Abort();
				}
				secondMonitorForm.Dispose();
				Application.Exit();
			}
		}

		private void eraseRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (eraseRadio.Checked)
			{
				inkPanel.InkEditingMode = InkOverlayEditingMode.Delete;
				secondMonitorView.InkEditingMode = InkOverlayEditingMode.Delete;
				eraseRadio.ImageIndex = 21;
				enableColorButtons(false);
			}
			else
			{
				eraseRadio.ImageIndex = 22;
			}
		}

		private void inkRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (inkRadio.Checked)
			{
				inkPanel.InkEditingMode = InkOverlayEditingMode.Ink;
				secondMonitorView.InkEditingMode = InkOverlayEditingMode.Ink;
				inkPanel.PenAttributes = penAttributes.getPen(1);
				secondMonitorView.PenAttributes = penAttributes.getPen(1);
				inkRadio.ImageIndex = 27;
				enableColorButtons(true);
			}
			else
			{
				inkRadio.ImageIndex = 28;
			}
		}

		private void remoteOpenButton_Click(object sender, EventArgs e)
		{
			connectToLecture();
		}

		private void submission_DoubleClick(object sender, EventArgs e)
		{
			SubmissionThumb submissionThumb = (SubmissionThumb)sender;
			addSubmissionToSlideList(submissionThumb.Sub);
			toggleSubmissionPreview();
		}

		private void clearButton_Click(object sender, EventArgs e)
		{
			newNoteOnCurrentSlide(true);
		}

		private void MainWindow_Load(object sender, EventArgs e)
		{
			startupActions();
		}

		private void colorRadioBlack_CheckedChanged(object sender, EventArgs e)
		{
			DoColorButton(0);
		}

		private void colorRadioRed_CheckedChanged(object sender, EventArgs e)
		{
			DoColorButton(1);
		}

		private void colorRadioGreen_CheckedChanged(object sender, EventArgs e)
		{
			DoColorButton(2);
		}

		private void colorRadioBlue_CheckedChanged(object sender, EventArgs e)
		{
			DoColorButton(3);
		}

		private void colorRadioYellow_CheckedChanged(object sender, EventArgs e)
		{
			DoColorButton(4);
		}

		private void menuItemSync_Click(object sender, EventArgs e)
		{
			syncToCurrentLecture();
		}

		private void menuItemClose_Click(object sender, EventArgs e)
		{
			closeLecture();
		}

		private void slideList_Paint(object sender, PaintEventArgs e)
		{
		}

		private void statusText_Click(object sender, EventArgs e)
		{
		}

		private void instrInkCheck_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void rightPrevButton_Click(object sender, EventArgs e)
		{
			prevSlide();
		}

		private void rightNextButton_Click(object sender, EventArgs e)
		{
			nextSlide();
		}

		private void leftPrevButton_Click(object sender, EventArgs e)
		{
			prevSlide();
		}

		private void leftNextButton_Click(object sender, EventArgs e)
		{
			nextSlide();
		}

		private void menuConnect_Click(object sender, EventArgs e)
		{
			connectToLecture();
		}

		private void menuImport_Click(object sender, EventArgs e)
		{
			importLecture(getSlideImagesFromPNGs());
		}

		private void menuDisconnect_Click(object sender, EventArgs e)
		{
			Disconnect();
		}

		private void menuItem10_Click(object sender, EventArgs e)
		{
			saveCurrentLecture();
			Reconnect();
		}

		private void menuQuit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void menuItem2_Click(object sender, EventArgs e)
		{
			setStatus("Saving...");
			saveCurrentLecture();
			setStatus("Saved.");
		}

		private void menuExportLecture_Click(object sender, EventArgs e)
		{
			MainWindowData mainWindowData = new MainWindowData(server, username, classroom, lecture, slideList.Slides, lectureFileName);
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.OverwritePrompt = true;
			saveFileDialog.ValidateNames = true;
			saveFileDialog.Filter = "PDF Documents (*.pdf)|*.pdf";
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				mainWindowData.FileName = saveFileDialog.FileName;
				ExportProgress exportProgress = new ExportProgress();
				exportProgress.export(mainWindowData);
			}
		}

		private void menuItem1_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "NoteTaker version 3.3.1\n\nBuild information:\n$Rev: 462 $\n$Date: 2011-06-09 14:20:32 -0700 (Thu, 09 Jun 2011) $");
		}

		private void menuItem11_Click(object sender, EventArgs e)
		{
			Process.Start("http://up.ucsd.edu/about/projectionFAQ.html");
		}

		private void showHideSubsButton_Click(object sender, EventArgs e)
		{
			showHideSubsButton.BackColor = default_show_sub_color;
			showHideSubsButton.ForeColor = default_show_sub_text_color;
			toggleSubmissionPreview();
		}

		private void toggleSubmissionsEnabled_Click(object sender, EventArgs e)
		{
			if (webService != null)
			{
				if (SubsEnabled)
				{
					SubsEnabled = false;
					webService.asyncEndSubmissions();
				}
				else
				{
					SubsEnabled = true;
					webService.asyncAllowSubmissionsOnAll();
				}
			}
		}

		private void Panel2_Layout(object sender, EventArgs e)
		{
			float num7 = (float)mainSplitter.Panel2.Width;
			float num6 = (float)mainSplitter.Panel2.Height;
			if (num7 / num6 > 1.302799f)
			{
				num7 = num6 * 1.302799f;
			}
			else
			{
				num6 = num7 / 1.302799f;
			}
			int num5 = (int)Math.Ceiling((double)num7);
			int num4 = (int)Math.Ceiling((double)num6);
			inkPanel.Left = (mainSplitter.Panel2.Width - num5) / 2;
			inkPanel.Top = (mainSplitter.Panel2.Height - num4) / 2;
			inkPanel.Size = new Size(num5, num4);
			subPreviewPanel.Width = mainSplitter.Panel2.Width;
			subPreviewPanel.Height = mainSplitter.Panel2.Height;
		}

		private void instrCurButton_Click(object sender, EventArgs e)
		{
			goToInstructorSlide();
		}

		private void menuItem9_Click(object sender, EventArgs e)
		{
			goToInstructorSlide();
		}

		private void menuItem7_Click(object sender, EventArgs e)
		{
			nextSlide();
		}

		private void menuItem8_Click(object sender, EventArgs e)
		{
			prevSlide();
		}

		private void minimizeCheck_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void minimizeCheck_Click(object sender, EventArgs e)
		{
			if (slideList != null && slideList.SelectedSlide != null)
			{
				if (slideList.SelectedSlide.Minimized)
				{
					slideList.SelectedSlide.Minimized = false;
					minimizeCheck.Checked = false;
				}
				else
				{
					slideList.SelectedSlide.Minimized = true;
					minimizeCheck.Checked = true;
				}
				inkPanel.ScaleInk();
				inkPanel.otherPanel.ScaleInk();
			}
		}

		private void noteButton_Click(object sender, EventArgs e)
		{
			newNoteOnCurrentSlide();
		}

		[STAThread]
		private static void Main(string[] args)
		{
			try
			{
				Application.EnableVisualStyles();
				Application.Run(new MainWindow((args.Length > 0) ? args[0] : null));
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
		}

		public bool saveCurrentLecture()
		{
			for (int i = 0; i < slideList.Slides.Count; i++)
			{
				((Slide)slideList.Slides[i]).serializeInk();
			}
			return LocalFileService.SaveSlides(new MainWindowData(server, username, classroom, lecture, slideList.Slides, lectureFileName));
		}

		public void startLocalLecture(string filename)
		{
			closeLecture();
			MainWindowData mainWindowData = new MainWindowData();
			mainWindowData.FileName = filename;
			if (LocalFileService.LoadSlides(mainWindowData))
			{
				server = mainWindowData.Server;
				username = mainWindowData.Username;
				classroom = mainWindowData.Classroom;
				lecture = mainWindowData.Lecture;
				lectureFileName = mainWindowData.FileName;
				for (int i = 0; i < mainWindowData.SlideList.Count; i++)
				{
					((Slide)mainWindowData.SlideList[i]).deserializeInk();
				}
				slideList.setSlideList(mainWindowData.SlideList);
				if (server != null && classroom != null && username != null && lecture != null)
				{
					webService = WebService.webServiceForLocalLecture(setUploadQueueStatus, server, username, classroom, lecture);
					startInkThread();
				}
				setWindowTitle();
				toStateOffline();
				if (webService != null && MessageBox.Show("Would you like to sync to this lecture now?", DEFAULT_WINDOW_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					syncToCurrentLecture();
				}
			}
		}

		public void syncToCurrentLecture()
		{
			if (webService == null)
			{
				MessageBox.Show("There is no lecture open, or the open lecture has not been uploaded to the server!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else if (webService.syncToLecture())
			{
				startUpdateThread();
				toStateOnline();
			}
		}

		public bool connectToLecture()
		{
			if (this.webService != null)
			{
				closeLecture();
			}
			WebService webService = WebService.connectNewWebServiceToLecture(setUploadQueueStatus, WebService.SyncMode.Instructor);
			if (webService == null)
			{
				return false;
			}
			slideList.Clear();
			this.webService = webService;
			server = this.webService.BaseURL;
			username = this.webService.UserName;
			password = this.webService.Password;
			classroom = this.webService.Classroom;
			lecture = this.webService.Lecture;
			updateSlideList();
			startInkThread();
			startUpdateThread();
			setWindowTitle();
			toStateOnline();
			return true;
		}

		public ArrayList getSlideImagesFromPNGs()
		{
			string[] array = null;
			while (array == null || array.Length < 1)
			{
				if (array != null)
				{
					MessageBox.Show("That folder doesn't contain any PNG images. Please select a different folder.");
				}
				VistaFolderBrowserDialog vistaFolderBrowserDialog = new VistaFolderBrowserDialog();
				vistaFolderBrowserDialog.Description = "Select the folder containing your slides as PNG images";
				vistaFolderBrowserDialog.UseDescriptionForTitle = true;
				bool? flag = vistaFolderBrowserDialog.ShowDialog();
				if (!flag.HasValue || (flag.HasValue && !flag.Value))
				{
					return null;
				}
				array = Directory.GetFiles(vistaFolderBrowserDialog.SelectedPath, "*.png", SearchOption.TopDirectoryOnly);
			}
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < array.Length; i++)
			{
				Image image = new Bitmap(array[i]);
				Slide value = new Slide(i, string.Empty, string.Empty, image, image);
				arrayList.Add(value);
			}
			return arrayList;
		}

		public bool importLecture(ArrayList slides)
		{
			if (slides != null)
			{
				if (this.webService != null)
				{
					closeLecture();
				}
				WebService webService = WebService.uploadLecture(slides);
				if (webService != null)
				{
					MainWindowData mainWindowData = new MainWindowData(webService.BaseURL, webService.UserName, webService.Classroom, webService.Lecture, slides, null);
					LocalFileService.SaveSlides(mainWindowData);
					if (mainWindowData.FileName != null)
					{
						startLocalLecture(mainWindowData.FileName);
						return true;
					}
					return false;
				}
				return false;
			}
			return false;
		}

		public void Reconnect()
		{
			Disconnect();
			connectToLecture();
		}

		private void Disconnect()
		{
			if (webService != null)
			{
				webService.unSyncFromLecture();
			}
			stopUpdateThread();
			if (previewingSubs)
			{
				toggleSubmissionPreview();
			}
			toStateOffline();
			setWindowTitle();
		}

		private bool closeLecture()
		{
			if (webService != null)
			{
                if (webService.uploadQueueIsEmpty() || MessageBox.Show("There are pending upload operations that will be lost if you close this lecture. Are you sure you want to continue?", "Pending Uploads", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
                    bool close = false;
					switch (MessageBox.Show("Would you like to save your current lecture?", "Closing Lecture...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
					{
					case DialogResult.Cancel:
                        return false;
					case DialogResult.Yes:
						close = saveCurrentLecture();
						break;
					case DialogResult.No:
						close = true;
						break;
					}
					if (close)
					{
						Disconnect();
						stopInkThread();
						inkPanel.Clear();
						slideList.Clear();
						server = null;
						classroom = null;
						lecture = null;
						username = null;
						lectureFileName = null;
						webService = null;
						toStateIdle();
						setSlideStatus("(No Slide Selected)");
						setUploadQueueStatus("(No Pending Uploads)");
						return true;
					}
					return false;
				}
				return false;
			}
			return true;
		}

		private void getSubmissions()
		{
			if (webService != null)
			{
				bool flag = false;
				string[] submissionsList = webService.getSubmissionsList();
				double num3 = 0.0;
				string[] array3 = submissionsList;
				foreach (string text in array3)
				{
					if (!(text == ""))
					{
						string[] array2 = text.Split('\t');
						Slide slide3 = null;
						string id3 = array2[0];
						string type = array2[1];
						string id2 = array2[2];
						double num2 = double.Parse(array2[3]);
						if (num2 > num3)
						{
							num3 = num2;
						}
						foreach (Slide slide4 in slideList.Slides)
						{
							if (slide4.isSameSlide(type, id2))
							{
								slide3 = slide4;
							}
						}
						if (slide3 == null)
						{
							return;
						}
						Submission submission = new Submission(submission_DoubleClick, slide3, null, id3, num2);
						if (slide3.AddSubmission(submission))
						{
							webService.asyncDownloadImage(submission.getAttrs(), submission.SetImage, false);
							flag = true;
						}
					}
				}
				if (flag)
				{
					subPreviewPanel.SubmissionThumbList = CurrentSlide.SubmissionList;
					if (!subPreviewPanel.Visible && num3 >= webService.LectureSyncTime)
					{
						showHideSubsButton.ForeColor = new_show_sub_text_color;
						showHideSubsButton.BackColor = new_show_sub_color;
					}
				}
			}
		}

		private void toggleSubmissionPreview()
		{
			if (CurrentSlide != null)
			{
				if (previewingSubs)
				{
					inkPanel.BringToFront();
					subPreviewPanel.Visible = false;
					previewingSubs = false;
					showHideSubsButton.Text = "Show Submissions";
				}
				else
				{
					if (slideList.PreviousSlideHadSubmissions)
					{
						slideList.gotoPreviousSlide();
					}
					inkPanel.SendToBack();
					subPreviewPanel.Visible = true;
					subPreviewPanel.SubmissionThumbList = CurrentSlide.SubmissionList;
					previewingSubs = true;
					showHideSubsButton.Text = "Hide Submissions";
				}
			}
		}

		private void startInkThread()
		{
			lock (inkThreadLock)
			{
				if (inkThread != null)
				{
					inkThread.Abort();
				}
				inkThread = new Thread(inkThreadMethod);
				inkThread.Name = "InkThread";
				inkThread.Start();
			}
		}

		private void stopInkThread()
		{
			lock (inkThreadLock)
			{
				if (inkThread != null)
				{
					inkThread.Abort();
					inkThread = null;
				}
			}
		}

		private void startUpdateThread()
		{
			lock (updateThreadLock)
			{
				if (updateThread == null)
				{
					updateThread = new Thread(updateThreadMethod);
					updateThread.Name = "UpdateThread";
					updateThread.Start();
				}
			}
		}

		private void stopUpdateThread()
		{
			lock (updateThreadLock)
			{
				if (updateThread != null)
				{
					updateThread.Abort();
					updateThread = null;
				}
			}
		}

		private void inkThreadMethod()
		{
			while (true)
			{
				Thread.Sleep(new TimeSpan(0, 0, 0, 0, INK_TOTAL_MS));
				uploadDirtySlides();
			}
		}

		private void updateThreadMethod()
		{
			int num = 0;
			while (true)
			{
				bool flag = true;
				Thread.Sleep(THREAD_SLEEP_TS);
				num += THREAD_SLEEP_MS;
				if (num >= REFRESH_TOTAL_MS)
				{
					getSubmissions();
					num = 0;
				}
			}
		}

		private void enableColorButtons(bool val)
		{
			colorRadioBlack.Enabled = val;
			colorRadioRed.Enabled = val;
			colorRadioGreen.Enabled = val;
			colorRadioBlue.Enabled = val;
			colorRadioYellow.Enabled = val;
		}

		private void setWindowTitle()
		{
			if (webService != null)
			{
				Text = DEFAULT_WINDOW_TITLE + " [Connected] Classroom: " + classroom + ", Lecture: " + lecture;
			}
			else
			{
				Text = DEFAULT_WINDOW_TITLE + " [Disconnected]";
			}
		}

		private void updateSlideList()
		{
			if (webService != null)
			{
				if (slideList.Slides == null)
				{
					ArrayList arrayList = new ArrayList();
				}
				Slide slide3 = null;
				int num = 0;
				Slide selectedSlide = slideList.SelectedSlide;
				string[] fullLectureList = webService.getFullLectureList();
				string[] array = fullLectureList;
				foreach (string line in array)
				{
					Slide slide2 = WebService.createSlideFromTextLine(line);
					if (slide2 != null)
					{
						if (slide2.isSameSlide(slide3))
						{
							slide3.Current = slide2.Current;
							if (slide2.Iter > slide3.Iter)
							{
								slide3.Iter = slide2.Iter;
								slide3.Time = slide2.Time;
								webService.asyncDownloadImage(slide3.getAttrs(), slide3.SetStudentImage, false);
							}
						}
						else
						{
							slideList.Insert(num, slide2);
							webService.asyncDownloadImage(slide2.getAttrs(), slide2.SetStudentImage, false);
						}
						selectedSlide?.isSameSlide(slide2);
						num++;
					}
				}
				base.UseWaitCursor = true;
				base.Enabled = false;
				while (!webService.uploadQueueIsEmpty())
				{
					Thread.Sleep(1000);
				}
				Thread.Sleep(1000);
				base.Enabled = true;
				slideList.PerformLayout();
				base.UseWaitCursor = false;
				BringToFront();
			}
		}

		private void nextSlide()
		{
			int selectedIndex = slideList.SelectedIndex;
			if (selectedIndex < slideList.Slides.Count - 1)
			{
				slideList.SelectedIndex = selectedIndex + 1;
			}
		}

		private void prevSlide()
		{
			int selectedIndex = slideList.SelectedIndex;
			if (selectedIndex > 0)
			{
				slideList.SelectedIndex = selectedIndex - 1;
			}
		}

		private void updateMainSlideImage(Slide curSlide, int selectedIndex, int totalSlides)
		{
			setStatus("Getting slide...");
			try
			{
				if (curSlide != null)
				{
					inkPanel.Slide = curSlide;
					secondMonitorView.Slide = curSlide;
					subPreviewPanel.SubmissionThumbList = curSlide.SubmissionList;
				}
				setStatus("Done.");
			}
			catch (WebException ex)
			{
				MessageBox.Show(curSlide + " is invalid: " + ex.Message);
				setStatus("Error.");
			}
		}

		private void updateMinimizeButton(Slide curSlide, int selectedIndex, int totalSlides)
		{
			if (curSlide.Minimized)
			{
				minimizeCheck.Checked = true;
			}
			else
			{
				minimizeCheck.Checked = false;
			}
		}

		private void sendChangeSlide(Slide curSlide, int selectedIndex, int totalSlides)
		{
			if (webService != null)
			{
				webService.asyncChangeSlide(curSlide.getAttrs());
			}
		}

		private void GoToDownloadUpdate(object Sender, EventArgs e)
		{
			Process.Start("http://up.ucsd.edu/download/");
			notifyIcon.Visible = false;
		}

		private void StartVersionRequest()
		{
			try
			{
				versionReq.BeginGetResponse(FinishVersionRequest, null);
			}
			catch (Exception)
			{
			}
		}

		private bool isNewerVersion(string newVersion)
		{
			string[] array = newVersion.Split('.');
			string[] array2 = "3.3.1".Split('.');
			if (array.Length == array2.Length)
			{
				for (int i = 0; i < array.Length; i++)
				{
					int num = Convert.ToInt32(array[i]).CompareTo(Convert.ToInt32(array2[i]));
					if (num < 0)
					{
						return false;
					}
					if (num > 0)
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}

		private void FinishVersionRequest(IAsyncResult result)
		{
			try
			{
				Stream responseStream = versionReq.EndGetResponse(result).GetResponseStream();
				StreamReader streamReader = new StreamReader(responseStream);
				string text = streamReader.ReadToEnd().Trim();
				streamReader.Close();
				if (isNewerVersion(text))
				{
					notifyIcon.ShowBalloonTip(8000, "New version available", "Click here to download the latest version of Ubiquitous Presenter (" + text.Trim() + ").", ToolTipIcon.Info);
				}
				else
				{
					notifyIcon.Visible = false;
				}
			}
			catch (Exception)
			{
			}
		}

		private void startupActions()
		{
			monitorDetectionThread.Start();
			StartVersionRequest();
			startLocalLecture(cmdline_filename);
		}

		private void uploadDirtySlides()
		{
			lock (inkThreadLock)
			{
				foreach (Slide slide in slideList.Slides)
				{
					if (slide.Dirty)
					{
						slide.Dirty = false;
						Ink ink = default(Ink);
						lock (slide)
						{
							ink = slide.Ink.Clone();
						}
						MemoryStream memoryStream = new MemoryStream();
						Util.inkToPNG(ink, (int)slide.convertSizeForInkTransmission(1024f), (int)slide.convertSizeForInkTransmission(786f), memoryStream);
						byte[] img = memoryStream.ToArray();
						memoryStream.Close();
						webService.asyncUploadSlide(img, null, false, slide.getAttrs());
					}
				}
			}
		}

		private void DoColorButton(int index)
		{
			inkPanel.Color = penColors.getColor(index);
		}

		public void goToInstructorSlide()
		{
			int num = 0;
			foreach (Slide slide in slideList.Slides)
			{
				if (slide.Current)
				{
					slideList.SelectedIndex = num;
					break;
				}
				num++;
			}
		}

		public void newNoteOnCurrentSlide()
		{
			newNoteOnCurrentSlide(false);
		}

		public void newNoteOnCurrentSlide(bool isClone)
		{
			Slide slide = new Slide();
			slide.Type = "wb";
			slide.Id = slideList.getNextAvailableId(slide.Type);
			if (isClone)
			{
				slide.StudentImage = slideList.SelectedSlide.StudentImage;
				slide.InstructorImage = slideList.SelectedSlide.InstructorImage;
			}
			int num3 = 0;
			if (slideList.Slides.Count > 0)
			{
				int num2 = slideList.Slides.IndexOf(slideList.SelectedSlide);
				num3 = num2 + 1;
				if (!((Slide)slideList.Slides[num2]).Type.Equals("wb"))
				{
					for (; num3 < slideList.Slides.Count && ((Slide)slideList.Slides[num3]).Type.Equals("wb"); num3++)
					{
					}
				}
			}
			if (webService != null)
			{
				if (isClone)
				{
					webService.asyncCloneSlide(slideList.SelectedSlide.Id, slideList.SelectedSlide.Type, slide.Id, num3);
				}
				else
				{
					webService.asyncCreateBlankSlide(slide.getAttrs(), num3);
				}
			}
			slideList.Insert(num3, slide);
			slideList.SelectedIndex = num3;
		}

		public void addSubmissionToSlideList(Submission s)
		{
			Slide slide = new Slide();
			slide.Type = "wb";
			if (s.Image != null)
			{
				slide.StudentImage = s.Image;
				slide.Id = slideList.getNextAvailableId(slide.Type);
				int num3 = 0;
				if (slideList.Slides.Count > 0)
				{
					int num2 = slideList.Slides.IndexOf(s.ParentSlide);
					num3 = num2 + 1;
				}
				if (webService != null)
				{
					webService.asyncCloneSlide(s.Id, "ss", slide.Id, num3);
				}
				slideList.Insert(num3, slide);
				slideList.SelectedIndex = num3;
				slideList.PreviousSlideHadSubmissions = true;
			}
		}

		public void redrawCurrentThumbnail()
		{
			slideList.InvalidateCurrentThumbnail();
		}

		private bool determineCurrentColor(out int index, out RadioButton button)
		{
			if (colorRadioBlack.Checked)
			{
				index = 0;
				button = colorRadioBlack;
			}
			else if (colorRadioRed.Checked)
			{
				index = 1;
				button = colorRadioRed;
			}
			else if (colorRadioGreen.Checked)
			{
				index = 2;
				button = colorRadioGreen;
			}
			else if (colorRadioBlue.Checked)
			{
				index = 3;
				button = colorRadioBlue;
			}
			else
			{
				if (!colorRadioYellow.Checked)
				{
					index = 0;
					button = null;
					MessageBox.Show("Please select a color first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					return false;
				}
				index = 4;
				button = colorRadioYellow;
			}
			return true;
		}

		private void menuChangePen_Click(object sender, EventArgs e)
		{
			if (determineCurrentColor(out int index, out RadioButton radioButton))
			{
				penColors.changeColor(index);
				radioButton.BackColor = penColors.getColor(index);
				DoColorButton(index);
			}
		}

		private void menuOpenLecture_Click(object sender, EventArgs e)
		{
			setStatus("Loading...");
			startLocalLecture(null);
			setStatus("Loaded.");
		}

		public void toStateIdle()
		{
			menuConnect.Enabled = true;
			menuImport.Enabled = true;
			menuItemSync.Enabled = false;
			menuDisconnect.Enabled = false;
			menuOpenLecture.Enabled = true;
			menuItemClose.Enabled = false;
			menuSaveLecture.Enabled = false;
			menuExportLecture.Enabled = false;
			toggleSubmissionsEnabled.Enabled = false;
			showHideSubsButton.Enabled = false;
			SubsEnabled = false;
			minimizeCheck.Enabled = false;
			enableColorButtons(false);
			inkRadio.Enabled = false;
			thinInkRadio.Enabled = false;
			eraseRadio.Enabled = false;
			leftNextButton.Enabled = false;
			leftPrevButton.Enabled = false;
			rightNextButton.Enabled = false;
			rightPrevButton.Enabled = false;
			clearButton.Enabled = false;
			blankSlideButton.Enabled = false;
		}

		public void toStateOffline()
		{
			menuConnect.Enabled = false;
			menuImport.Enabled = false;
			if (webService != null)
			{
				menuItemSync.Enabled = true;
			}
			else
			{
				menuItemSync.Enabled = false;
			}
			menuDisconnect.Enabled = false;
			menuOpenLecture.Enabled = false;
			menuItemClose.Enabled = true;
			menuSaveLecture.Enabled = true;
			menuExportLecture.Enabled = true;
			toggleSubmissionsEnabled.Enabled = false;
			showHideSubsButton.Enabled = false;
			SubsEnabled = false;
			minimizeCheck.Enabled = true;
			enableColorButtons(!eraseRadio.Checked);
			inkRadio.Enabled = true;
			thinInkRadio.Enabled = true;
			eraseRadio.Enabled = true;
			leftNextButton.Enabled = true;
			leftPrevButton.Enabled = true;
			rightNextButton.Enabled = true;
			rightPrevButton.Enabled = true;
			clearButton.Enabled = true;
			blankSlideButton.Enabled = true;
		}

		public void toStateOnline()
		{
			menuConnect.Enabled = false;
			menuImport.Enabled = false;
			menuItemSync.Enabled = false;
			menuDisconnect.Enabled = true;
			menuOpenLecture.Enabled = false;
			menuItemClose.Enabled = true;
			menuSaveLecture.Enabled = true;
			menuExportLecture.Enabled = true;
			toggleSubmissionsEnabled.Enabled = true;
			showHideSubsButton.Enabled = true;
			minimizeCheck.Enabled = true;
			enableColorButtons(!eraseRadio.Checked);
			inkRadio.Enabled = true;
			thinInkRadio.Enabled = true;
			eraseRadio.Enabled = true;
			leftNextButton.Enabled = true;
			leftPrevButton.Enabled = true;
			rightNextButton.Enabled = true;
			rightPrevButton.Enabled = true;
			clearButton.Enabled = true;
			blankSlideButton.Enabled = true;
		}

		private void thinInkRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (thinInkRadio.Checked)
			{
				inkPanel.InkEditingMode = InkOverlayEditingMode.Ink;
				secondMonitorView.InkEditingMode = InkOverlayEditingMode.Ink;
				inkPanel.PenAttributes = penAttributes.getPen(0);
				secondMonitorView.PenAttributes = penAttributes.getPen(0);
				thinInkRadio.ImageIndex = 27;
				enableColorButtons(true);
			}
			else
			{
				thinInkRadio.ImageIndex = 28;
			}
		}

		private void menuItem5_Click(object sender, EventArgs e)
		{
			int i;
			if (thinInkRadio.Checked)
			{
				i = 0;
			}
			else
			{
				if (!inkRadio.Checked)
				{
					MessageBox.Show("Please select a pen first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					return;
				}
				i = 1;
			}
			penAttributes.changePen(i);
			inkPanel.PenAttributes = penAttributes.getPen(i);
			secondMonitorView.PenAttributes = penAttributes.getPen(i);
		}

		private void menuItem10_Click_1(object sender, EventArgs e)
		{
			penColors.changeColor(5);
			inkPanel.BackColor = penColors.getColor(5);
			secondMonitorView.BackColor = penColors.getColor(5);
		}

		public void MainSplitter_Invalidated(object sender, InvalidateEventArgs e)
		{
			inkPanel.ScaleInk();
			inkPanel.otherPanel.ScaleInk();
		}

		public static void showError(string msg)
		{
			MessageBox.Show(msg, DEFAULT_WINDOW_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
		}

		private void menuItem10_Click_2(object sender, EventArgs e)
		{
			if (determineCurrentColor(out int index, out RadioButton radioButton))
			{
				penColors.restoreDefaultColor(index);
				radioButton.BackColor = penColors.getColor(index);
				DoColorButton(index);
			}
		}

		private void updateSlideStatusBar(Slide curSlide, int selectedIndex, int totalSlides)
		{
			setSlideStatus("Slide " + (selectedIndex + 1) + " / " + totalSlides);
		}

		private void menuItem_subThumbSize_Click(object sender, EventArgs e)
		{
			subPreviewPanel.changeThumbnailSize();
		}

		private void changeSlidePreview_Click(object sender, EventArgs e)
		{
			SlidePreviewPanel.changeSlideSize();
		}

		private void setUploadQueueStatus(int queueSize)
		{
			ChangeUploadQueueStatusDelegate method = setUploadQueueStatus;
			Invoke(method, queueSize + " pending uploads");
		}

		private void setUploadQueueStatus(string msg)
		{
			uploadQueueStatus.Text = msg;
			statusBar.Invalidate();
		}
	}
}
