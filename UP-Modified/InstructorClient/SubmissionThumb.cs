using NetworkIO;
using SlideModel;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace InstructorClient
{
	public class SubmissionThumb : Panel
	{
		public const string WIDTH_KEY = "submission_thumb_width";

		public const string HEIGHT_KEY = "submission_thumb_height";

		public const int DEFAULT_WIDTH = 256;

		public const int DEFAULT_HEIGHT = 192;

		public const int BORDER_THICKNESS = 4;

		private static int SUB_WIDTH = PropertiesManager.getSavedNumber("submission_thumb_width", 256);

		private static int SUB_HEIGHT = PropertiesManager.getSavedNumber("submission_thumb_height", 192);

		private static DimensionChooser sizeChooser = new DimensionChooser("Submission", SUB_WIDTH, SUB_HEIGHT, 256, 192);

		private static Color BORDER_COLOR = Color.Blue;

		private Color defaultBackColor;

		private Submission sub;

		private SlidePreviewPanel previewPanel;

		private IContainer components;

		public static int SubWidth => SUB_WIDTH;

		public static int SubHeight => SUB_HEIGHT;

		public Submission Sub => sub;

		public static void changeThumbnailSize()
		{
			if (sizeChooser.ShowDialog() == DialogResult.OK)
			{
				SUB_WIDTH = sizeChooser.SelectedWidth;
				SUB_HEIGHT = sizeChooser.SelectedHeight;
				PropertiesManager.SetStringProperty("submission_thumb_width", SUB_WIDTH.ToString());
				PropertiesManager.SetStringProperty("submission_thumb_height", SUB_HEIGHT.ToString());
			}
		}

		public SubmissionThumb(Submission sub)
		{
			InitializeComponent();
			previewPanel = new SlidePreviewPanel(getImageMethod);
			defaultBackColor = BackColor;
			this.sub = sub;
			base.Width = SUB_WIDTH;
			base.Height = SUB_HEIGHT;
			base.DoubleClick += sub.DoubleClick;
			base.Click += SubmissionThumb_Click;
			base.MouseUp += SubmissionThumb_MouseUp;
			base.MouseMove += SubmissionThumb_MouseMove;
		}

		private void SubmissionThumb_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && !base.ClientRectangle.Contains(e.Location))
			{
				previewPanel.showPreview(this);
			}
		}

		private void SubmissionThumb_MouseUp(object sender, MouseEventArgs e)
		{
			previewPanel.hidePreview();
		}

		private void SubmissionThumb_Click(object sender, EventArgs e)
		{
			sub.toggleBordered();
			Invalidate();
		}

		private Image getImageMethod()
		{
			return sub.Image;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			while (sub.Image == null)
			{
				Thread.Sleep(1000);
			}
			RectangleF rect = e.Graphics.VisibleClipBounds;
			if (sub.Bordered)
			{
				BackColor = BORDER_COLOR;
				RectangleF rectangleF = new RectangleF(rect.X + 4f, rect.Y + 4f, rect.Width - 8f, rect.Height - 8f);
				rect = rectangleF;
			}
			else
			{
				BackColor = defaultBackColor;
			}
			e.Graphics.DrawImage(sub.Image, rect);
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
		}
	}
}
