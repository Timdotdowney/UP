using Microsoft.Ink;
using NetworkIO;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace InstructorClient
{
	public class SlidePreviewPanel : Form
	{
		public delegate Image GetImageDelegate();

		public delegate Ink GetInkDelegate();

		public delegate int ConvertSizeDelegate(float original);

		public const string WIDTH_KEY = "slide_preview_width";

		public const string HEIGHT_KEY = "slide_preview_height";

		public const string SUB_WIDTH_KEY = "submission_preview_width";

		public const string SUB_HEIGHT_KEY = "submission_preview_height";

		public const int DEFAULT_PREVIEW_WIDTH = 256;

		public const int DEFAULT_PREVIEW_HEIGHT = 192;

		private static int sl_preview_width = PropertiesManager.getSavedNumber("slide_preview_width", 256);

		private static int sl_preview_height = PropertiesManager.getSavedNumber("slide_preview_height", 192);

		private static int sub_preview_width = PropertiesManager.getSavedNumber("submission_preview_width", 256);

		private static int sub_preview_height = PropertiesManager.getSavedNumber("submission_preview_height", 192);

		private static DimensionChooser sizeChooser = new DimensionChooser("Slide Preview", sl_preview_width, sl_preview_height, 256, 192);

		private static DimensionChooser subSizeChooser = new DimensionChooser("Submission Preview", sub_preview_width, sub_preview_height, 256, 192);

		private GetImageDelegate getImage;

		private GetInkDelegate getInk;

		private ConvertSizeDelegate convertSize;

		private bool useSubSize;

		private IContainer components = null;

		public static void changeSlideSize()
		{
			if (sizeChooser.ShowDialog() == DialogResult.OK)
			{
				sl_preview_width = sizeChooser.SelectedWidth;
				sl_preview_height = sizeChooser.SelectedHeight;
				PropertiesManager.SetStringProperty("slide_preview_width", sl_preview_width.ToString());
				PropertiesManager.SetStringProperty("slide_preview_height", sl_preview_height.ToString());
			}
		}

		public static void changeSubmissionSize()
		{
			if (subSizeChooser.ShowDialog() == DialogResult.OK)
			{
				sub_preview_width = subSizeChooser.SelectedWidth;
				sub_preview_height = subSizeChooser.SelectedHeight;
				PropertiesManager.SetStringProperty("submission_preview_width", sub_preview_width.ToString());
				PropertiesManager.SetStringProperty("submission_preview_height", sub_preview_height.ToString());
			}
		}

		public SlidePreviewPanel(GetImageDelegate img, GetInkDelegate ink, ConvertSizeDelegate sz)
		{
			InitializeComponent();
			getImage = img;
			getInk = ink;
			convertSize = sz;
			base.MouseLeave += SlidePreviewPanel_MouseLeave;
			base.Click += SlidePreviewPanel_Click;
			base.MouseUp += SlidePreviewPanel_MouseUp;
			BackColor = Color.White;
			useSubSize = false;
		}

		public SlidePreviewPanel(GetImageDelegate img)
			: this(img, null, null)
		{
			useSubSize = true;
		}

		private void SlidePreviewPanel_MouseUp(object sender, MouseEventArgs e)
		{
			hidePreview();
		}

		private void SlidePreviewPanel_Click(object sender, EventArgs e)
		{
			hidePreview();
		}

		private void SlidePreviewPanel_MouseLeave(object sender, EventArgs e)
		{
			hidePreview();
		}

		public void showPreview(Control c)
		{
			int num3 = 0;
			int num2 = 0;
			base.Width = ((getCurrentWidth() < c.Width + 15) ? (c.Width + 15) : getCurrentWidth());
			base.Height = ((getCurrentHeight() < c.Height + 10) ? (c.Height + 10) : getCurrentHeight());
			while (c != null)
			{
				int num4 = num3;
				Point location = c.Location;
				num3 = num4 + location.X;
				int num5 = num2;
				location = c.Location;
				num2 = num5 + location.Y;
				c = c.Parent;
			}
			base.Location = new Point(num3, num2 + 45);
			Show();
		}

		public void hidePreview()
		{
			Hide();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Image image = getImage();
			Size clientSize;
			if (image != null)
			{
				Graphics graphics = e.Graphics;
				Image image2 = image;
				clientSize = base.ClientSize;
				int width = clientSize.Width;
				clientSize = base.ClientSize;
				graphics.DrawImage(image2, 0, 0, width, clientSize.Height);
			}
			if (convertSize != null && getInk != null)
			{
				Ink ink = getInk();
				if (ink != null && ink.Strokes.Count > 0)
				{
					Graphics graphics2 = e.Graphics;
					Bitmap image3 = Util.createInkImage(ink, convertSize(1024f), convertSize(786f));
					clientSize = base.ClientSize;
					int width2 = clientSize.Width;
					clientSize = base.ClientSize;
					graphics2.DrawImage(image3, 0, 0, width2, clientSize.Height);
				}
			}
		}

		private int getCurrentWidth()
		{
			if (useSubSize)
			{
				return sub_preview_width;
			}
			return sl_preview_width;
		}

		private int getCurrentHeight()
		{
			if (useSubSize)
			{
				return sub_preview_height;
			}
			return sl_preview_height;
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
			SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(280, 258);
			base.ControlBox = false;
			ForeColor = System.Drawing.SystemColors.ControlText;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SlidePreviewPanel";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			base.TopMost = true;
			ResumeLayout(false);
		}
	}
}
